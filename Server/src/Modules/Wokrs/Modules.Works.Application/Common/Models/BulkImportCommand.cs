using BuildingBlocks.Contracts;

namespace Modules.Works.Application.Common.Models
{
    public class BulkImportCommand : ICommand
    {
        public Guid Id { get; } = Guid.Empty!;
        public Stream FileStream { get; set; } = null!;
        public string FileName { get; init; } = null!;
    }
}