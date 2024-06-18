using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;

namespace CustomIdentity.API.Controllers
{
    namespace FreelanceCoders.Core.Controllers
    {
        [ApiController]
        public abstract class MainController : ControllerBase
        {
            protected ICollection<string> Errors = new List<string>();

            protected ActionResult CustomResponse(object result = null)
            {
                if (OperacaoValida()) return Ok(result);

                return BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                {"Mensagens", Errors.ToArray() }
            }));
            }
            protected ActionResult CustomResponse(ModelStateDictionary modelState)
            {
                var errors = modelState.Values.SelectMany(x => x.Errors);

                foreach (var error in errors)
                {
                    AddProcessError(error.ErrorMessage);
                }

                return CustomResponse();
            }

            protected void AddProcessError(string error)
            {
                Errors.Add(error);
            }

            protected void ClearProcessErrors()
            {
                Errors.Clear();
            }

            protected bool OperacaoValida()
            {
                return !Errors.Any();
            }
        }
    }
}
