using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pro_Web_API.Business.Abstract;
using Pro_Web_API.Core.DTO;
using Pro_Web_API.Core.Entities;

namespace Pro_Web_API.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto userDto)
        {
            var response = await _userService.RegisterAsync(userDto);
            if (!response.Success)
            {
                // Özel hata kodlarına göre yanıt döndür
                return NotFound(new
                {
                    success = false,
                    message = response.Message
                });
            }

            return Ok(new
            {
                success = true,
                data = response.Data,
                message = response.Message
            });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginUserDto userDto)
        {
            var response = await _userService.LoginAsync(userDto.UserName, userDto.Password);
            if (!response.Success)
            {
                return Unauthorized(new
                {
                    success = false,
                    message = response.Message
                });
            }

            return Ok(new
            {
                success = true,
                data = response.Data,
                message = response.Message
            });
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var response = await _userService.GetUserByIdAsync(id);
            if (!response.Success)
            {
                return NotFound(new
                {
                    success = false,
                    message = response.Message
                });
            }

            return Ok(new
            {
                success = true,
                data = response.Data,
                message = response.Message
            });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var response = await _userService.GetAllUsersAsync();
            return Ok(new
            {
                success = true,
                data = response.Data,
                message = "Kullanıcılar başarıyla alındı."
            });
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {
            if (id != user.Id)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Kullanıcı ID eşleşmiyor."
                });
            }

            var response = await _userService.UpdateUserAsync(user);
            if (!response.Success)
            {
                return NotFound(new
                {
                    success = false,
                    message = response.Message
                });
            }

            return Ok(new
            {
                success = true,
                data = response.Data,
                message = "Kullanıcı başarıyla güncellendi."
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var response = await _userService.DeleteUserAsync(id);
            if (!response.Success)
            {
                return NotFound(new
                {
                    success = false,
                    message = response.Message
                });
            }

            return Ok(new
            {
                success = true,
                message = "Kullanıcı başarıyla silindi."
            });
        }
    }
}
