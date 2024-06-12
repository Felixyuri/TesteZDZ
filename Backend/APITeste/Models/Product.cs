using System.Globalization;

namespace APITeste.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string? Name { get; set; }
        public decimal Quantity { get; set; }
        public string? Supplier { get; set; }

    }
}