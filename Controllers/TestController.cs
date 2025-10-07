using Microsoft.AspNetCore.Mvc;

namespace FertilizerWarehouseAPI.Controllers
{
[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
        [HttpGet("attendance")]
        public IActionResult GetAttendance()
        {
            return Ok(new { success = true, data = new List<object>() });
        }
    }
}