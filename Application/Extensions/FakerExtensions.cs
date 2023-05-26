using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Extensions {
    public static class FakerExtensions {

        private static readonly Dictionary<string, string> _fakerLocalesMap = new() {
            { "ru-RU", "ru" },
            { "pl-PL", "pl" },
            { "en-US",  "en_US" }
        };

        public static string GetFakerLocale(this string locale) => _fakerLocalesMap.ContainsKey(locale) ? _fakerLocalesMap[locale] : string.Empty;
    }
}
