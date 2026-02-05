using Microsoft.Extensions.Hosting; // Arka plan servisi iskeleti için gerekli
using System; // Tarih ve konsol işlemleri için gerekli
using System.Threading; // Durdurma sinyali için gerekli
using System.Threading.Tasks; // Bekleme (Delay) işlemleri için gerekli

namespace api
{
    //public class AllBackgroundServices : BackgroundService
    //{
    //    protected override async Task ExecuteAsync(CancellationToken durdurmaSinyali)
    //    {
    //        while (!durdurmaSinyali.IsCancellationRequested)
    //        {
    //            Console.WriteLine("Sayım işlemi başlıyor... Saat: " + DateTime.Now);
    //            try
    //            {
    //                for (int sayac = 1; sayac <= 100; sayac++)
    //                {
    //                    if (durdurmaSinyali.IsCancellationRequested) break;
    //                    Console.WriteLine("Şu anki sayı: " + sayac);
    //                }
    //                Console.WriteLine("100'e kadar saydım. Şimdi 1 dakika dinleneceğim.");
    //            }
    //            catch (Exception hata)
    //            {
    //                Console.WriteLine("Bir hata oluştu: " + hata.Message);
    //            }
    //            await Task.Delay(TimeSpan.FromMinutes(1), durdurmaSinyali);
    //        }
    //    }
    //}
}

