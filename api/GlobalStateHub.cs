using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;

namespace api
{
    public sealed class GlobalStateHub : IDisposable
    {
        #region Events - Dinleyicilere Sinyal Gönderir

        public event Func<string, Task>? OnDataChanged;

        public event Func<string, string, Task>? OnSystemError;

        #endregion

        #region Configuration

        private readonly IMemoryCache _cache;
        private readonly SemaphoreSlim _lock = new(1, 1);
        private readonly ConcurrentDictionary<string, DateTime> _lastBroadcast = new();

        private const int DEBOUNCE_MS = 250;          
        private const int CACHE_SECONDS = 5;          
        private bool _disposed;

        #endregion

        #region Constructor

        public GlobalStateHub(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        #endregion

        #region CORE: Broadcast Sistemi

        /// <summary>
        /// TÜM DİNLEYİCİLERE SİNYAL GÖNDER (Debounced)
        /// 
        /// Örnek: await StateHub.NotifyDataChanged("Users");
        /// → Tüm açık sayfalar otomatik güncellenir
        /// 
        /// PERFORMANS:
        /// - Saniyede 100 çağrı → 250ms'de 1 broadcast (4 broadcast/sn)
        /// - 10,000 kullanıcı × 4 broadcast/sn = 40,000 render/sn (yönetilebilir)
        /// </summary>
        public async Task NotifyDataChanged(string entityName)
        {
            if (_disposed) return;

            var now = DateTime.UtcNow;
            var key = $"debounce_{entityName}";

            // ✅ THROTTLING: 250ms içinde tekrar çağrıldıysa atla
            if (_lastBroadcast.TryGetValue(key, out var last))
            {
                if ((now - last).TotalMilliseconds < DEBOUNCE_MS)
                    return; // Çok erken, bekle
            }

            await _lock.WaitAsync();
            try
            {
                _lastBroadcast[key] = now;

                // Cache'i invalidate et (sonraki sorgu fresh data çekecek)
                _cache.Remove($"data_{entityName}");

                // ✅ TÜM DİNLEYİCİLERE BROADCAST
                if (OnDataChanged != null)
                {
                    var handlers = OnDataChanged.GetInvocationList()
                        .Cast<Func<string, Task>>()
                        .ToList();

                    // Paralel broadcast - non-blocking
                    await Task.WhenAll(handlers.Select(h => SafeInvoke(h, entityName)));
                }
            }
            finally
            {
                _lock.Release();
            }
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
            // 1. Cache'de var mı?
            if (_cache.TryGetValue(cacheKey, out T? cached))
                return cached;

            // 2. Yok, DB'den çek (thread-safe)
            await _lock.WaitAsync(ct);
            try
            {
                // Double-check (başka thread cache'lemiş olabilir)
                if (_cache.TryGetValue(cacheKey, out cached))
                    return cached;

                // DB'den çek
                var fresh = await loadFromDb();

                if (fresh != null)
                {
                    // 1 saniye cache'le
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

        #region Memory Leak Koruması

        /// <summary>
        /// Event handler'ı güvenli çağır
        /// Bir circuit disposed olsa bile diğerleri etkilenmez
        /// </summary>
        private static async Task SafeInvoke(Func<string, Task> handler, string arg)
        {
            try
            {
                await handler(arg);
            }
            catch (ObjectDisposedException)
            {
                // Circuit disposed (kullanıcı sayfayı kapattı), normal
            }
            catch
            {
                // Bir handler hata verse bile diğerleri çalışmalı
            }
        }

        private static async Task SafeInvoke(Func<string, string, Task> handler, string arg1, string arg2)
        {
            try
            {
                await handler(arg1, arg2);
            }
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
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            ClearAllSubscribers();
            _lock.Dispose();
            _lastBroadcast.Clear();

            GC.SuppressFinalize(this);
        }

        #endregion

        #region Monitoring (Opsiyonel)

        /// <summary>
        /// Sistem durumu - Debug amaçlı
        /// </summary>
        public string GetStats()
        {
            var subscriberCount = OnDataChanged?.GetInvocationList().Length ?? 0;
            return $"Active Subscribers: {subscriberCount}, Cache Entries: {_lastBroadcast.Count}";
        }

        #endregion
    }
}