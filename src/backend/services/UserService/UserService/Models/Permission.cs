// using System.ComponentModel.DataAnnotations;
// using System.ComponentModel.DataAnnotations.Schema;
//
// namespace UserService.Models;
//
// [Table("permissions")]
// public class Permission
// {
//     [Key]
//     [Column("id")]
//     public Guid Id { get; set; } = Guid.NewGuid();
//     
//     [Required]
//     [Column("name")]
//     [MaxLength(100)]
//     public string Name { get; set; } = string.Empty;
//
//     [Column("description")]
//     [MaxLength(500)]
//     public string? Description { get; set; }
// }