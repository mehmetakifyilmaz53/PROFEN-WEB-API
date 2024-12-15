using Pro_Web_API.Business.Abstract;
using Pro_Web_API.Core.DTO;
using Pro_Web_API.Core.Entities;
using Pro_Web_API.Core.Utilities;
using Pro_Web_API.Data.Repositories;

namespace Pro_Web_API.Business.Concrete
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<ServiceResponse<List<Category>>> GetAllCategoriesAsync()
        {
            var response = new ServiceResponse<List<Category>>();
            try
            {
                var categories = await _categoryRepository.GetAllAsync();
                response.Success = true;
                response.Data = categories;
                response.Message = "Kategoriler getirildi";
            }
            catch (AppException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<ServiceResponse<Category>> RegisterAsync(RegisterCategoryDto categoryDto)
        {
            var response = new ServiceResponse<Category>();

            try
            {
                var existingCategory = await _categoryRepository.GetByCategoryNameAsync(categoryDto.CategoryName);
                if (existingCategory != null)
                {
                    response.Success = false;
                    response.Message = "Kategori adı zaten mevcut.";
                    return response;
                }

                var category = new Category
                {
                    category_name = categoryDto.CategoryName,
                };

                await _categoryRepository.AddAsync(category);

                response.Success = true;
                response.Data = category;
                response.Message = "Kategori kaydedildi.";
            }
            catch (AppException ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
