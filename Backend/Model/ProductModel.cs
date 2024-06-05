using System.ComponentModel.DataAnnotations;

namespace APITeste.Model
{
    public class ProductModel
    {
        public int? Id { get; internal set; }

        [Required(ErrorMessage = "O campo 'Name' é obrigatório.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "O campo 'Quantity' é obrigatório.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "O campo 'Suppliers' é obrigatório.")]
        public List<int> Suppliers { get; set; }
    }
}
