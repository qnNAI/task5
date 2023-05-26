using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models.Common {

    public class GetUsersArgs {

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public int Seed { get; set; }
        public string Locale { get; set; } = null!;
    }
}
