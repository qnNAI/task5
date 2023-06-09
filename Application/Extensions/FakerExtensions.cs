﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Bogus.DataSets.Name;

namespace Application.Extensions {
    public static class FakerExtensions {

        public static string GetFakerLocale(this string locale) => locale switch {
            "ru-RU" => "ru",
            "pl-PL" => "pl",
            "en-US" => "en_US",
            _ => string.Empty
        };

        public static string GetGenderPrefix(this Gender gender) => gender switch {
            Gender.Male => "M",
            Gender.Female => "F",
            _ => "M"
        };
    }
}
