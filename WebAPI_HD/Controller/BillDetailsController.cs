using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using WebAPI_HD.Model;
using WebAPI_HD.Repository;

namespace WebAPI_HD.Controller
{
    [Authorize(Roles = UserRoles.Admin)]
    [Route("api/[controller]")]
    [ApiController]
    public class BillDetailsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public BillDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("GetBillDetails")]
        public async Task<ActionResult<IEnumerable<BillDetails>>> GetBillDetails()
        {
            return await _context.BillDetails.Include(x => x.Product).Include(x => x.Bill).ToListAsync();
        }
        [HttpPost("CreateBillDetails")]
        public async Task<IActionResult> PostBillDetails(BillDetails billdetail)
        {
            _context.BillDetails.Add(billdetail);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetBillDetails", new { id = billdetail.BillDetailsID }, billdetail);
        }
        private bool BillDetailsExists(int id)
        {
            return _context.BillDetails.Any(e => e.BillDetailsID == id);
        }
        [HttpPut("UpdateBillDetails/{id}")]
        public async Task<IActionResult> PutBillDetails(int id, BillDetails billdetail)
        {
            if (id != billdetail.BillDetailsID)
            {
                return BadRequest();
            }

            _context.Entry(billdetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BillDetailsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { message = "BillDetails updated successfully" });
        }
        [HttpDelete("DeleteBillDetails/{id}")]
        public async Task<ActionResult<BillDetails>> DeleteBillDetails(int id)
        {
            var billdetail = await _context.BillDetails.FindAsync(id);
            if (billdetail == null)
            {
                return NotFound();
            }

            _context.BillDetails.Remove(billdetail);
            await _context.SaveChangesAsync();

            return Ok(new { message = "BillDetails deleted successfully" });
        }
    }
}
