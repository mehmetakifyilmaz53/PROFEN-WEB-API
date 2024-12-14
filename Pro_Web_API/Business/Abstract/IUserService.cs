using Pro_Web_API.Business.Concrete;
using Pro_Web_API.Core.DTO;
using Pro_Web_API.Core.Entities;

namespace Pro_Web_API.Business.Abstract
{
    public interface IUserService
    {
        Task<ServiceResponse<User>> RegisterAsync(RegisterUserDto userDto);
        Task<ServiceResponse<string>> LoginAsync(string username, string password);
        Task<ServiceResponse<User?>> GetUserByIdAsync(int id);
        Task<ServiceResponse<List<User>>> GetAllUsersAsync();
        Task<ServiceResponse<bool>> UpdateUserAsync(User user);
        Task<ServiceResponse<bool>> DeleteUserAsync(int id);
    }
}
