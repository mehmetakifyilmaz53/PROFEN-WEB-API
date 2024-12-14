using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pro_Web_API.Business.Abstract;
using Pro_Web_API.Business.Concrete;
using Pro_Web_API.Core.DTO;
using Pro_Web_API.Core.Entities;
using Swashbuckle.AspNetCore.Annotations;

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

        /// <summary>
        /// Kullanıcı kayıt işlemi.
        /// </summary>
        /// <param name="userDto">Kullanıcı kayıt bilgileri.</param>
        /// <returns>Başarılı kayıt sonucu veya hata mesajı döndürür.</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        [SwaggerResponse(200, "Kullanıcı başarıyla kaydedildi.", typeof(ServiceResponse<User>))]
        [SwaggerResponse(400, "Geçersiz veri gönderildi.")]
        [SwaggerResponse(500, "Sunucu hatası.")]
        [SwaggerResponse(401, "Kimlik doğrulama başarısız.")]
        [SwaggerResponse(403, "Bu işlem için yeterli yetkiye sahip değilsiniz.")]
        [SwaggerResponse(404, "Kayıt bulunamadı.")]
        [SwaggerResponse(429, "Çok fazla istek.")]
        [SwaggerResponse(503, "Hizmet kullanılamıyor. Sunucu geçici olarak kullanılamıyor.")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto userDto)
        {
            if (userDto.Role != UserRole.Viewer && (!User.Identity.IsAuthenticated || !User.IsInRole("Admin")))
            {
                return Forbid();
            }
            var response = await _userService.RegisterAsync(userDto);
            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }
        /// <summary>
        /// Kullanıcı giriş işlemi.
        /// </summary>
        /// <param name="userDto">Kullanıcı giriş bilgileri.</param>
        /// <returns>Başarılı giriş sonucu kullanıcı bilgileri ve JWT Token döndürür.</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        [SwaggerResponse(200, "Kullanıcı başarıyla giriş yapabildi.", typeof(ServiceResponse<string>))]
        [SwaggerResponse(400, "Geçersiz veri gönderildi.")]
        [SwaggerResponse(429, "Çok fazla istek.")]
        [SwaggerResponse(500, "Sunucu hatası.")]
        [SwaggerResponse(503, "Hizmet kullanılamıyor. Sunucu geçici olarak kullanılamıyor.")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto userDto)
        {
            var response = await _userService.LoginAsync(userDto.UserName, userDto.Password);
            if (!response.Success)
            {
                return Unauthorized(response);
            }

            return Ok(response);
        }
        /// <summary>
        /// ID'ye göre Kullanıcı getirme işlemi.
        /// </summary>
        /// <param name="id">Kullanıcı ID bilgisi.</param>
        /// <returns>Id'ye göre kullanıcı bilgilerini getirir.</returns>
        [SwaggerResponse(200, "Kullanıcı başarıyla getirildi.", typeof(ServiceResponse<User>))]
        [SwaggerResponse(400, "Geçersiz veri gönderildi.")]
        [SwaggerResponse(500, "Sunucu hatası.")]
        [SwaggerResponse(401, "Kimlik doğrulama başarısız.")]
        [SwaggerResponse(403, "Bu işlem için yeterli yetkiye sahip değilsiniz.")]
        [SwaggerResponse(404, "Kayıt bulunamadı.")]
        [SwaggerResponse(429, "Çok fazla istek.")]
        [SwaggerResponse(503, "Hizmet kullanılamıyor. Sunucu geçici olarak kullanılamıyor.")]
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var response = await _userService.GetUserByIdAsync(id);
            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Bütün kullanıcıları listeleme işlemi.
        /// </summary>
        /// <param>Kullanıcı kayıt bilgileri.</param>
        /// <returns>Başarılı giriş sonucu kullanıcı bilgileri ve JWT Token döndürür.</returns>
        [SwaggerResponse(200, "Bütün kullanıcılar başarıyla getirildi.", typeof(ServiceResponse<List<User>>))]
        [SwaggerResponse(500, "Sunucu hatası.")]
        [SwaggerResponse(401, "Kimlik doğrulama başarısız.")]
        [SwaggerResponse(403, "Bu işlem için yeterli yetkiye sahip değilsiniz.")]
        [SwaggerResponse(404, "Kayıt bulunamadı.")]
        [SwaggerResponse(429, "Çok fazla istek.")]
        [SwaggerResponse(503, "Hizmet kullanılamıyor. Sunucu geçici olarak kullanılamıyor.")]
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var response = await _userService.GetAllUsersAsync();
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        /// <summary>
        /// Kullanıcı bilgileri güncelleme işlemi.
        /// </summary>
        /// <param name="id">Kullanıcı ID bilgisi.</param>
        /// <param name="userDto">Kullanıcı biglileri.</param>
        /// <returns>Kullanıcı bilgilerini günceller</returns>
        [SwaggerResponse(200, "Kullanıcı başarıyla güncellendi.", typeof(ServiceResponse<bool>))]
        [SwaggerResponse(400, "Geçersiz veri gönderildi.")]
        [SwaggerResponse(500, "Sunucu hatası.")]
        [SwaggerResponse(401, "Kimlik doğrulama başarısız.")]
        [SwaggerResponse(403, "Bu işlem için yeterli yetkiye sahip değilsiniz.")]
        [SwaggerResponse(404, "Kayıt bulunamadı.")]
        [SwaggerResponse(429, "Çok fazla istek.")]
        [SwaggerResponse(503, "Hizmet kullanılamıyor. Sunucu geçici olarak kullanılamıyor.")]
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto userDto)
        {
            var response = await _userService.UpdateUserAsync(id, userDto);
            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Kullanıcı silme işlemi.
        /// </summary>
        /// <param name="id">Kullanıcı ID bilgisi.</param>
        /// <returns>Kullanıcıyı sistemden siler</returns>
        [SwaggerResponse(200, "Kullanıcı başarıyla silindi.", typeof(ServiceResponse<bool>))]
        [SwaggerResponse(400, "Geçersiz veri gönderildi.")]
        [SwaggerResponse(500, "Sunucu hatası.")]
        [SwaggerResponse(401, "Kimlik doğrulama başarısız.")]
        [SwaggerResponse(403, "Bu işlem için yeterli yetkiye sahip değilsiniz.")]
        [SwaggerResponse(404, "Kayıt bulunamadı.")]
        [SwaggerResponse(429, "Çok fazla istek.")]
        [SwaggerResponse(503, "Hizmet kullanılamıyor. Sunucu geçici olarak kullanılamıyor.")]
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var response = await _userService.DeleteUserAsync(id);
            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }
    }
}
