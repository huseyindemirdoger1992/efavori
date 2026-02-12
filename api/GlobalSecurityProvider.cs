using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace api
{
    public static class GlobalSecurityProvider
    {
        private static readonly byte[] MasterKey;
        private static readonly byte[] HmacKey;

        // Thread-safe için object pool
        private static readonly ArrayPool<byte> ByteArrayPool = ArrayPool<byte>.Shared;

        static GlobalSecurityProvider()
        {
            string passphrase = Environment.GetEnvironmentVariable("ENCRYPTION_KEY")
                ?? "Dv1QrnBA1GY6FPGVFLCelAs8ZVm7rMV+NJ/K/kC05Uft0k68rzy/yzPWmsDl+3uPYqLI57gjraX3npRssAgCmgrb2OtXO735gBZD0hE1S0OZK/6p6VRFd6AIKN5G9LLR";

            string salt = Environment.GetEnvironmentVariable("ENCRYPTION_SALT")
                ?? "KL7WlB1/hK2HkEyBp0yJfdsLIFfmwEKCTcHzEqVMMKLC6t1E90QbgVfk6ZhfNf/IoR+MvZTQey2ZfRxty9nu1+DHZNqVWSK05TYnT5oMRBEvgWjWo/VeGTjXJwk7aBUD";

            MasterKey = DerivedKeyFromPassphrase(passphrase, salt, 100000);
            HmacKey = DerivedKeyFromPassphrase(passphrase, salt + "_HMAC", 100000);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte[] DerivedKeyFromPassphrase(string passphrase, string salt, int iterations)
        {
            using (var rfc2898 = new Rfc2898DeriveBytes(
                Encoding.UTF8.GetBytes(passphrase),
                Encoding.UTF8.GetBytes(salt),
                iterations,
                HashAlgorithmName.SHA512))
            {
                return rfc2898.GetBytes(32);
            }
        }

        public static string Encrypt(string metin)
        {
            if (string.IsNullOrEmpty(metin)) return metin;

            byte[] plainBytes = null;
            byte[] iv = null;
            byte[] encryptedBuffer = null;
            byte[] finalBuffer = null;

            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.KeySize = 256;
                    aes.BlockSize = 128;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.Key = MasterKey;

                    aes.GenerateIV();
                    iv = aes.IV;

                    // Unicode encoding (tüm dilleri destekler)
                    plainBytes = Encoding.Unicode.GetBytes(metin);

                    // Şifreli veri için buffer tahmin et
                    int estimatedSize = plainBytes.Length + aes.BlockSize;
                    encryptedBuffer = ByteArrayPool.Rent(estimatedSize);

                    int encryptedLength;

                    using (var encryptor = aes.CreateEncryptor(aes.Key, iv))
                    {
                        // ICryptoTransform ile direkt transform
                        encryptedLength = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length).Length;

                        using (MemoryStream ms = new MemoryStream())
                        {
                            ms.Write(iv, 0, iv.Length);

                            using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                            {
                                cs.Write(plainBytes, 0, plainBytes.Length);
                                cs.FlushFinalBlock();
                            }

                            byte[] encryptedData = ms.ToArray();

                            // HMAC hesaplama
                            using (var hmac = new HMACSHA512(HmacKey))
                            {
                                byte[] hash = hmac.ComputeHash(encryptedData);

                                // Final paket: [IV + Encrypted + HMAC]
                                int finalLength = encryptedData.Length + hash.Length;
                                finalBuffer = ByteArrayPool.Rent(finalLength);

                                Buffer.BlockCopy(encryptedData, 0, finalBuffer, 0, encryptedData.Length);
                                Buffer.BlockCopy(hash, 0, finalBuffer, encryptedData.Length, hash.Length);

                                // Base64 encoding
                                return Convert.ToBase64String(finalBuffer, 0, finalLength);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Şifreleme hatası: {ex.Message}");
                return null;
            }
            finally
            {
                // Kritik: Bellekte hassas verileri temizle
                if (plainBytes != null)
                {
                    CryptographicOperations.ZeroMemory(plainBytes);
                }
                if (iv != null)
                {
                    CryptographicOperations.ZeroMemory(iv);
                }
                if (encryptedBuffer != null)
                {
                    CryptographicOperations.ZeroMemory(encryptedBuffer.AsSpan(0, encryptedBuffer.Length));
                    ByteArrayPool.Return(encryptedBuffer, clearArray: true);
                }
                if (finalBuffer != null)
                {
                    CryptographicOperations.ZeroMemory(finalBuffer.AsSpan(0, finalBuffer.Length));
                    ByteArrayPool.Return(finalBuffer, clearArray: true);
                }

                // GC'ye yardımcı ol
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized, blocking: false);
            }
        }

        public static string Decrypt(string base64Metin)
        {
            if (string.IsNullOrEmpty(base64Metin)) return base64Metin;

            byte[] decryptedBytes = null;
            byte[] tumVeri = null;
            byte[] dataBuffer = null;

            try
            {
                tumVeri = Convert.FromBase64String(base64Metin);

                // Minimum boyut kontrolü: IV(16) + En az 16 byte veri + HMAC(64) = 96 byte
                if (tumVeri.Length < 96)
                {
                    return "Hata: Geçersiz Veri Formatı";
                }

                using (Aes aes = Aes.Create())
                {
                    aes.KeySize = 256;
                    aes.BlockSize = 128;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.Key = MasterKey;

                    // HMAC ayırma
                    int dataLength = tumVeri.Length - 64;
                    dataBuffer = ByteArrayPool.Rent(dataLength);

                    byte[] receivedHmac = new byte[64];
                    Buffer.BlockCopy(tumVeri, 0, dataBuffer, 0, dataLength);
                    Buffer.BlockCopy(tumVeri, dataLength, receivedHmac, 0, 64);

                    // HMAC doğrulama (Encrypt-then-MAC)
                    using (var hmac = new HMACSHA512(HmacKey))
                    {
                        byte[] calculatedHmac = hmac.ComputeHash(dataBuffer, 0, dataLength);

                        // Timing attack'a karşı güvenli karşılaştırma
                        if (!CryptographicOperations.FixedTimeEquals(calculatedHmac, receivedHmac))
                        {
                            return "Hata: Veri Bütünlüğü İhlali - Veri Değiştirilmiş";
                        }

                        // HMAC'i temizle
                        CryptographicOperations.ZeroMemory(calculatedHmac);
                        CryptographicOperations.ZeroMemory(receivedHmac);
                    }

                    // IV ayırma
                    byte[] iv = new byte[16];
                    Buffer.BlockCopy(dataBuffer, 0, iv, 0, 16);

                    // Şifreli veri ayırma
                    int encryptedDataLength = dataLength - 16;
                    byte[] encryptedData = new byte[encryptedDataLength];
                    Buffer.BlockCopy(dataBuffer, 16, encryptedData, 0, encryptedDataLength);

                    aes.IV = iv;

                    using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    {
                        using (MemoryStream ms = new MemoryStream(encryptedData))
                        {
                            using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                            {
                                using (MemoryStream resultStream = new MemoryStream())
                                {
                                    cs.CopyTo(resultStream);
                                    decryptedBytes = resultStream.ToArray();

                                    string result = Encoding.Unicode.GetString(decryptedBytes);
                                    return result;
                                }
                            }
                        }
                    }
                }
            }
            catch (CryptographicException)
            {
                return "Hata: Şifre Çözme Hatası - Geçersiz Anahtar veya Bozuk Veri";
            }
            catch (FormatException)
            {
                return "Hata: Geçersiz Base64 Formatı";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Şifre çözme hatası: {ex.Message}");
                return "Hata: Beklenmeyen Bir Hata Oluştu";
            }
            finally
            {
                // Kritik: Bellekte hassas verileri temizle
                if (decryptedBytes != null)
                {
                    CryptographicOperations.ZeroMemory(decryptedBytes);
                }
                if (tumVeri != null)
                {
                    CryptographicOperations.ZeroMemory(tumVeri);
                }
                if (dataBuffer != null)
                {
                    CryptographicOperations.ZeroMemory(dataBuffer.AsSpan(0, tumVeri?.Length - 64 ?? 0));
                    ByteArrayPool.Return(dataBuffer, clearArray: true);
                }

                // GC'ye düşük öncelikli collection hint ver
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized, blocking: false);
            }
        }

        // Bellek temizleme helper metodu
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SecureClear(byte[] array)
        {
            if (array != null)
            {
                CryptographicOperations.ZeroMemory(array);
            }
        }
    }
}