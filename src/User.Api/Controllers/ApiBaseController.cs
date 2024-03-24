using Ecoeden.User.Domain.Models.Core;
using Ecoeden.User.Domain.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using User.Api.Extensions;
using User.Api.Services;

namespace User.Api.Controllers
{
    [Route("api/v{version:apiVersion}")]
    [ApiController]
    public class ApiBaseController : ControllerBase
    {
        protected ILogger Logger;
        protected readonly IIdentityService IdentityService;

        public ApiBaseController(ILogger logger, IIdentityService identityService)
        {
            Logger = logger;
            IdentityService = identityService;
        }

        protected UserDto CurrentUser => IdentityService.PrepareUser();

        protected RequestInformation RequestInformation => new RequestInformation
        {
            CorrelationId = GetOrGenerateCorelationId(),
            CurrentUser = CurrentUser
        };

        protected string GetOrGenerateCorelationId() => Request?.GetRequestHeaderOrdefault("CorrelationId", $"GEN-{Guid.NewGuid().ToString()}");

        protected IActionResult OkOrFailure<T>(Result<T> result)
        {
            if (result == null) return NotFound(new ApiResponse(ErrorCodes.NotFound));
            if (result.IsSuccess && result.Value == null) return NotFound(new ApiResponse(ErrorCodes.NotFound));
            if (result.IsSuccess && result.Value != null) return Ok(result.Value);

            return result.ErrorCode switch
            {
                ErrorCodes.BadRequest => BadRequest(new ApiValidationResponse(result.ErrorMessage)),
                ErrorCodes.InternalServerError => InternalServerError(new ApiExceptionResponse(result.ErrorMessage)),
                ErrorCodes.NotFound => NotFound(new ApiResponse(ErrorCodes.NotFound, result.ErrorMessage)),
                ErrorCodes.Unauthorized => Unauthorized(new ApiResponse(ErrorCodes.Unauthorized, result.ErrorMessage)),
                ErrorCodes.OperationFailed => BadRequest(new ApiResponse(ErrorCodes.OperationFailed, result.ErrorMessage)),
                _ => BadRequest(new ApiResponse(ErrorCodes.BadRequest, result.ErrorMessage))
            };
        }


        private ObjectResult InternalServerError(ApiResponse response)
        {
            return new ObjectResult(response)
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                ContentTypes = new Microsoft.AspNetCore.Mvc.Formatters.MediaTypeCollection
                {
                    "application/json"
                }
            };
        }
    }
}
