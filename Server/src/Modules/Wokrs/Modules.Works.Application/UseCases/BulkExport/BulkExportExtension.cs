using Microsoft.Extensions.DependencyInjection;

namespace Modules.Works.Application.UseCases.BulkExport
{
    public static class BulkExportExtension
    {
        public static IServiceCollection AddBulkExport(this IServiceCollection services)
        {
            services.AddScoped<BulkExportHandler>();
            return services;
        }
    }
}