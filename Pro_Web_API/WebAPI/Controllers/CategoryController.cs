using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pro_Web_API.Business.Abstract;
using Pro_Web_API.Business.Concrete;
using Pro_Web_API.Core.DTO;

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


        [HttpPost("register")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register([FromBody] RegisterCategoryDto categoryDto)
        {
            var response = await _categoryService.RegisterAsync(categoryDto);
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

        [HttpGet]
        [Authorize(Roles = "Admin,Manager,Viewer")]
        public async Task<IActionResult> GetAllCategories()
        {
            var response = await _categoryService.GetAllCategoriesAsync();
            return Ok(new
            {
                success = true,
                data = response.Data,
                message = "Kategoriler başarıyla alındı."
            });
        }
    }
}
