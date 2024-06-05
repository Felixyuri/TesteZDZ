namespace MyAspNetCoreProject.Data
{
    public class SupplierEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<ProductEntity> Products { get; set; } = new List<ProductEntity>();
    }
}
