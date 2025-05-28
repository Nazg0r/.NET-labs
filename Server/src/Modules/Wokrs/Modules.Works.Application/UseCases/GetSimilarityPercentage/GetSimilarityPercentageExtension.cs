using Microsoft.Extensions.DependencyInjection;

namespace Modules.Works.Application.UseCases.GetSimilarityPercentage
{
    public static class GetSimilarityPercentageExtension
    {
        public static IServiceCollection AddGetSimilarityPercentage(this IServiceCollection services)
        {
            services.AddScoped<GetSimilarityPercentageHandler>();
            return services;
        }
    }
}