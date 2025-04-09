using Microsoft.AspNetCore.Mvc.Filters;

namespace RPGCharacterService.Controllers.Filters {
  /// <summary>
  /// This filter is used to handle exceptions that occur during the execution of an action method.
  /// It returns Internal Server Error (500) for unhandled exceptions.
  /// </summary>
  public class ExceptionFilterAttribute : ActionFilterAttribute {
    public override void OnActionExecuted(ActionExecutedContext context) {
      if (context.Exception == null) {
        return;
      }

      context.Result = new Microsoft.AspNetCore.Mvc.ObjectResult("Internal Server Error") {
        StatusCode = StatusCodes.Status500InternalServerError
      };

      context.ExceptionHandled = true;
    }
  }
}
