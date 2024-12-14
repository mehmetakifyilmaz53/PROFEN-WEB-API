namespace Pro_Web_API.Core.Utilities
{
    public class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            // Örnek olarak BCrypt kullanımı
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool VerifyPassword(string inputPassword, string storedHashedPassword)
        {
            // Hashlenmiş şifreyi doğrula
            return BCrypt.Net.BCrypt.Verify(inputPassword, storedHashedPassword);
        }
    }
}
