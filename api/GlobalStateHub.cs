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
        /// - Saniyede 100 çağrı → DEBOUNCE_MS'de 1 broadcast
        /// - Race condition yok: debounce kontrolü + güncelleme atomik (lock içinde)
        /// - Broadcast lock dışında: deadlock riski yok
        /// </summary>
        public async Task NotifyDataChanged(string entityName)
        {
            if (_disposed) return;

            var key = $"debounce_{entityName}";
            List<Func<string, Task>>? handlers = null;

            // ✅ LOCK İÇİNDE: Debounce kontrolü + güncelleme atomik
            // Race condition önlemi: okuma ve yazma aynı kritik bölgede
            await _lock.WaitAsync();
            try
            {
                var now = DateTime.UtcNow;

                if (_lastBroadcast.TryGetValue(key, out var last) &&
                    (now - last).TotalMilliseconds < DEBOUNCE_MS)
                {
                    return; // Çok erken, atla
                }

                _lastBroadcast[key] = now;

                // Cache'i invalidate et (sonraki sorgu fresh data çekecek)
                _cache.Remove($"data_{entityName}");

                // Handler listesini lock içinde kopyala, broadcast dışarıda yap
                handlers = OnDataChanged?.GetInvocationList()
                    .Cast<Func<string, Task>>()
                    .ToList();
            }
            finally
            {
                _lock.Release();
            }

            // ✅ LOCK DIŞINDA: Broadcast
            // Kritik: handler içinden NotifyDataChanged çağrılsa bile deadlock yok
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

        #region Monitoring

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