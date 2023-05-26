using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations {
    public class MiddleNameConfiguration : IEntityTypeConfiguration<MiddleName> {

        public void Configure(EntityTypeBuilder<MiddleName> builder) {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Locale).IsRequired();
        }
    }
}
