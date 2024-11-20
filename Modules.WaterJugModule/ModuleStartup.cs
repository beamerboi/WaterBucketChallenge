using Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Modules.WaterJugModule.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.WaterJugModule
{
    public static class ModuleStartup
    {
        public static IServiceCollection AddWaterJugModule(this IServiceCollection services)
        {
            services.AddScoped<IWaterJugService, WaterJugService>();
            return services;
        }
    }
}
