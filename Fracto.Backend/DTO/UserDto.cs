namespace Fracto.Backend.DTO
{
    public class UserDto
    {
        public int id { get; set; }
        public string username { get; set; } = string.Empty;
        public string role { get; set; } = "User"; // Default role is "User"
        public string? profileImagePath { get; set; } // Nullable to allow users without a profile image
    }
}