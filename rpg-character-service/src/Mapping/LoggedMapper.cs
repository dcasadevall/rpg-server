using AutoMapper;

namespace RPGCharacterService.Mapping {
  /// <summary>
  /// LoggedMapper is a wrapper around IMapper that logs mapping errors.
  /// This is useful as the default IMapper throws exceptions without logging anything on the server side.
  /// </summary>
  /// <param name="mapper"></param>
  /// <param name="logger"></param>
  public class LoggedMapper(IMapper mapper, ILogger<LoggedMapper> logger) {
    public TDestination Map<TDestination>(object source) {
      try {
        return mapper.Map<TDestination>(source);
      } catch (AutoMapperMappingException ex) {
        logger.LogError(ex, "Error mapping object");
        throw;
      }
    }
  }
}
