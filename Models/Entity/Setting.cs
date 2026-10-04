using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NhaGiaKim.Models.Entity;

[Table("settings")]
public class Setting
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity), Column("setting_id")]
    public long SettingId { get; set; }

    [Column("setting_key")] public string SettingKey { get; set; } = "";
    [Column("setting_value")] public string? SettingValue { get; set; }
    [Column("description")] public string? Description { get; set; }
    [Column("updated_at")] public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
