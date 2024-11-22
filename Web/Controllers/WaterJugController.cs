using Core.Domain;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Web.DTOs;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WaterJugController : ControllerBase
    {
        private readonly IWaterJugService _service;

        public WaterJugController(IWaterJugService service)
        {
            _service = service;
        }

        [HttpPost("solve")]
        public IActionResult Solve([FromBody] WaterJugRequest request)
        {
            try
            {
                var result = _service.Solve(request.XCapacity, request.YCapacity, request.ZAmountWanted);
                return Ok(new { Solution = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}
