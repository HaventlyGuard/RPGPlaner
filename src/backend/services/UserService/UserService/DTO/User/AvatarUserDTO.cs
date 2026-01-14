namespace UserService.DTO.User;

public class AvatarUserDTO
{
    public Guid Id { get; set; }
    public string? AvatarUrl { get; set; }
    public byte[]? AvatarByte { get; set; }
    public string? AvatarType { get; set; }
}