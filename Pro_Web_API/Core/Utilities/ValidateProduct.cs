using Microsoft.Data.SqlClient;
using Pro_Web_API.Core.DTO;
using System.Threading.Tasks;

namespace Pro_Web_API.Core.Utilities
{
    public class ValidateProduct
    { 
        public static async Task<string> ValidationProduct(RegisterProductDto productDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(productDto.ProductName))
                {
                    return "Ürün adı boş olamaz.";
                }

                if (productDto.Price <= 0)
                {
                    return "Ürün fiyatı pozitif bir değer olmalıdır.";
                }

                if (productDto.Quantity < 0)
                {
                    return "Ürün miktarı negatif olamaz.";
                }

                if (!await CategoryExistsAsync(productDto.Category))
                {
                    return "Geçersiz kategori ismi.";
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }

        private static async Task<bool> CategoryExistsAsync(string categoryName)
        {
            string connectionString = "Server=(localdb)\\mssqllocaldb;Database=LibraryDb;Trusted_Connection=True;MultipleActiveResultSets=true";
            using (var connection = new SqlConnection(connectionString))
            {
                var query = "SELECT COUNT(*) FROM [ProfenDB].[dbo].[Categories] WHERE category_name = @CategoryName";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CategoryName", categoryName);
                    await connection.OpenAsync();
                    var count = (int)await command.ExecuteScalarAsync();
                    return count > 0;
                }
            }
        }
    }
}
