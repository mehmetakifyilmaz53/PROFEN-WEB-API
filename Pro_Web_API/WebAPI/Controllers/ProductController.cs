using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pro_Web_API.Business.Abstract;
using Pro_Web_API.Business.Concrete;
using Pro_Web_API.Core.DTO;
using Pro_Web_API.Core.Entities;
using Pro_Web_API.Data.Repositories.Abstract;
using Swashbuckle.AspNetCore.Annotations;

namespace Pro_Web_API.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IProductRepository _productRepo;

        public ProductController(IProductService productService, IProductRepository productRepo)
        {
            _productService = productService;
            _productRepo = productRepo;
        }

        /// <summary>
        /// Ürün ekleme işlemi.
        /// </summary>
        /// <param name="productDto">Ürün bilgleri.</param>
        /// <returns>Ürün ekler</returns>
        [SwaggerResponse(200, "Ürün başarıyla kaydedildi.", typeof(ServiceResponse<Product>))]
        [SwaggerResponse(400, "Geçersiz veri gönderildi.")]
        [SwaggerResponse(500, "Sunucu hatası.")]
        [SwaggerResponse(401, "Kimlik doğrulama başarısız.")]
        [SwaggerResponse(403, "Bu işlem için yeterli yetkiye sahip değilsiniz.")]
        [SwaggerResponse(429, "Çok fazla istek.")]
        [SwaggerResponse(503, "Hizmet kullanılamıyor. Sunucu geçici olarak kullanılamıyor.")]
        [HttpPost("add")]
        [Authorize(Roles = "Admin,Manager")] // Admin ve Manager rolleri için erişim
        public async Task<IActionResult> AddProduct([FromBody] RegisterProductDto productDto)
        {
            if (!User.IsInRole("Admin") && !User.IsInRole("Manager"))
            {
                return Forbid(); // Yeterli yetkiye sahip olmayan kullanıcılar için
            }

            var result = await _productService.CreateProductAsync(productDto);
            if (!result.Success) return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Ürün bilgilerini güncelleme işlemi.
        /// </summary>
        /// <param name="id">Ürün Mongo ID bilgisi.</param>
        /// <param name="productDto">Ürün bilgileri.</param>
        /// <returns>Ürün bilgilerini günceller</returns>
        [SwaggerResponse(200, "Ürün başarıyla güncellendi.", typeof(ServiceResponse<bool>))]
        [SwaggerResponse(400, "Geçersiz veri gönderildi.")]
        [SwaggerResponse(500, "Sunucu hatası.")]
        [SwaggerResponse(401, "Kimlik doğrulama başarısız.")]
        [SwaggerResponse(403, "Bu işlem için yeterli yetkiye sahip değilsiniz.")]
        [SwaggerResponse(404, "Kayıt bulunamadı.")]
        [SwaggerResponse(429, "Çok fazla istek.")]
        [SwaggerResponse(503, "Hizmet kullanılamıyor. Sunucu geçici olarak kullanılamıyor.")]
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] RegisterProductDto productDto)
        {
            // Manual role kontrolü
            if (!User.IsInRole("Admin") && !User.IsInRole("Manager"))
            {
                return Forbid();
            }

            var response = await _productService.UpdateProductAsync(id, productDto);
            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }


        /// <summary>
        /// Ürün silme işlemi.
        /// </summary>
        /// <param name="id">Ürün ID bilgisi.</param>
        /// <returns>Ürünü sistemden siler</returns>
        [SwaggerResponse(200, "Ürün başarıyla silindi.", typeof(ServiceResponse<bool>))]
        [SwaggerResponse(400, "Geçersiz veri gönderildi.")]
        [SwaggerResponse(500, "Sunucu hatası.")]
        [SwaggerResponse(401, "Kimlik doğrulama başarısız.")]
        [SwaggerResponse(403, "Bu işlem için yeterli yetkiye sahip değilsiniz.")]
        [SwaggerResponse(404, "Kayıt bulunamadı.")]
        [SwaggerResponse(429, "Çok fazla istek.")]
        [SwaggerResponse(503, "Hizmet kullanılamıyor. Sunucu geçici olarak kullanılamıyor.")]
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var response = await _productService.DeleteProductAsync(id);
            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// ID'ye göre ürün getirme işlemi.
        /// </summary>
        /// <param name="id">Ürün ID bilgisi.</param>
        /// <returns>Id'ye göre ürün bilgilerini getirir.</returns>
        [SwaggerResponse(200, "Ürün başarıyla getirildi.", typeof(ServiceResponse<Product>))]
        [SwaggerResponse(400, "Geçersiz veri gönderildi.")]
        [SwaggerResponse(500, "Sunucu hatası.")]
        [SwaggerResponse(401, "Kimlik doğrulama başarısız.")]
        [SwaggerResponse(403, "Bu işlem için yeterli yetkiye sahip değilsiniz.")]
        [SwaggerResponse(404, "Kayıt bulunamadı.")]
        [SwaggerResponse(429, "Çok fazla istek.")]
        [SwaggerResponse(503, "Hizmet kullanılamıyor. Sunucu geçici olarak kullanılamıyor.")]
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager,Viewer")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var response = await _productService.GetProductByIdAsync(id);
            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);

        }

        /// <summary>
        /// Bütün ürünleri listeleme işlemi.
        /// </summary>
        /// <returns>Başarılı giriş sonucu kullanıcı bilgileri ve JWT Token döndürür.</returns>
        [SwaggerResponse(200, "Bütün kullanıcılar başarıyla getirildi.", typeof(ServiceResponse<List<Product>>))]
        [SwaggerResponse(500, "Sunucu hatası.")]
        [SwaggerResponse(401, "Kimlik doğrulama başarısız.")]
        [SwaggerResponse(403, "Bu işlem için yeterli yetkiye sahip değilsiniz.")]
        [SwaggerResponse(404, "Kayıt bulunamadı.")]
        [SwaggerResponse(429, "Çok fazla istek.")]
        [SwaggerResponse(503, "Hizmet kullanılamıyor. Sunucu geçici olarak kullanılamıyor.")]
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetAllProducts()
        {
            var response = await _productService.GetAllProductsAsync();
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);

        }
    }

}
