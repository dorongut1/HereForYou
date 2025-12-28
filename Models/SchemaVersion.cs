using SQLite;

namespace HereForYou.Models;

[Table("schema_version")]
public class SchemaVersion
{
    [PrimaryKey]
    [Column("version")]
    public int Version { get; set; }

    [Column("applied_at")]
    public DateTime AppliedAt { get; set; }

    [Column("description")]
    public string? Description { get; set; }
}
