using DocumGen.Application.Contracts.Persistence;
using DocumGen.Persistence.InMemoryDb.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DocumGen.Persistence.InMemoryDb
{
    public static class ServiceRegistraction
    {
        public static IServiceCollection TryAddInMemoryRepositories(this IServiceCollection services)
        {
            // Use Singleton for InMemory Repositories
            services.TryAddSingleton<IFileOrderRepository, FileOrderInMemoryRepository>();

            return services;
        }
    }
}
