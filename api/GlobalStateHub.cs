using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;

namespace api
{
    public sealed class GlobalStateHub : IDisposable
    {
        #region Events - Dinleyicilere Sinyal Gönderir

        public event Func<string, Task>? OnDataChanged;
        public event Func<string, string, Task>? OnSystemError;

        // Online kullanıcı değişikliği eventi
        public event Func<Task>? OnOnlineUsersChanged;

        // ✅ Sesli/Görüntülü arama sinyal eventi (WebRTC signaling: offer/answer/ice/hangup/ring)
        // Args: fromUserId, toUserId, signalType, payload (JSON)
        public event Func<Guid, Guid, string, string, Task>? OnCallSignal;

        #endregion

        #region Configuration

        private readonly IMemoryCache _cache;
        private readonly SemaphoreSlim _lock = new(1, 1);
        private readonly ConcurrentDictionary<string, DateTime> _lastBroadcast = new();

        // ✅ Heartbeat tabanlı online takip: UserId → Son heartbeat zamanı
        // Tarayıcı aniden kapansa bile TIMEOUT_SECONDS sonra otomatik offline olur
        private readonly ConcurrentDictionary<Guid, DateTime> _onlineUsers = new();
        private readonly Timer _cleanupTimer;
        private const int TIMEOUT_SECONDS = 35;      // 35sn heartbeat gelmezse offline say
        private const int CLEANUP_INTERVAL_MS = 10_000; // Her 10sn'de bir kontrol et

        private const int DEBOUNCE_MS = 200;
        private const int CACHE_SECONDS = 5;
        private bool _disposed;

        #endregion

        #region Constructor

        public GlobalStateHub(IMemoryCache memoryCache)
        {
            _cache = memoryCache;

            // ✅ Süresi dolan heartbeat'leri temizleyen arka plan zamanlayıcısı
            // Async lambda + try/catch: callback içindeki unhandled exception process'i çökertmesin
            _cleanupTimer = new Timer(
                async _ =>
                {
                    try
                    {
                        await CleanupStaleUsers();
                        CleanupOldDebounceKeys();
                    }
                    catch (ObjectDisposedException) { }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[GlobalStateHub] Cleanup timer hata: {ex.Message}");
                    }
                },
                null,
                CLEANUP_INTERVAL_MS,
                CLEANUP_INTERVAL_MS);
        }

        #endregion

        #region CORE: Broadcast Sistemi

        /// <summary>
        /// TÜM DİNLEYİCİLERE SİNYAL GÖNDER (Debounced)
        /// Örnek: await StateHub.NotifyDataChanged("Users");
        /// → Tüm açık sayfalar otomatik güncellenir
        /// PERFORMANS:
        /// - Saniyede 100 çağrı → DEBOUNCE_MS'de 1 broadcast
        /// - Race condition yok: debounce kontrolü + güncelleme atomik (lock içinde)
        /// - Broadcast lock dışında: deadlock riski yok
        /// </summary>
        public async Task NotifyDataChanged(string entityName)
        {
            if (_disposed) return;

            var key = $"debounce_{entityName}";
            List<Func<string, Task>>? handlers = null;

            await _lock.WaitAsync();
            try
            {
                var now = DateTime.UtcNow;

                if (_lastBroadcast.TryGetValue(key, out var last) &&
                    (now - last).TotalMilliseconds < DEBOUNCE_MS)
                {
                    return;
                }

                _lastBroadcast[key] = now;
                _cache.Remove($"data_{entityName}");

                handlers = OnDataChanged?.GetInvocationList()
                    .Cast<Func<string, Task>>()
                    .ToList();
            }
            finally
            {
                _lock.Release();
            }

            if (handlers != null)
                await Task.WhenAll(handlers.Select(h => SafeInvoke(h, entityName)));
        }

        /// <summary>
        /// GLOBAL HATA BİLDİRİMİ
        /// Tüm aktif kullanıcılara sistem mesajı gönderir
        /// </summary>
        public async Task NotifySystemError(string title, string message)
        {
            if (_disposed || OnSystemError == null) return;

            var handlers = OnSystemError.GetInvocationList()
                .Cast<Func<string, string, Task>>()
                .ToList();

            await Task.WhenAll(handlers.Select(h => SafeInvoke(h, title, message)));
        }

        #endregion

        #region CORE: RAM Cache Sistemi (5 Saniye Buffer)

        public async Task<T?> GetOrLoadAsync<T>(
            string cacheKey,
            Func<Task<T>> loadFromDb,
            CancellationToken ct = default) where T : class
        {
            if (_cache.TryGetValue(cacheKey, out T? cached))
                return cached;

            await _lock.WaitAsync(ct);
            try
            {
                if (_cache.TryGetValue(cacheKey, out cached))
                    return cached;

                var fresh = await loadFromDb();

                if (fresh != null)
                {
                    _cache.Set(cacheKey, fresh, new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(CACHE_SECONDS),
                        Priority = CacheItemPriority.High
                    });
                }

                return fresh;
            }
            finally
            {
                _lock.Release();
            }
        }

        /// <summary>
        /// Cache'i manuel temizle (veri güncellendiyse)
        /// </summary>
        public void ClearCache(string cacheKey)
        {
            _cache.Remove(cacheKey);
        }

        #endregion

        #region Online Kullanıcı Yönetimi (Heartbeat Tabanlı)

        /// <summary>
        /// Kullanıcının heartbeat'ini günceller.
        /// Component hem OnInitializedAsync'te hem de periyodik timer'da çağırır.
        /// </summary>
        public async Task Heartbeat(Guid userId)
        {
            if (_disposed) return;

            var wasOnline = _onlineUsers.ContainsKey(userId);
            _onlineUsers[userId] = DateTime.UtcNow;

            // Yeni giriş yaptıysa broadcast gönder
            if (!wasOnline)
                await NotifyOnlineUsersChanged();
        }

        /// <summary>
        /// Kullanıcıyı çevrimdışı yap (temiz çıkış: sekme kapatma, logout).
        /// </summary>
        public async Task UnregisterOnlineUser(Guid userId)
        {
            if (_disposed) return;

            if (_onlineUsers.TryRemove(userId, out _))
                await NotifyOnlineUsersChanged();
        }

        /// <summary>
        /// Mevcut online kullanıcı ID setini döndürür (snapshot).
        /// </summary>
        public HashSet<Guid> GetOnlineUsers()
        {
            return new HashSet<Guid>(_onlineUsers.Keys);
        }

        /// <summary>
        /// Heartbeat süresi dolan kullanıcıları temizler.
        /// Tarayıcı çökmesi / bağlantı kopmasında otomatik offline yapar.
        /// </summary>
        private async Task CleanupStaleUsers()
        {
            if (_disposed) return;

            var threshold = DateTime.UtcNow.AddSeconds(-TIMEOUT_SECONDS);
            var stale = _onlineUsers
                .Where(kv => kv.Value < threshold)
                .Select(kv => kv.Key)
                .ToList();

            if (!stale.Any()) return;

            foreach (var uid in stale)
                _onlineUsers.TryRemove(uid, out _);

            await NotifyOnlineUsersChanged();
        }

        /// <summary>
        /// Online kullanıcı listesi değişti sinyali (Debounced).
        /// </summary>
        private async Task NotifyOnlineUsersChanged()
        {
            if (_disposed) return;

            const string key = "debounce_online_users";
            List<Func<Task>>? handlers = null;

            await _lock.WaitAsync();
            try
            {
                var now = DateTime.UtcNow;
                if (_lastBroadcast.TryGetValue(key, out var last) &&
                    (now - last).TotalMilliseconds < DEBOUNCE_MS)
                {
                    return;
                }
                _lastBroadcast[key] = now;

                handlers = OnOnlineUsersChanged?.GetInvocationList()
                    .Cast<Func<Task>>()
                    .ToList();
            }
            finally
            {
                _lock.Release();
            }

            if (handlers != null)
                await Task.WhenAll(handlers.Select(h => SafeInvokeNoArg(h)));
        }

        #endregion

        #region Call Signaling (Sesli/Görüntülü Arama - WebRTC)

        /// <summary>
        /// Aramaya dair sinyali (offer/answer/ice/ring/accept/reject/hangup) hedef kullanıcıya yönlendirir.
        /// Broadcast tüm dinleyicilere gider; component tarafı toUserId filtresiyle kendi mesajını ayırır.
        /// Bu event DEBOUNCE EDİLMEZ — ICE candidate'ları geciktirmek bağlantıyı bozar.
        /// </summary>
        public async Task SendCallSignal(Guid fromUserId, Guid toUserId, string signalType, string payload)
        {
            if (_disposed || OnCallSignal == null) return;
            if (string.IsNullOrWhiteSpace(signalType)) return;

            var handlers = OnCallSignal.GetInvocationList()
                .Cast<Func<Guid, Guid, string, string, Task>>()
                .ToList();

            await Task.WhenAll(handlers.Select(h => SafeInvokeCallSignal(h, fromUserId, toUserId, signalType, payload ?? string.Empty)));
        }

        #endregion

        #region Debounce Bakım (Memory Sızıntısı Önleme)

        // 30 saniyeden eski debounce anahtarlarını temizle. _lastBroadcast sonsuz büyümesin.
        private void CleanupOldDebounceKeys()
        {
            if (_disposed) return;

            var threshold = DateTime.UtcNow.AddSeconds(-30);
            var stale = _lastBroadcast
                .Where(kv => kv.Value < threshold)
                .Select(kv => kv.Key)
                .ToList();

            foreach (var key in stale)
                _lastBroadcast.TryRemove(key, out _);
        }

        private static async Task SafeInvoke(Func<string, Task> handler, string arg)
        {
            try { await handler(arg); }
            catch (ObjectDisposedException) { }
            catch { }
        }

        private static async Task SafeInvoke(Func<string, string, Task> handler, string arg1, string arg2)
        {
            try { await handler(arg1, arg2); }
            catch (ObjectDisposedException) { }
            catch { }
        }

        private static async Task SafeInvokeNoArg(Func<Task> handler)
        {
            try { await handler(); }
            catch (ObjectDisposedException) { }
            catch { }
        }

        private static async Task SafeInvokeCallSignal(
            Func<Guid, Guid, string, string, Task> handler,
            Guid fromUserId, Guid toUserId, string signalType, string payload)
        {
            try { await handler(fromUserId, toUserId, signalType, payload); }
            catch (ObjectDisposedException) { }
            catch { }
        }

        /// <summary>
        /// Tüm event subscriber'ları temizle (memory leak önlemi)
        /// </summary>
        public void ClearAllSubscribers()
        {
            OnDataChanged = null;
            OnSystemError = null;
            OnOnlineUsersChanged = null;
            OnCallSignal = null;
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            _cleanupTimer.Dispose();
            ClearAllSubscribers();
            _lock.Dispose();
            _lastBroadcast.Clear();
            _onlineUsers.Clear();

            GC.SuppressFinalize(this);
        }

        #endregion

        #region Monitoring

        /// <summary>
        /// Sistem durumu - Debug amaçlı
        /// </summary>
        public string GetStats()
        {
            var subscriberCount = OnDataChanged?.GetInvocationList().Length ?? 0;
            return $"Active Subscribers: {subscriberCount}, Cache Entries: {_lastBroadcast.Count}, Online Users: {_onlineUsers.Count}";
        }

        #endregion
    }
}
