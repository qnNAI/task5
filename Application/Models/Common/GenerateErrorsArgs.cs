using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Models.User;

namespace Application.Models.Common {

    public class GenerateErrorsArgs {

        public IEnumerable<UserDto> Users { get; set; } = Enumerable.Empty<UserDto>();
        public int Seed { get; set; }
        public double ErrorProbability { get; set; }
        public string Locale { get; set; } = null!;
    }
}
