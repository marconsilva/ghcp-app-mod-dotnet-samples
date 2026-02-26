using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using System.Threading.Tasks;

namespace ContosoUniversity.Controllers
{
    /// <summary>
    /// Health check endpoint for container orchestration and monitoring
    /// </summary>
    [Route("[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly SchoolContext _context;

        public HealthController(SchoolContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Basic health check endpoint
        /// Returns 200 OK if the application is running
        /// </summary>
        [HttpGet]
        [Route("")]
        [Route("~/health")]
        public IActionResult Get()
        {
            return Ok(new
            {
                status = "Healthy",
                timestamp = System.DateTime.UtcNow,
                version = "1.0.0",
                environment = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            });
        }

        /// <summary>
        /// Readiness probe - checks if the application is ready to serve traffic
        /// Includes database connectivity check
        /// </summary>
        [HttpGet("ready")]
        [Route("~/health/ready")]
        public async Task<IActionResult> Ready()
        {
            try
            {
                // Check database connectivity
                await _context.Database.CanConnectAsync();

                return Ok(new
                {
                    status = "Ready",
                    timestamp = System.DateTime.UtcNow,
                    database = "Connected"
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(503, new
                {
                    status = "Not Ready",
                    timestamp = System.DateTime.UtcNow,
                    database = "Disconnected",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Liveness probe - checks if the application is alive
        /// Simpler check than readiness
        /// </summary>
        [HttpGet("live")]
        [Route("~/health/live")]
        public IActionResult Live()
        {
            return Ok(new
            {
                status = "Alive",
                timestamp = System.DateTime.UtcNow
            });
        }

        /// <summary>
        /// Startup probe - checks if the application has started successfully
        /// </summary>
        [HttpGet("startup")]
        [Route("~/health/startup")]
        public IActionResult Startup()
        {
            return Ok(new
            {
                status = "Started",
                timestamp = System.DateTime.UtcNow
            });
        }
    }
}
