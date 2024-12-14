using MongoDB.Driver;
using Pro_Web_API.Business.Abstract;
using Pro_Web_API.Core.DTO;
using Pro_Web_API.Core.Entities;
using Pro_Web_API.Core.Utilities;
using Pro_Web_API.Data.Repositories;

namespace Pro_Web_API.Business.Concrete
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ServiceResponse<Product> response = new ServiceResponse<Product>();

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public async Task<ServiceResponse<Product>> CreateProductAsync(RegisterProductDto productDto)
        {

            var product = new Product
            {
                Name = productDto.ProductName,
                Description = PasswordHelper.HashPassword(productDto.Description),
                Price = productDto.Price,
                Quantity = productDto.Quantity,
                Category = productDto.Category,
            };
            await _productRepository.AddAsync(product);

            response.Success = true;
            response.Data = product;
            response.Message = "Kullanıcı başarıyla kaydedildi.";
            return response;

        }

        public Task<ServiceResponse<bool>> DeleteProductAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<List<Product>>> GetAllProductsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<Product?>> GetProductByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResponse<bool>> UpdateProductAsync(int id, UpdateUserDto userDto)
        {
            throw new NotImplementedException();
        }
    }
}
