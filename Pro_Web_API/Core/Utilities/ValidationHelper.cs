using System.Text.RegularExpressions;

namespace Pro_Web_API.Core.Utilities
{
    public class ValidationHelper
    {
        public static bool IsValidEmail(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return false;

                var emailRegex = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.(com|org|net|edu|gov|io)$";
                return Regex.IsMatch(email, emailRegex);
            }
            catch
            {
                return false;
            }
        }

        public static bool IsPasswordComplex(string password)
        {
            return password.Length >= 8 &&
                   password.Any(char.IsUpper) &&
                   password.Any(char.IsLower) &&
                   password.Any(char.IsDigit) &&
                   password.Any(ch => "!@#$%^&*()_+[]{}|;:,.<>?".Contains(ch));
        }
    }
}
