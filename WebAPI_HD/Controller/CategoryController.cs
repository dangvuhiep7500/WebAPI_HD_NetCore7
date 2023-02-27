
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using WebAPI_HD.Model;
using WebAPI_HD.Repository;

namespace WebAPI_HD.Controller
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("GetCategory")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategory()
        {
            return await _context.Categories.Include(x => x.Products).ToListAsync();
        }
        [HttpGet("GetCategory/{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _context.Categories.Include(y => y.Products).FirstOrDefaultAsync(p => p.CategoryID == id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }
        [HttpPost("CreateCategory")]
        public async Task<IActionResult> PostCategory(CategoryViewModel cate)
        {
            var category = new Category
            {
                CategoryName = cate.CategoryName,
            };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetCategory", new { id = category.CategoryID }, cate);
        }
        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryID == id);
        }
        [HttpPut("UpdateCategory/{id}")]
        public async Task<IActionResult> PutCategory(int id, Category categories)
        {
            if (id != categories.CategoryID)
            {
                return BadRequest();
            }

            _context.Entry(categories).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { message = "Category updated successfully" });
        }
        [HttpDelete("DeleteCategory/{id}")]
        public async Task<ActionResult<Category>> DeleteCategory(int id)
        {
            var categories = await _context.Categories.FindAsync(id);
            if (categories == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(categories);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Category deleted successfully" });
        }
    }
}
