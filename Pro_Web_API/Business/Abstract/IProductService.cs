using Pro_Web_API.Business.Concrete;
using Pro_Web_API.Core.DTO;
using Pro_Web_API.Core.Entities;

namespace Pro_Web_API.Business.Abstract
{
    public interface IProductService
    {
        Task<ServiceResponse<Product>> CreateProductAsync(RegisterProductDto productDto);
        Task<ServiceResponse<Product?>> GetProductByIdAsync(int id);
        Task<ServiceResponse<List<Product>>> GetAllProductsAsync();
        Task<ServiceResponse<bool>> UpdateProductAsync(int id, UpdateUserDto userDto);
        Task<ServiceResponse<bool>> DeleteProductAsync(int id);
    }
}
