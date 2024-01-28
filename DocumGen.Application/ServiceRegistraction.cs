using DocumGen.Application.Services.Configuration;
using DocumGen.Application.Services.FileConverters;
using DocumGen.Application.Services.FileOrders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DocumGen.Application
{
    public static class ServiceRegistraction
    {
        public static IServiceCollection TryAddFileOrderService(this IServiceCollection services)
        {
            services.TryAddScoped<IFileOrderService, FileOrderService>();
            return services;
        }

        public static IServiceCollection TryAddFileConverter(this IServiceCollection services)
        {
            services.TryAddScoped<IFileConverterConfiguration, FileConverterConfiguration>();
            services.TryAddScoped<IFileConverter, FileConverter>();

            return services;
        }
    }
}
