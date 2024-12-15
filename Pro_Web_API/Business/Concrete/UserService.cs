using Microsoft.IdentityModel.Tokens;
using Pro_Web_API.Business.Abstract;
using Pro_Web_API.Core.DTO;
using Pro_Web_API.Core.Entities;
using Pro_Web_API.Core.Utilities;
using Pro_Web_API.Data.Repositories.Abstract;
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

            try
            {
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
                var user = new User
                {
                    user_Name = userDto.UserName,
                    password_Hash = PasswordHelper.HashPassword(userDto.PasswordHash),
                    email = userDto.Email,
                    role = userDto.Role
                };

                await _userRepository.AddAsync(user);

                response.Success = true;
                response.Data = user;
                response.Message = "Kullanıcı başarıyla kaydedildi.";

            }
            catch (AppException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;

        }

        public async Task<ServiceResponse<List<Dictionary<string, string>>>> LoginAsync(string username, string password)
        {
            var response = new ServiceResponse<List<Dictionary<string, string>>>();
            var tokenList = new List<Dictionary<string, string>>();


            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null || !PasswordHelper.VerifyPassword(password, user.password_Hash))
            {
                response.Success = false;
                response.Message = "Geçersiz kullanıcı adı veya şifre.";
                return response;
            }


            var token = JWTToken.GenerateJwtToken(user);


            tokenList.Add(new Dictionary<string, string> { { "JWTToken", token } });

            response.Success = true;
            response.Data = tokenList;
            response.Message = "Giriş başarılı.";
            return response;
        }


        public async Task<ServiceResponse<User?>> GetUserByIdAsync(int id)
        {
            var response = new ServiceResponse<User?>();

            try
            {
                var user = await _userRepository.GetByIdAsync(id);

                if (user == null)
                {
                    response.Success = false;
                    response.Message = "Kullanıcı bulunamadı.";
                    return response;
                }
                response.Success = true;
                response.Data = user;
                response.Message = "Kullanıcı başarıyla getirildi";
            }

            catch (AppException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<List<User>>> GetAllUsersAsync()
        {
            var response = new ServiceResponse<List<User>>();
            try
            {
                response.Data = await _userRepository.GetAllAsync();
                response.Success = true;
                response.Message = "Kullanıcılar başarıyla listelendi.";
            }
            catch (AppException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<bool>> UpdateUserAsync(int id, UpdateUserDto userDto)
        {
            var response = new ServiceResponse<bool>();
            try
            {
                var existingUser = await _userRepository.GetByIdAsync(id);

                if (existingUser == null)
                {
                    response.Success = false;
                    response.Message = "Kullanıcı bulunamadı.";
                    return response;
                }

                if (userDto.Email != existingUser.email)
                {
                    var existingUserByEmail = await _userRepository.GetByEmailAsync(userDto.Email);
                    if (existingUserByEmail != null && existingUserByEmail.Id != id)
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
                    existingUser.email = userDto.Email;
                }

                if (userDto.UserName != existingUser.user_Name)
                {
                    var existingUserByUserName = await _userRepository.GetByUsernameAsync(userDto.UserName);
                    if (existingUserByUserName != null && existingUserByUserName.Id != id)
                    {
                        response.Success = false;
                        response.Message = "Kullanıcı adı zaten mevcut.";
                        return response;
                    }
                    existingUser.user_Name = userDto.UserName;
                }

                if (!string.IsNullOrEmpty(userDto.PasswordHash))
                {
                    if (!ValidationHelper.IsPasswordComplex(userDto.PasswordHash))
                    {
                        response.Success = false;
                        response.Message = "Şifreniz en az 8 karakterli olmalı, en az bir büyük harf, bir rakam ve özel karakter içermelidir.";
                        return response;
                    }
                    existingUser.password_Hash = PasswordHelper.HashPassword(userDto.PasswordHash);
                }

                if (userDto.Role != existingUser.role)
                {
                    if (!Enum.IsDefined(typeof(UserRole), userDto.Role))
                    {
                        response.Success = false;
                        response.Message = "Geçersiz rol tipi.";
                        return response;
                    }
                    existingUser.role = userDto.Role;
                }

                await _userRepository.UpdateAsync(existingUser);

                response.Success = true;
                response.Data = true;
                response.Message = "Kullanıcı başarıyla güncellendi.";
            }
            catch (AppException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<ServiceResponse<bool>> DeleteUserAsync(int id)
        {
            var response = new ServiceResponse<bool>();

            try
            {
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
            }
            catch (AppException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;

        }


    }
}
