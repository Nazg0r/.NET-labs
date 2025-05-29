using System.ComponentModel.DataAnnotations.Schema;

namespace Modules.Works.Domain.Entities
{
    public class UploadFIleJobResult
    {
        public Guid Id { get; set; } = Guid.Empty;
        [Column("job_id")] public string? JobId { get; set; } = string.Empty;
        [Column("work_id")] public Guid WorkId { get; set; } = Guid.Empty;
    }
}