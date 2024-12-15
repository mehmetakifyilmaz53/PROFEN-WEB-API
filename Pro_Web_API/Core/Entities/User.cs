namespace Pro_Web_API.Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string user_Name { get; set; }
        public string password_Hash { get; set; } 
        public string email { get; set; }
        public UserRole role { get; set; }
    }

    public enum UserRole
    {
        Admin,
        Manager,
        Viewer
    }
}
