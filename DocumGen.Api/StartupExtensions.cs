using DocumGen.Api.Common.ExceptionHandling;
using DocumGen.Api.Services;
using DocumGen.Api.Workers;
using DocumGen.Application;
using DocumGen.FileStorages;
using DocumGen.MessageBus.RabbitMq;
using DocumGen.Persistence.InMemoryDb;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DocumGen.Api
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            services.TryAddInMemoryRepositories();
            services.TryAddRabbitConsumer();
            services.TryAddRabbitPublisher();
            services.TryAddFileStorages();

            services.TryAddFileOrderService();

            services.AddHostedService<FileConvertedWorker>();

            return services;
        }

        public static IServiceCollection AddAppCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("Open", builder 
                    => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            });

            return services;
        }

        public static IServiceCollection AddAppHealthCheck(this IServiceCollection services)
        {
            services.AddHealthChecks().AddCheck<HealthCheck>("Main");
            return services;
        }

        public static IServiceCollection AddAppExceptionHandler(this IServiceCollection services)
        {
            services.TryAddScoped<AppExceptionHandler>();
            services.TryAddScoped<AppExceptionMiddleware>();
            return services;
        }

        public static IApplicationBuilder UseAppExceptionHandler(this IApplicationBuilder app)
        {
            app.UseMiddleware<AppExceptionMiddleware>();
            return app;
        }
    }
}
