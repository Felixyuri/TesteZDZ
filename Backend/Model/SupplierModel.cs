using System.ComponentModel.DataAnnotations;

namespace APITeste.Model
{
    public class SupplierModel
    {
        public int? Id { get; internal set; }

        [Required(ErrorMessage = "O campo 'Name' é obrigatório.")]
        public string Name { get; set; }

        public List<int>? Products { get; internal set; }

        public decimal ProductsStock { get; internal set; }

        public int RegisteredProducts { get; internal set; }
    }
}
