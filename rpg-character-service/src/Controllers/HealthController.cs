using Microsoft.AspNetCore.Mvc;

namespace RPGCharacterService.Controllers {
  /// <summary>
  /// I am sure there is a better way to implement a health check, but this was the simplest :).
  /// </summary>
  [ApiController]
  [Route("health")]
  public class HealthController : ControllerBase {
    [HttpGet("ready")]
    public IActionResult Ready() {
      return Ok(new {status = "ready"});
    }
  }
}
