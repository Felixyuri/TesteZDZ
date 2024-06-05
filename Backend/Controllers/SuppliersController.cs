using APITeste.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAspNetCoreProject.Data;

namespace APITeste.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SupplierController : ControllerBase
    {
        private readonly DataContext _context;

        public SupplierController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SupplierEntity>>> GetSuppliers()
        {
            try
            {
                var suppliers = await _context.Suppliers
               .Include(x => x.Products)
               .Select(s => new SupplierModel
               {
                   Id = s.Id,
                   Name = s.Name,
                   Products = s.Products.Select(p => p.Id).ToList(),
                   ProductsStock = s.Products.Sum(p => p.Quantity),
                   RegisteredProducts = s.Products.Count
               })
               .ToListAsync();

                return Ok(suppliers);
            }
            catch (DbUpdateConcurrencyException) { throw; };
        }

        [HttpPost]
        public async Task<ActionResult<SupplierEntity>> CreateSupplier([FromBody] SupplierModel supplier)
        {
            var existingSupplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.Name == supplier.Name);
            if (existingSupplier != null) { 
                return Conflict("Já existe um fornecedor com este nome.");
            }

            var supplierEntity = new SupplierEntity { Name = supplier.Name };
            try {
                _context.Suppliers.Add(supplierEntity);
                await _context.SaveChangesAsync();
            } catch (DbUpdateConcurrencyException) { throw; };

            return CreatedAtAction(nameof(GetSuppliers), new { id = supplierEntity.Id }, supplierEntity);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSuplier(int id, SupplierEntity supplier)
        {
            if (id != supplier.Id)
            {
                return BadRequest();
            }

            try
            {
                var existingSupplier = await _context.Suppliers.FindAsync(id);

                if (existingSupplier != null) {
                    return NotFound("Fornecedor não encontrado.");
                }

                existingSupplier.Name = supplier.Name;
                _context.Entry(existingSupplier).State = EntityState.Modified;


                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Suppliers.Any(e => e.Id == id))
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupplier(int id)
        {
            try
            {
                var supplier = await _context.Suppliers
                  .Include(s => s.Products)
                  .FirstOrDefaultAsync(s => s.Id == id);

                if (supplier == null)
                {
                    return NotFound("Fornecedor não encontrado.");
                }

                _context.Suppliers.Remove(supplier);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) { throw; };
  

            return NoContent();
        }
    }
}
