using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using WebAPI_HD.Model;
using WebAPI_HD.Repository;

namespace WebAPI_HD.Controller
{
 /*   [Authorize(Roles = UserRoles.Admin)]*/
    [Route("api/[controller]")]
    [ApiController]
    public class BillController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public BillController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("GetBill")]
        public async Task<ActionResult<IEnumerable<Bill>>> GetBill()
        {
            return await _context.Bills.Include(x => x.Customer).Include(x => x.ApplicationUser).Include(x => x.BillDetails).ToListAsync();
        }
        [HttpGet("GetBill/{id}")]
        public async Task<ActionResult<Bill>> GetBill(string id)
        {
            var bills = await _context.Bills.Include(x => x.Customer).Include(x => x.ApplicationUser).Include(x => x.BillDetails).FirstOrDefaultAsync(x => x.BillID == id);

            if (bills == null)
            {
                return NotFound();
            }

            return bills;
        }
        [HttpPost("CreateBill")]
        public async Task<IActionResult> PostBill(Bill bills)
        {
            _context.Bills.Add(bills);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetBill", new { id = bills.BillID }, bills);
        }
        private bool BillExists(string id)
        {
            return _context.Bills.Any(e => e.BillID == id);
        }
        [HttpPut("UpdateBill/{id}")]
        public async Task<IActionResult> PutBill(string id, Bill bills)
        {
            if (id != bills.BillID)
            {
                return BadRequest();
            }

            _context.Entry(bills).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BillExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { message = "Bill updated successfully" });
        }
        [HttpDelete("DeleteBill/{id}")]
        public async Task<ActionResult<Bill>> DeleteBill(int id)
        {
            var bills = await _context.Bills.FindAsync(id);
            if (bills == null)
            {
                return NotFound();
            }

            _context.Bills.Remove(bills);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Bill deleted successfully" });
        }
    }
}
