using Microsoft.AspNetCore.Mvc;

namespace RPGCharacterService.Controllers {
  /// <summary>
  /// I am sure there is a better way to implement a health check, but this was the simplest :).
  /// This HealthCheck class has nothing to do with our application, but instead is used to serve
  /// your typical health check endpoint.
  /// </summary>
  [ApiController]
  [Route("health")]
  public class HealthCheckController : ControllerBase {
    [HttpGet("ready")]
    public IActionResult Ready() {
      return Ok(new {status = "ready"});
    }
  }
}
