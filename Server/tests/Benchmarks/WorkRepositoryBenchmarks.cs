using BenchmarkDotNet.Attributes;
using Bogus;
using MassTransit.Internals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Modules.Works.Domain.Entities;
using Modules.Works.Persistence.Data;
using Testcontainers.PostgreSql;

namespace Benchmarks
{
    [MemoryDiagnoser]
    public class WorkRepositoryBenchmarks
    {
        private PostgreSqlContainer _dbContainer = null!;
        private WorkDbContext _context = null!;
        private Guid _workId = Guid.Empty;
        private string _studentId = string.Empty;

        [GlobalSetup]
        public async Task Setup()
        {
            var dbContainer = new PostgreSqlBuilder()
                .WithImage("postgres")
                .WithDatabase("plagiarism_checker_test")
                .WithUsername("postgres")
                .WithPassword("postgres")
                .Build();

            await dbContainer.StartAsync();
            _dbContainer = dbContainer;

            _context = new WorkDbContext(new DbContextOptionsBuilder<WorkDbContext>()
                .UseNpgsql(_dbContainer.GetConnectionString())
                .ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.DetachedLazyLoadingWarning))
                .Options);

            await _context.Database.EnsureCreatedAsync();

            var fakeWorks = GenerateFakeWorks(1000);
            _workId = fakeWorks[new Random().Next(1000)].Id;
            _studentId = fakeWorks[new Random().Next(6)].StudentId;

            _context.Works.AddRange(GenerateFakeWorks(1000));
        }

        [Benchmark]
        public async Task<Work?> GetWorkByIdAsync() =>
            await _context.Works.FindAsync(_workId);

        [Benchmark]
        public Task<Work?> GetWorkByIdAsyncCompiled() =>
            GetWorkByIdAsyncComp(_context, _workId);

        [Benchmark]
        public async Task<List<Work>> GetAllWorksAsync() =>
            await _context.Works.AsNoTracking().ToListAsync();

        [Benchmark]
        public List<Work> GetAllWorksCompiled() =>
            GetAllWorkComp(_context);

        [Benchmark]
        public async Task<List<Work>> GetWorksByStudentIdAsync() =>
            await _context.Works.AsNoTracking().Where(w => w.StudentId == _studentId).ToListAsync();

        [Benchmark]
        public async Task<List<Work>> GetWorksByStudentIdCompiled() =>
            (List<Work>)await CompiledGetWorksByStudentIdAsync(_context, _studentId).ToListAsync();

        [GlobalCleanup]
        public async Task Cleanup()
        {
            await _dbContainer.DisposeAsync();
        }

        private List<Work> GenerateFakeWorks(int count)
        {
            List<string> extensions = [".pdf", ".docx", ".txt", ".md", ".cs"];
            List<string> studentIds =
            [
                Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString()
            ];

            var faker = new Faker<Work>()
                .RuleFor(w => w.Id, f => Guid.NewGuid())
                .RuleFor(w => w.StudentId, f => f.PickRandom(studentIds))
                .RuleFor(w => w.FileName, f => f.Lorem.Word())
                .RuleFor(w => w.Content, f => f.Random.Bytes(f.Random.Int(1000, 5000)))
                .RuleFor(w => w.LoadDate, f => f.Date.Past(1))
                .RuleFor(w => w.Extension, f => f.PickRandom(extensions));

            return faker.Generate(count);
        }

        private static Func<WorkDbContext, Guid, Task<Work?>> GetWorkByIdAsyncComp =>
            EF.CompileAsyncQuery((WorkDbContext context, Guid id) =>
                context.Works.AsNoTracking().FirstOrDefault(w => w.Id == id));

        private static Func<WorkDbContext, List<Work>> GetAllWorkComp =>
            EF.CompileQuery((WorkDbContext context) =>
                context.Works.AsNoTracking().ToList());

        private static readonly Func<WorkDbContext, string, IAsyncEnumerable<Work>> CompiledGetWorksByStudentIdAsync =
            EF.CompileAsyncQuery((WorkDbContext context, string studentId) =>
                context.Works.AsNoTracking().Where(w => w.StudentId == studentId));
    }
}