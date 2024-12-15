using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Pro_Web_API.Business.Abstract;
using Pro_Web_API.Business.Concrete;
using Pro_Web_API.Core.DTO;
using Pro_Web_API.Core.Entities;
using Pro_Web_API.Data.Repositories;
using Pro_Web_API.WebAPI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Pro_Web_API.Tests.Controllers
{
    public class ProductControllerTests
    {
        private readonly Mock<IProductService> _productServiceMock;
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly ProductController _controller;

        public ProductControllerTests()
        {
            _productServiceMock = new Mock<IProductService>();
            _productRepositoryMock = new Mock<IProductRepository>();
            _controller = new ProductController(_productServiceMock.Object, _productRepositoryMock.Object);
        }

        private void MockUserWithRole(int role, string userId = "1")
        {
            string roleName = role switch
            {
                (int)UserRole.Admin => UserRole.Admin.ToString(),
                (int)UserRole.Manager => UserRole.Manager.ToString(),
                (int)UserRole.Viewer => UserRole.Viewer.ToString(),
                _ => "Unknown"
            };

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.Name, "admin"),
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, roleName)
        }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }


        [Fact]
        public async Task AddProduct_ShouldReturnForbidden_WhenUserIsNotAdminOrManager()
        {
     
            MockUserWithRole((int)UserRole.Viewer);

            var productDto = new RegisterProductDto
            {
                ProductName = "Test Product",
                Description = "Test Description",
                Price = 100,
                Quantity = 10,
                Category = "kahve"
            };

      
            _productServiceMock
                .Setup(x => x.CreateProductAsync(It.IsAny<RegisterProductDto>()))
                .ReturnsAsync(new ServiceResponse<Product> { Success = false });

        
            var result = await _controller.AddProduct(productDto);

     
            Assert.IsType<ForbidResult>(result); 
        }



        [Fact]
        public async Task AddProduct_ShouldReturnOk_WhenProductIsAdded()
        {
        
            MockUserWithRole(0); 
            var productDto = new RegisterProductDto { ProductName = "Test", Price = 100 };

            _productServiceMock.Setup(x => x.CreateProductAsync(productDto))
                               .ReturnsAsync(new ServiceResponse<Product>
                               {
                                   Success = true,
                                   Message = "Ürün başarıyla kaydedildi."
                               });

            var result = await _controller.AddProduct(productDto);

            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
        }




        [Fact]
        public async Task UpdateProduct_ShouldReturnForbidden_WhenUserIsNotAdminOrManager()
        {
            MockUserWithRole((int)UserRole.Viewer); // Viewer rolü atanıyor

            var productDto = new RegisterProductDto
            {
                ProductName = "Updated Product",
                Description = "Updated Description",
                Price = 200,
                Quantity = 5,
                Category = "electronics"
            };

            _productServiceMock.Setup(service => service.UpdateProductAsync(1, productDto))
                .ReturnsAsync(new ServiceResponse<bool> { Success = true });

            var result = await _controller.UpdateProduct(1, productDto);

            Assert.IsType<ForbidResult>(result); // Yetkisiz erişim sonucu
        }



        [Fact]
        public async Task DeleteProduct_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            _productServiceMock.Setup(service => service.DeleteProductAsync(1))
                .ReturnsAsync(new ServiceResponse<bool> { Success = false, Message = "Ürün bulunamadı." });

            var result = await _controller.DeleteProduct(1);
   
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var response = Assert.IsType<ServiceResponse<bool>>(notFoundResult.Value);
            Assert.False(response.Success);
            Assert.Equal("Ürün bulunamadı.", response.Message);
        }

        [Fact]
        public async Task DeleteProduct_ShouldReturnOk_WhenProductIsDeleted()
        {
   
            _productServiceMock.Setup(service => service.DeleteProductAsync(1))
                .ReturnsAsync(new ServiceResponse<bool> { Success = true, Message = "Ürün silindi.", Data = true });

            var result = await _controller.DeleteProduct(1);
         
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ServiceResponse<bool>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("Ürün silindi.", response.Message);
        }

        [Fact]
        public async Task GetProductById_ShouldReturnOk_WhenUserHasAnyRole()
        {
            var product = new Product { Id = 1, Name = "ExistingProduct" };
            _productServiceMock.Setup(service => service.GetProductByIdAsync(1))
                .ReturnsAsync(new ServiceResponse<Product> { Success = true, Data = product });

            MockUserWithRole(0); 
            var result = await _controller.GetProductById(1);
            Assert.IsType<OkObjectResult>(result);

            MockUserWithRole(1); 
            result = await _controller.GetProductById(1);
            Assert.IsType<OkObjectResult>(result);

            MockUserWithRole(2);
            result = await _controller.GetProductById(1);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetAllProducts_ShouldReturnOk_WhenUserHasAnyRole()
        {
            var products = new List<Product> { new Product { Id = 1, Name = "Product1" } };
            _productServiceMock.Setup(service => service.GetAllProductsAsync())
                .ReturnsAsync(new ServiceResponse<List<Product>> { Success = true, Data = products });

            MockUserWithRole(0); 
            var result = await _controller.GetAllProducts();
            Assert.IsType<OkObjectResult>(result);

            MockUserWithRole(1);
            result = await _controller.GetAllProducts();
            Assert.IsType<OkObjectResult>(result);

            MockUserWithRole(2); 
            result = await _controller.GetAllProducts();
            Assert.IsType<OkObjectResult>(result);
        }
    }
}
