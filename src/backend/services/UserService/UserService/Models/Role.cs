// using System.ComponentModel.DataAnnotations;
// using System.ComponentModel.DataAnnotations.Schema;
//
// namespace UserService.Models;
//
// [Table("roles")]
// public class Role
// {
//     [Key]
//     [Column("id")]
//     public Guid Id { get; set; }
//
//     [Required]
//     [Column("name")]
//     [MaxLength(50)]
//     public string Name { get; set; } = string.Empty;
//
//     [Column("description")]
//     [MaxLength(500)]
//     public string? Description { get; set; }
//
//     // Просто массив ID разрешений
//     [Column("permission_ids", TypeName = "uuid[]")]
//     public List<Guid> PermissionIds { get; set; } = new();
//
//     // Стандартные роли
//     public static readonly Guid AdminRoleId = Guid.Parse("00000000-0000-0000-0000-000000000001");
//     public static readonly Guid UserRoleId = Guid.Parse("00000000-0000-0000-0000-000000000002");
//     public static readonly Guid GuestRoleId = Guid.Parse("00000000-0000-0000-0000-000000000003");
// }