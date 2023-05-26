using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using Application.Common.Mappings;
using Application.Common.Contracts.Services;
using Application.Services;

namespace Application {

    public static class DependencyInjection {

        public static IServiceCollection AddApplication(this IServiceCollection services) {
            var config = TypeAdapterConfig.GlobalSettings;
            MappingProfile.ApplyMappings();

            services.AddSingleton(config);
            services.AddScoped<IMapper, Mapper>();

            services.AddScoped<IUserGenerationService, UserGenerationService>();

            return services;
        }
    }
}
