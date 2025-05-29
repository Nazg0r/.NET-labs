using Microsoft.Extensions.DependencyInjection;

namespace Modules.Works.Application.UseCases.GetAllWorks
{
    public static class GetAllWorksExtension
    {
        public static IServiceCollection AddGetAllWorks(this IServiceCollection services)
        {
            services.AddScoped<GetAllWorksHandler>();
            return services;
        }
    }
}