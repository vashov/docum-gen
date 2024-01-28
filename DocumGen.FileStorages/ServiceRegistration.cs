using DocumGen.Application.Contracts.FileStorages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DocumGen.FileStorages
{
    public static class ServiceRegistration
    {
        public static IServiceCollection TryAddFileStorages(this IServiceCollection services)
        {
            services.TryAddSingleton<LocalStorageConfiguration>();
            services.TryAddScoped<IFileStorage, LocalFileStorage>();

            return services;
        }
    }
}
