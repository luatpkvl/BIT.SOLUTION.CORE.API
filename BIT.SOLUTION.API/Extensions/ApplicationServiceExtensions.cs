using BIT.SOLUTION.DbHepper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BIT.SOLUTION.DL;
using BIT.SOLUTION.SERVICE;

namespace BIT.SOLUTION.API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IDLBase, DLBase>();
            services.AddScoped<IDLCRMBase, DLCRMBase>();
            services.AddScoped<IDLCRMBase, DLCRMBase>();
            services.AddScoped<IServiceBase, ServiceBase>();
            services.AddScoped<IServiceBitBase, ServiceBitBase>();
            services.AddScoped<IDLAcount, DLAcount>();
            services.AddScoped<IAcountService, AcountService>();
            return services;
        }
    }
}
