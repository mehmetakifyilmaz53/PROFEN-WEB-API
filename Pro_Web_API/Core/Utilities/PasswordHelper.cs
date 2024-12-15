namespace Pro_Web_API.Core.Utilities
{
    public class PasswordHelper
    {
        public static string HashPassword(string password)
        {
        
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool VerifyPassword(string inputPassword, string storedHashedPassword)
        {
        
            return BCrypt.Net.BCrypt.Verify(inputPassword, storedHashedPassword);
        }
    }
}
