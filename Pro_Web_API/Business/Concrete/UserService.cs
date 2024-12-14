using Microsoft.IdentityModel.Tokens;
using Pro_Web_API.Business.Abstract;
using Pro_Web_API.Core.DTO;
using Pro_Web_API.Core.Entities;
using Pro_Web_API.Core.Utilities;
using Pro_Web_API.Data.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Pro_Web_API.Business.Concrete
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ServiceResponse<User>> RegisterAsync(RegisterUserDto userDto)
        {
            var response = new ServiceResponse<User>();

            // Kullanıcı adının benzersiz olup olmadığını kontrol et
            var existingUser = await _userRepository.GetByUsernameAsync(userDto.UserName);
            if (existingUser != null)
            {
                response.Success = false;
                response.Message = "Kullanıcı adı zaten mevcut.";
                return response;
            }

            var existingUserByEmail = await _userRepository.GetByEmailAsync(userDto.Email);
            if (existingUserByEmail != null)
            {
                response.Success = false;
                response.Message = "E-posta adresi zaten mevcut.";
                return response;
            }

            if (!ValidationHelper.IsValidEmail(userDto.Email))
            {
                response.Success = false;
                response.Message = "Geçersiz e-posta formatı.";
                return response;
            }

            if (!ValidationHelper.IsPasswordComplex(userDto.PasswordHash))
            {
                response.Success = false;
                response.Message = "Şifreniz en az 8 karakterli olmalı, en az bir büyük harf, bir rakam ve özel karakter içermelidir.";
                return response;
            }

            if (!Enum.IsDefined(typeof(UserRole), userDto.Role))
            {
                response.Success = false;
                response.Message = "Geçersiz rol tipi.";
                return response;
            }

            // Yeni kullanıcı nesnesi oluştur
            var user = new User
            {
                user_Name = userDto.UserName,
                password_Hash = HashPassword(userDto.PasswordHash),
                email = userDto.Email,
                role = userDto.Role
            };

            // Yeni kullanıcıyı veritabanına ekle
            await _userRepository.AddAsync(user);

            response.Success = true;
            response.Data = user;
            response.Message = "Kullanıcı başarıyla kaydedildi.";
            return response;
        }

        public async Task<ServiceResponse<string>> LoginAsync(string username, string password)
        {
            var response = new ServiceResponse<string>();

            // Kullanıcı doğrulama
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null || !VerifyPassword(password, user.password_Hash))
            {
                response.Success = false;
                response.Message = "Geçersiz kullanıcı adı veya şifre.";
                return response;
            }

            // JWT token üretme (Örnek bir işlem)
            var token = GenerateJwtToken(user);

            response.Success = true;
            response.Data = token;
            response.Message = "Giriş başarılı.";
            return response;
        }

        public async Task<ServiceResponse<User?>> GetUserByIdAsync(int id)
        {
            var response = new ServiceResponse<User?>();
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                response.Success = false;
                response.Message = "Kullanıcı bulunamadı.";
                return response;
            }

            response.Success = true;
            response.Data = user;
            return response;
        }

        public async Task<ServiceResponse<List<User>>> GetAllUsersAsync()
        {
            var response = new ServiceResponse<List<User>>();
            var users = await _userRepository.GetAllAsync();

            response.Success = true;
            response.Data = users;
            return response;
        }

        public async Task<ServiceResponse<bool>> UpdateUserAsync(User user)
        {
            var response = new ServiceResponse<bool>();
            var existingUser = await _userRepository.GetByIdAsync(user.Id);

            if (existingUser == null)
            {
                response.Success = false;
                response.Message = "Kullanıcı bulunamadı.";
                return response;
            }

            existingUser.email = user.email;
            existingUser.user_Name = user.user_Name;
            existingUser.role = user.role;

            await _userRepository.UpdateAsync(existingUser);

            response.Success = true;
            response.Data = true;
            response.Message = "Kullanıcı başarıyla güncellendi.";
            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteUserAsync(int id)
        {
            var response = new ServiceResponse<bool>();
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                response.Success = false;
                response.Message = "Kullanıcı bulunamadı.";
                return response;
            }

            await _userRepository.DeleteAsync(user);

            response.Success = true;
            response.Data = true;
            response.Message = "Kullanıcı başarıyla silindi.";
            return response;
        }

        //private string HashPassword(string password)
        //{
        //    using var sha256 = SHA256.Create();
        //    var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        //    return Convert.ToBase64String(hashedBytes);
        //}

        private string HashPassword(string password)
        {
            // Örnek olarak BCrypt kullanımı
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string inputPassword, string storedHashedPassword)
        {
            // Hashlenmiş şifreyi doğrula
            return BCrypt.Net.BCrypt.Verify(inputPassword, storedHashedPassword);
        }


        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("b1f6c3f8e2d94b5eaef5ac39484c9476")); // Rastgele GUID

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Kullanıcı bilgileri ve rollerini içeren claims
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.email),
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Role, user.role.ToString()) // Kullanıcının rolü
    };

            // Token yapılandırması
            var token = new JwtSecurityToken(
                issuer: "localhost",               // Issuer
                audience: "localhost",             // Audience
                claims: claims,
                expires: DateTime.Now.AddHours(1),   // Token geçerlilik süresi
                signingCredentials: creds
            );

            // Token string'e dönüştürülür
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
