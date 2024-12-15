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
        private readonly CacheService _cacheService;

        public ProductService(IProductRepository productRepository, CacheService cacheService)
        {
            _productRepository = productRepository;
            _cacheService = cacheService;
        }
        public async Task<ServiceResponse<Product>> CreateProductAsync(RegisterProductDto productDto)
        {
            try
            {
                var validationMessage = await ValidateProduct.ValidationProduct(productDto);

                if (!string.IsNullOrEmpty(validationMessage))
                {
                    return new ServiceResponse<Product>
                    {
                        Success = false,
                        Message = validationMessage
                    };
                }

                var product = new Product
                {
                    Name = productDto.ProductName,
                    Description = productDto.Description,
                    Price = productDto.Price,
                    Quantity = productDto.Quantity,
                    Category = productDto.Category,
                };

                await _productRepository.AddAsync(product);

                var cacheKey = $"Product:{product.Id}";
                await _cacheService.SetCacheAsync(cacheKey, product, TimeSpan.FromMinutes(10));

                return new ServiceResponse<Product>
                {
                    Success = true,
                    Data = product,
                    Message = "Ürün başarıyla kaydedildi."
                };
            }
            catch (AppException ex)
            {
                return new ServiceResponse<Product>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                throw new AppException("Sunucu hatası oluştu.", 500);
            }


        }

        public async Task<ServiceResponse<bool>> DeleteProductAsync(int id)
        {

            try
            {
                var product = await _productRepository.GetByIdAsync(id);
                if (product == null)
                {
                    return new ServiceResponse<bool> { Success = false, Message = "Ürün bulunamadı." };
                }

                await _productRepository.DeleteAsync(product);

                return new ServiceResponse<bool> { Success = true, Message = "Ürün silindi.", Data = true };
            }
            catch (AppException ex)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                throw new AppException("Sunucu hatası oluştu.", 500);
            }
        }

        public async Task<ServiceResponse<List<Product>>> GetAllProductsAsync()
        {


            try
            {
                var products = await _productRepository.GetAllAsync();
                return new ServiceResponse<List<Product>>
                {
                    Success = true,
                    Data = products
                };
            }
            catch (AppException ex)
            {
                return new ServiceResponse<List<Product>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
  
                throw new AppException("Sunucu hatası oluştu.", 500);
            }
        }

        public async Task<ServiceResponse<Product?>> GetProductByIdAsync(int id)
        {
            try
            {
                var cacheKey = $"Product:{id}";

                var cachedProduct = await _cacheService.GetCacheAsync<Product>(cacheKey);
                if (cachedProduct != null)
                {
                    return new ServiceResponse<Product?>
                    {
                        Success = true,
                        Data = cachedProduct,
                        Message = "Ürün önbellekten getirildi."
                    };
                }

                var product = await _productRepository.GetByIdAsync(id);
                if (product != null)
                {
                    await _cacheService.SetCacheAsync(cacheKey, product, TimeSpan.FromMinutes(10));
                }

                return new ServiceResponse<Product?>
                {
                    Success = product != null,
                    Data = product,
                    Message = product != null ? "Ürün bulundu." : "Ürün bulunamadı."
                };
            }
            catch (AppException ex)
            {
                return new ServiceResponse<Product?>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
   
                throw new AppException("Sunucu hatası oluştu.", 500);
            }
        }

        public async Task<ServiceResponse<bool>> UpdateProductAsync(int id, RegisterProductDto productDto)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(id);
                if (product == null)
                {
                    return new ServiceResponse<bool> { Success = false, Message = "Ürün bulunamadı." };
                }

                var validationMessage = await ValidateProduct.ValidationProduct(productDto);

                if (!string.IsNullOrEmpty(validationMessage))
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = validationMessage
                    };
                }

                product.Name = productDto.ProductName;
                product.Description = productDto.Description;
                product.Price = productDto.Price;
                product.Quantity = productDto.Quantity;
                product.Category = productDto.Category;

                await _productRepository.UpdateAsync(product);

                var cacheKey = $"Product:{id}";
                await _cacheService.SetCacheAsync(cacheKey, product, TimeSpan.FromMinutes(10));

                return new ServiceResponse<bool> { Success = true, Message = "Ürün güncellendi.", Data = true };
            }
            catch (AppException ex)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            catch (Exception ex)
            {
                throw new AppException("Sunucu hatası oluştu.", 500);
            }
        }
    }
}
