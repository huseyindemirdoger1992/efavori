using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace api
{
    public class TextualFunctions
    {
        public string FirstLastLetter(string metin)
        {
            // Kullanıcının adının veya soyadının sadece ilk ve son harfini gösterir, aradaki harfleri '*' ile gizler.
            metin = metin.Trim();
            if (metin.Length <= 2) return metin;
            return metin[0] + new string('*', metin.Length - 2) + metin[metin.Length - 1];
        }
        public string NormalizePhoneNumberEditor(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // 1️⃣ Rakam dışındaki her şeyi sil
            string number = Regex.Replace(input, @"\D", "");

            if (string.IsNullOrEmpty(number))
                return string.Empty;

            // 2️⃣ Uluslararası çıkış prefixlerini temizle (00, 000, 0000...)
            number = Regex.Replace(number, @"^0{2,}", "");

            // 3️⃣ Ulusal trunk prefix (başta anlamsız 0'lar)
            number = number.TrimStart('0');

            // 4️⃣ Mantıksız uzunlukları ele (opsiyonel ama prod'da şart)
            if (number.Length < 6 || number.Length > 15)
                return string.Empty;

            return number;
        }

    }
}
