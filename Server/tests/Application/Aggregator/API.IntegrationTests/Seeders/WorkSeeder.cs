using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modules.Students.Domain.Entities;
using Modules.Works.Domain.Entities;
using Modules.Works.Persistence.Data;
using TestsTools;

namespace API.IntegrationTests.Seeders
{
    public static class WorkSeeder
    {
        public static async Task PrepareWorkAsync(WebApplicationFixture fixture, Work work, Student? student = null)
        {
            using var scope = fixture.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<WorkDbContext>();
            await dbContext.Works.AddAsync(work);
            await dbContext.SaveChangesAsync();
        }

        public static async Task PrepareWorksAsync(WebApplicationFixture fixture, List<Work> works, Student? student = null)
        {
            using var scope = fixture.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<WorkDbContext>();
            foreach (var work in works)
                await dbContext.Works.AddAsync(work);
            await dbContext.SaveChangesAsync();
        }

        public static async Task RemoveWorksAsync(WebApplicationFixture fixture)
        {
            using var scope = fixture.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<WorkDbContext>();
            var works = await dbContext.Works.ToListAsync();
            dbContext.Works.RemoveRange(works);
            await dbContext.SaveChangesAsync();
        }
    }
}