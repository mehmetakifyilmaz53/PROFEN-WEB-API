using Pro_Web_API.Business.Concrete;
using Pro_Web_API.Core.DTO;
using Pro_Web_API.Core.Entities;

namespace Pro_Web_API.Business.Abstract
{
    public interface ICategoryService
    {
        Task<ServiceResponse<Category>> RegisterAsync(RegisterCategoryDto categoryDto);
        Task<ServiceResponse<List<Category>>> GetAllCategoriesAsync();
    }
}
