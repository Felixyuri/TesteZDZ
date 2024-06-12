using APITeste.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAspNetCoreProject.Data;

namespace APITeste.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly DataContext _context;

        public ProductController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductEntity>>> GetProducts()
        {
            var products = await _context.Products
                .Include(p => p.Suppliers)
                .Select(p => new ProductModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Quantity = (int)p.Quantity,
                    Suppliers = p.Suppliers.Select(s => s.Id).ToList()
                })
                .ToListAsync();

            return Ok(products);
        }


        [HttpPost]
        public async Task<ActionResult<ProductEntity>> CreateProduct([FromBody] ProductModel product)
        {
            var suppliersEntities = await _context.Suppliers
                .Where(s => product.Suppliers.Contains(s.Id))
                .ToListAsync();

            if (suppliersEntities.Count != product.Suppliers.Count) {
                return BadRequest("Um ou mais fornecedores não foram encontrados.");
            }

            var productEntity = new ProductEntity
            {
                Name = product.Name,
                Quantity = product.Quantity,
                Suppliers = suppliersEntities
            };

            _context.Products.Add(productEntity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProducts), new {id = productEntity.Id }, productEntity);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductModel product)
        {
            if (product == null)
            {
                return BadRequest();
            }

            var productEntity = await _context.Products
                .Include(p => p.Suppliers)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (productEntity == null)
            {
                return NotFound();
            }

            productEntity.Name = product.Name;
            productEntity.Quantity = product.Quantity;

            var existingSupplierIds = new HashSet<int>(productEntity.Suppliers.Select(s => s.Id));
            var newSupplierIds = new HashSet<int>(product.Suppliers);

            var suppliersToRemove = productEntity.Suppliers.Where(s => !newSupplierIds.Contains(s.Id)).ToList();

            foreach (var supplier in suppliersToRemove)
            {
                productEntity.Suppliers.Remove(supplier);
            }

            foreach (var supplierId in newSupplierIds)
            {
                if (!existingSupplierIds.Contains(supplierId))
                {
                    var supplier = await _context.Suppliers.FindAsync(supplierId);
                    if (supplier != null)
                    {
                        productEntity.Suppliers.Add(supplier);
                    }
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Suppliers)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
