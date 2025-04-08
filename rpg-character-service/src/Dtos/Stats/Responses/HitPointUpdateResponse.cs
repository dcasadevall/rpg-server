using Swashbuckle.AspNetCore.Annotations;

namespace RPGCharacterService.Dtos.Stats.Responses {
  /// <summary>
  ///   Response object for hit point update operations
  /// </summary>
  public class HitPointUpdateResponse {
    /// <summary>
    ///   The character's current hit points after the update
    /// </summary>
    [SwaggerSchema(Description = "The character's current hit points after the update")]
    public int HitPoints { get; set; }
  }
}
