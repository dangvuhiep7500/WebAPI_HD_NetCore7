using Microsoft.AspNetCore.Mvc;

namespace WebAPI_HD.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        [HttpGet("{fileName}")]
        public IActionResult GetImage(string fileName)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);
            var image = System.IO.File.OpenRead(path);
            return File(image, "image/png");
        }
    }
}
