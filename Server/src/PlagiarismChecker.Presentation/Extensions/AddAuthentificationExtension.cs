using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Presentation.Extensions
{
	public static class AddAuthentificationExtension
	{
		public static IServiceCollection AddCustomAuthentification(this IServiceCollection services, IConfiguration conf) 
		{
			services.AddIdentity<Student, IdentityRole>()
				.AddEntityFrameworkStores<DataContext>()
				.AddDefaultTokenProviders();

			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters()
				{
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(conf["JWT:Secret"] ?? ""))
				};
			});

			return services;
		}
	}
}
