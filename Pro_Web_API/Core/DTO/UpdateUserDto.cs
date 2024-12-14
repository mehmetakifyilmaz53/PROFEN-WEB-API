using Pro_Web_API.Core.Entities;

namespace Pro_Web_API.Core.DTO
{
    public class UpdateUserDto
    {
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public UserRole Role { get; set; }
    }
}
