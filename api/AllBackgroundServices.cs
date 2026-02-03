using Microsoft.Extensions.Hosting; // Arka plan servisi iskeleti için gerekli
using System; // Tarih ve konsol işlemleri için gerekli
using System.Threading; // Durdurma sinyali için gerekli
using System.Threading.Tasks; // Bekleme (Delay) işlemleri için gerekli

namespace api
{
    // Bu sınıf, program çalıştığı sürece arkada kendi başına çalışan bir işçidir.
    public class AllBackgroundServices : BackgroundService
    {
        // Bu metot, servis çalışmaya başladığı anda devreye giren ana döngüdür.
        protected override async Task ExecuteAsync(CancellationToken durdurmaSinyali)
        {
            // Program kapatılana kadar bu döngü hep çalışsın diyoruz.
            while (!durdurmaSinyali.IsCancellationRequested)
            {
                // 1. ADIM: Ekrana başladığımızı haber verelim.
                Console.WriteLine("Sayım işlemi başlıyor... Saat: " + DateTime.Now);

                try
                {
                    // 2. ADIM: 1'den 100'e kadar sayan basit bir döngü.
                    for (int sayac = 1; sayac <= 1000; sayac++)
                    {
                        // Eğer tam sayarken birisi programı kapatırsa, saymayı yarıda kes.
                        if (durdurmaSinyali.IsCancellationRequested) break;

                        // Sayıyı ekrana yazdırıyoruz.
                        Console.WriteLine("Şu anki sayı: " + sayac);
                    }

                    // 3. ADIM: Sayım bitti, bilgi veriyoruz.
                    Console.WriteLine("100'e kadar saydım. Şimdi 1 dakika dinleneceğim.");
                }
                catch (Exception hata)
                {
                    // Bir sorun çıkarsa burası çalışır.
                    Console.WriteLine("Bir hata oluştu: " + hata.Message);
                }

                // 4. ADIM: Bilgisayarı yormamak için 1 dakika boyunca hiçbir şey yapmadan bekle.
                // TimeSpan.FromMinutes(1) = 1 Dakika demektir.
                await Task.Delay(TimeSpan.FromMinutes(1), durdurmaSinyali);
            }
        }
    }
}