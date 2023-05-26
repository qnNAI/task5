using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Contracts.Contexts {

    public interface IApplicationDbContext {

        DbSet<MiddleName> MiddleNames { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
