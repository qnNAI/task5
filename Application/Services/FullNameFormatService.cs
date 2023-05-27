using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Common.Contracts.Services;

namespace Application.Services {

    internal class FullNameFormatService : IFullNameFormatService {

        public string GetFullNameFormatted(string firstName, string middleName, string lastName, string locale) => locale switch {
            "ru-RU" => $"{lastName} {firstName} {middleName}",
            _ => string.IsNullOrEmpty(middleName) ? $"{firstName} {lastName}" : $"{firstName} {middleName} {lastName}"
        };
    }
}
