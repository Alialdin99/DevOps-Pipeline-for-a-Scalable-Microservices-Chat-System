namespace UserService.DTOs
{
    public record UserProfileDto(string Id, string AuthUserId, string Username, string Email, DateTime CreatedAt);

    public record CreateUserDto(string AuthUserId, string Username, string Email);
    public record UpdateUserDto(
       string Username,
       string Email
   );
}
