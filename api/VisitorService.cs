using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;

namespace api
{
    public class VisitorService
    {
        private readonly ConcurrentDictionary<string, DateTime> _ziyaretciler = new();
        private int _aktifSayisi;
        public int AktifSayisi => _aktifSayisi;

        public VisitorService()
        {
            // Arka planda tek bir temizlik görevi başlat (Periyodik Timer)
            _ = TemizlikDongusu();
        }

        public void SinyalVer(string id)
        {
            _ziyaretciler[id] = DateTime.Now;
        }

        public void Ayrildi(string id)
        {
            _ziyaretciler.TryRemove(id, out _);
            _aktifSayisi = _ziyaretciler.Count;
        }

        private async Task TemizlikDongusu()
        {
            while (true)
            {
                await Task.Delay(2000); // 2 saniyede bir kontrol et

                var sinir = DateTime.Now.AddSeconds(-6); // 6 saniye sinyal gelmezse sil
                var silinecekler = _ziyaretciler.Where(x => x.Value < sinir).Select(x => x.Key);

                foreach (var key in silinecekler)
                {
                    _ziyaretciler.TryRemove(key, out _);
                }

                _aktifSayisi = _ziyaretciler.Count;
            }
        }
    }
}


