using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Contracts.Services {

    public interface IFullNameFormatService {

        string GetFullNameFormatted(string firstName, string middleName, string lastName, string locale);
    }
}
