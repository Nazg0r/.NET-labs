namespace API.Extensions
{
	public static class ConfigureCors
	{
		public static void AddConfiguredCors(this IServiceCollection services, IConfiguration config)
		{
			services.AddCors(opt =>
			{
				opt.AddPolicy(config["CorsPolicy:Name"] ?? string.Empty, policy =>
				{
					policy.WithOrigins(config["Client:Url"] ?? string.Empty)
						.AllowCredentials()
						.AllowAnyHeader()
						.AllowAnyMethod();
				});
			});
		}
	}
}