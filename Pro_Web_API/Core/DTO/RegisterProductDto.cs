using Pro_Web_API.Core.Entities;

namespace Pro_Web_API.Core.DTO
{
    public class RegisterProductDto
    {
        public string ProductName { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; }
        public string Category { get; set; }
    }
}
