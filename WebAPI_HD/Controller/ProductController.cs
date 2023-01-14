using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using WebAPI_HD.Model;
using WebAPI_HD.Repository;

namespace WebAPI_HD.Controller
{
   /* [Authorize(Roles = UserRoles.Admin)]*/
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("GetProduct")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.Include(x => x.Categories).ToListAsync();
        }
        [HttpGet("GetProduct/{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }
        [HttpPost("CreateProduct")]
        public async Task<IActionResult> PostProduct(Product pro)
        {
            _context.Products.Add(pro);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetProduct", new { id = pro.ProductID }, pro);
        }
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductID == id);
        }
        [HttpPut("UpdateProduct/{id}")]
        public async Task<IActionResult> PutProduct(int id, Product products)
        {
            if (id != products.ProductID)
            {
                return BadRequest();
            }

            _context.Entry(products).State = EntityState.Modified;

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

            return Ok(new { message = "Product updated successfully" });
        }
        [HttpDelete("DeleteProduct/{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            var products = await _context.Products.FindAsync(id);
            if (products == null)
            {
                return NotFound();
            }

            _context.Products.Remove(products);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Product deleted successfully" });
        }
    }
}
