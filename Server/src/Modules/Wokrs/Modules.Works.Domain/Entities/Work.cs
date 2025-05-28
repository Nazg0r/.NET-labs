using System.ComponentModel.DataAnnotations.Schema;

namespace Modules.Works.Domain.Entities
{
    public class Work
    {
        [Column("id")] public Guid Id { get; set; }
        [Column("file_name")] public string FileName { get; set; } = null!;
        [Column("load_date")] public DateTime LoadDate { get; set; }
        [Column("extension")] public string Extension { get; set; } = null!;
        [Column("content")] public byte[] Content { get; set; } = null!;
        [Column("student_id")] public string StudentId { get; set; } = null!;
    }
}