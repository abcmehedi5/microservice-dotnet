using Microsoft.AspNetCore.Mvc;

namespace FreelanceMarketplaceService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                Message = "Freelance Marketplace Service is working!",
                Timestamp = DateTime.UtcNow,
                Version = "1.0",
                Endpoints = new[] {
                    "GET /api/test",
                    "GET /health",
                    "GET /"
                }
            });
        }
    }
}