using FluentValidation;
using Microsoft.Extensions.Hosting;
using Modules.Students.Application.UseCases.GetStudentByUsername;
using Modules.Students.Application.UseCases.GetStudentByWorkId;
using Modules.Students.Application.UseCases.Login;
using Modules.Students.Application.UseCases.Register;

namespace Modules.Students.Application
{
    public static class DependencyInjection
    {
        public static void AddStudentModuleApplication(this IHostApplicationBuilder builder)
        {
            var encryptionKey = builder.Configuration["JWT:Secret"];

            builder.Services.AddGetStudentByUsername()
                .AddGetStudentByWorkId()
                .AddRegisterStudent()
                .AddLoginStudent();

            builder.Services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        }
    }
}