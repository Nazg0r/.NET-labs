using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Modules.Works.Application.Contracts;
using Modules.Works.Infrastructure.CsvProcessing;
using Modules.Works.Infrastructure.Jobs;

namespace Modules.Works.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddWorksModuleInfrastructure(this IHostApplicationBuilder builder)
        {
            builder.Services.AddScoped<ICsvParser, CsvParser>();
            builder.Services.AddScoped<ImportDataJob>();
            builder.Services.AddScoped<UploadFileJob>();
        }
    }
}