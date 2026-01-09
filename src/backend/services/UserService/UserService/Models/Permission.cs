using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserService.Models;

[Table("permissions")]
public class Permission
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid(); // ГУИД!
    
    [Column("code")]
    public string Code { get; set; } = string.Empty;

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("description")]
    public string? Description { get; set; }

    [Column("resource_type")]
    public string ResourceType { get; set; } = string.Empty;

    [Column("resource_id")]
    public string? ResourceId { get; set; }

    [Column("level")]
    public AccessLevel Level { get; set; } = AccessLevel.Read;

    [Column("constraints")]
    public Dictionary<string, object>? Constraints { get; set; }

    [Column("is_inheritable")]
    public bool IsInheritable { get; set; } = true;

    [Column("implied_permissions")]
    public List<string> ImpliedPermissions { get; set; } = new();
}

public enum AccessLevel
{
    None = 0,
    Read = 1,
    Comment = 2,
    Write = 3,
    Manage = 4,
    Owner = 5
}