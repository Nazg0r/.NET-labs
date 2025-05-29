using MassTransit;
using Modules.Students.Application.UseCases;
using Modules.Works.Application.UseCases;

namespace API.Extensions
{
    public static class ConfigureMassTransit
    {
        public static void AddConfiguredMassTransit(this IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<GetStudentWorksRequestConsumer>();
                x.AddConsumer<WorkUploadedConsumer>();

                x.SetKebabCaseEndpointNameFormatter();
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                });
            });
        }
    }
}