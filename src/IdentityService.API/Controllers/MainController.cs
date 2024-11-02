using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.API.Controllers
{
    [ApiController]
    public abstract class MainController : ControllerBase
    {
        protected ICollection<string> Errors = [];
        protected ActionResult CustomResponse(object? result = null) =>
            ValidOperation() ? Ok(result) : BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                {"Messages", Errors.ToArray() }
            }));

        protected ActionResult CustomResponse(ModelStateDictionary modelState)
        {
            var errors = modelState.Values.SelectMany(x => x.Errors);

            foreach (var error in errors)
            {
                AddProcessError(error.ErrorMessage);
            }

            return CustomResponse();
        }
        protected void AddProcessError(string error) => Errors.Add(error);
        protected void ClearProcessErrors() => Errors.Clear();
        protected bool ValidOperation() => Errors.Count == 0;
    }
}
