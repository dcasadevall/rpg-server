using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RPGCharacterService.Controllers.Filters {
  /// <summary>
  /// Attribute used to add model state validation to every controller.
  /// Use this in program startup like:a
  ///
  /// services.AddControllers(options => {
  ///   options.Filters.Add(typeof(ValidateModelAttribute));
  /// });
  /// </summary>
  public class ValidateModelFilterAttribute : ActionFilterAttribute {
    public override void OnActionExecuting(ActionExecutingContext context) {
      if (!context.ModelState.IsValid) {
        context.Result = new BadRequestObjectResult(context.ModelState);
      }
    }
  }
}
