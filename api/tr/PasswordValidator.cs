using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace api.tr
{
    /// <summary>
    /// Şifre validasyonu için yardımcı sınıf
    /// </summary>
    public class PasswordValidator
    {
        private const int MinimumLength = 8;
        private const int MaximumLength = 128;

        /// <summary>
        /// Şifre geçerliliğini kontrol eder ve hata mesajını döndürür
        /// </summary>
        public static PasswordValidationResult ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return new PasswordValidationResult(false, "Şifre alanını boş geçemezsiniz.");
            }

            if (password.Length < MinimumLength)
            {
                return new PasswordValidationResult(false,
                    $"Şifre en az {MinimumLength} karakter olmalıdır.");
            }

            if (password.Length > MaximumLength)
            {
                return new PasswordValidationResult(false,
                    $"Şifre maksimum {MaximumLength} karakter olmalıdır.");
            }

            if (!Regex.IsMatch(password, @"[a-z]"))
            {
                return new PasswordValidationResult(false,
                    "Şifre en az bir küçük harf içermelidir.");
            }

            if (!Regex.IsMatch(password, @"[A-Z]"))
            {
                return new PasswordValidationResult(false,
                    "Şifre en az bir büyük harf içermelidir.");
            }

            if (!Regex.IsMatch(password, @"[0-9]"))
            {
                return new PasswordValidationResult(false,
                    "Şifre en az bir sayı içermelidir.");
            }

            if (!Regex.IsMatch(password, @"[!@#$%^&*()_+\-=\[\]{};':""\|,.<>\/?]"))
            {
                return new PasswordValidationResult(false,
                    "Şifre en az bir özel karakter içermelidir (!@#$%^&* vb.)");
            }

            // Ardışık aynı karakterleri kontrol et
            if (HasConsecutiveCharacters(password))
            {
                return new PasswordValidationResult(false,
                    "Şifre 3 veya daha fazla ardışık aynı karakteri içeremez.");
            }

            // Yaygın zayıf şifreler
            if (IsCommonPassword(password))
            {
                return new PasswordValidationResult(false,
                    "Bu şifre çok yaygın kullanılmaktadır. Lütfen daha güçlü bir şifre seçiniz.");
            }

            return new PasswordValidationResult(true, "Şifre güçlü ve uyumlıdır.");
        }

        /// <summary>
        /// Ardışık aynı karakterleri kontrol eder
        /// </summary>
        private static bool HasConsecutiveCharacters(string password)
        {
            for (int i = 0; i < password.Length - 2; i++)
            {
                if (password[i] == password[i + 1] && password[i + 1] == password[i + 2])
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Yaygın zayıf şifreleri kontrol eder
        /// </summary>
        private static bool IsCommonPassword(string password)
        {
            string[] commonPasswords = new[]
            {
                "password", "123456", "12345678", "qwerty", "abc123",
                "monkey", "letmein", "dragon", "baseball", "iloveyou"
            };

            foreach (var common in commonPasswords)
            {
                if (password.Equals(common, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Şifre güç seviyesini hesaplar (0-100)
        /// </summary>
        public static int CalculatePasswordStrength(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return 0;

            int score = 0;

            // Uzunluk skoru
            score += Math.Min(password.Length * 5, 20);

            // Küçük harf
            if (Regex.IsMatch(password, @"[a-z]"))
                score += 10;

            // Büyük harf
            if (Regex.IsMatch(password, @"[A-Z]"))
                score += 10;

            // Sayı
            if (Regex.IsMatch(password, @"[0-9]"))
                score += 20;

            // Özel karakterler
            if (Regex.IsMatch(password, @"[!@#$%^&*()_+\-=\[\]{};':""\|,.<>\/?]"))
                score += 30;

            // Sayı ve harf karışımı
            if (Regex.IsMatch(password, @"[a-zA-Z]") && Regex.IsMatch(password, @"[0-9]"))
                score += 10;

            return Math.Min(score, 100);
        }

        /// <summary>
        /// Şifre gücü seviyesini metin olarak döndürür
        /// </summary>
        public static string GetPasswordStrengthLabel(int score)
        {
            if (score < 30)
                return "Zayıf";
            else if (score < 60)
                return "Orta";
            else if (score < 80)
                return "İyi";
            else
                return "Çok Güçlü";
        }
    }

    /// <summary>
    /// Şifre validasyonu sonucunu temsil eder
    /// </summary>
    public class PasswordValidationResult
    {
        public bool IsValid { get; }
        public string Message { get; }

        public PasswordValidationResult(bool isValid, string message)
        {
            IsValid = isValid;
            Message = message;
        }
    }

}
