using System;
using System.Collections.Generic;
using System.Text;

namespace api
{
    public class GlobalSecurityProvider
    {
        // Senin belirlediğin gizli anahtar
        private static readonly int Salt = 16;

        /// <summary>
        /// Metni önce anahtar ile karıştırır, sonra Base64 formatına sokar.
        /// Japonca, Arapça ve Türkçe karakterlerle %100 uyumludur.
        /// </summary>
        public static string Sifrele(string metin)
        {
            if (string.IsNullOrEmpty(metin)) return metin;

            // 1. Metni Byte dizisine çevir (UTF8 her dili destekler)
            byte[] veriler = Encoding.UTF8.GetBytes(metin);

            // 2. Her bir byte'ı senin anahtarınla manipüle et
            for (int i = 0; i < veriler.Length; i++)
            {
                // Byte bazlı kaydırma (256 modunda döner, veri kaybı olmaz)
                veriler[i] = (byte)((veriler[i] + Salt) % 256);
            }

            // 3. Güvenli saklama için Base64'e çevir
            return Convert.ToBase64String(veriler);
        }

        /// <summary>
        /// Base64 metni çözer ve anahtar ile orijinal haline getirir.
        /// </summary>
        public static string Coz(string base64Metin)
        {
            if (string.IsNullOrEmpty(base64Metin)) return base64Metin;

            try
            {
                // 1. Base64'ten Byte dizisine geri dön
                byte[] veriler = Convert.FromBase64String(base64Metin);

                // 2. Manipülasyonu geri al
                for (int i = 0; i < veriler.Length; i++)
                {
                    int orijinalByte = veriler[i] - (Salt % 256);
                    if (orijinalByte < 0) orijinalByte += 256;
                    veriler[i] = (byte)orijinalByte;
                }

                // 3. Tekrar okunabilir metne çevir
                return Encoding.UTF8.GetString(veriler);
            }
            catch
            {
                // Eğer veri Base64 değilse veya bozulmuşsa orijinali döner
                return "Hata: Veri Çözülemedi";
            }
        }
    }
}
