namespace Modules.Works.Application.Common.Models
{
    public class ProcessedWorkResponse
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = null!;
        public DateTime LoadDate { get; set; }
        public byte[] Content { get; set; } = null!;
        public string StudentId { get; set; } = null!;
    }
}