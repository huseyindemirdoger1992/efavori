using System;
using System.Collections.Generic;
using System.Text;

namespace api
{
    public class TextualFunctions
    {
        public string FirstLastLetter(string metin)
        {
            metin = metin.Trim();
            if (metin.Length <= 2) return metin;
            return metin[0] + new string('*', metin.Length - 2) + metin[metin.Length - 1];
        }
    }
}
