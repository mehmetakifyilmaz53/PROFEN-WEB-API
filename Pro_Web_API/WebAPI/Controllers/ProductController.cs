using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pro_Web_API.Business.Abstract;
using Pro_Web_API.Business.Concrete;
using Pro_Web_API.Core.DTO;
using Pro_Web_API.Core.Entities;
using Pro_Web_API.Data.Repositories;
using Swashbuckle.AspNetCore.Annotations;

namespace Pro_Web_API.WebAPI.Controllers
{
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IProductRepository _productRepo;

    public ProductController(IProductService productService, IProductRepository productRepo)
    {
        _productService = productService;
        _productRepo = productRepo;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [SwaggerResponse(200, "Ürün başarıyla kaydedildi.", typeof(ServiceResponse<Product>))]
    public async Task<IActionResult> AddProduct([FromBody] RegisterProductDto productDto)
    {
        var result = await _productService.CreateProductAsync(productDto);
        if (!result.Success) return BadRequest(result.Message);

        return Ok(result);
    }
}

}
