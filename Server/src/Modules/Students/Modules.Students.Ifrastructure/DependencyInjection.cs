using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Modules.Students.Application.Contracts;
using Modules.Students.Infrastructure.Authentification;
using Modules.Students.Infrastructure.Command;
using Modules.Students.Infrastructure.Query;

namespace Modules.Students.Infrastructure
{
	public static class DependencyInjection
	{
		public static void AddStudentModuleInfrastructure(this IHostApplicationBuilder builder)
		{
			var encryptionKey = builder.Configuration["JWT:Secret"];

			builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
			builder.Services.AddScoped<IStudentAuthenticator, IdentityStudentAuthenticator>();
			builder.Services.AddScoped<IStudentCreator, StudentCreator>();
			builder.Services.AddScoped<IStudentQueries, StudentQueries>();
			builder.Services.AddScoped<IStudentCommands, StudentCommands>();

			builder.Services.AddAuthentication(options =>
				{
					options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
					options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
					options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
				})
				.AddJwtBearer(options =>
				{
					options.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuerSigningKey = true,
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(encryptionKey!)),

						ValidateIssuer = false,
						ValidateAudience = false,
						ValidateLifetime = true,
					};
				});

			builder.Services.AddAuthorization();
		}
	}
}