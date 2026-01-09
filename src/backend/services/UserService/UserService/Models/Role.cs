using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserService.Models;

[Table("roles")]
public class Role
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public Guid Id { get; set; }

    [Required]
    [Column("name")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Column("description")]
    [MaxLength(500)]
    public string? Description { get; set; }

    [Column("permissions", TypeName = "jsonb")]
    public List<Permission> Permissions { get; set; } = new();

    [Column("parent_role_id")]
    public Guid? ParentRoleId { get; set; }

    public virtual Role? ParentRole { get; set; }

    [Column("inherited_role_ids", TypeName = "uuid[]")]
    public List<Guid> InheritedRoleIds { get; set; } = new();

    [Column("is_system")]
    public bool IsSystem { get; set; } = false;

    [Column("is_default")]
    public bool IsDefault { get; set; } = false;

    [Column("priority")]
    public int Priority { get; set; } = 0;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("created_by")]
    public Guid CreatedBy { get; set; }

    public virtual ICollection<Role> ChildRoles { get; set; } = new List<Role>();
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}