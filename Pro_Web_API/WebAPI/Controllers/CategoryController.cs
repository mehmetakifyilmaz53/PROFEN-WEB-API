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
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Kategori kayıt işlemi.
        /// </summary>
        /// <param name="categoryDto">Kategori kayıt bilgileri.</param>
        /// <returns>Başarılı kayıt sonucu veya hata mesajı döndürür.</returns>
        [SwaggerResponse(200, "Kateogir başarıyla kaydedildi.", typeof(ServiceResponse<Category>))]
        [SwaggerResponse(400, "Geçersiz veri gönderildi.")]
        [SwaggerResponse(500, "Sunucu hatası.")]
        [SwaggerResponse(401, "Kimlik doğrulama başarısız.")]
        [SwaggerResponse(403, "Bu işlem için yeterli yetkiye sahip değilsiniz.")]
        [SwaggerResponse(429, "Çok fazla istek.")]
        [SwaggerResponse(503, "Hizmet kullanılamıyor. Sunucu geçici olarak kullanılamıyor.")]
        [HttpPost("register")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register([FromBody] RegisterCategoryDto categoryDto)
        {
            var response = await _categoryService.RegisterAsync(categoryDto);
            if (!response.Success)
            {
                return NotFound(new
                {
                    success = false,
                    message = response.Message
                });
            }

            return Ok(response);
        }

        /// <summary>
        /// Bütün kategorileri getirme işlemi.
        /// </summary>
        /// <returns>Bütün kategorileri getirir.</returns>
        [SwaggerResponse(200, "Kategoriler başarıyla getirildi.", typeof(ServiceResponse<List<Category>>))]
        [SwaggerResponse(500, "Sunucu hatası.")]
        [SwaggerResponse(401, "Kimlik doğrulama başarısız.")]
        [SwaggerResponse(403, "Bu işlem için yeterli yetkiye sahip değilsiniz.")]
        [SwaggerResponse(404, "Kayıt bulunamadı.")]
        [SwaggerResponse(429, "Çok fazla istek.")]
        [SwaggerResponse(503, "Hizmet kullanılamıyor. Sunucu geçici olarak kullanılamıyor.")]
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetAllCategories()
        {
            var response = await _categoryService.GetAllCategoriesAsync();
            return Ok(response);
        }
    }
}
