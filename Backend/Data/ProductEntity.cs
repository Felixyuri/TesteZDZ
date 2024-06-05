using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MyAspNetCoreProject.Data
{
    public class ProductEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Quantity { get; set; }

        public ICollection<SupplierEntity> Suppliers { get; set; } = new List<SupplierEntity>();
    }
}
