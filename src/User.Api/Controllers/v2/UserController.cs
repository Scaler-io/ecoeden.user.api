using Asp.Versioning;
using Ecoeden.Swagger;
using Ecoeden.User.Application.Extensions;
using Ecoeden.User.Application.Features.User.Queries.GetAllUsers;
using Ecoeden.User.Application.Features.User.Queries.GetUserById;
using Ecoeden.User.Domain.Models.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using User.Api.Filters;
using User.Api.Services;

namespace User.Api.Controllers.v2
{
    [ApiVersion("2")]
    [Authorize]
    public class UserController : ApiBaseController
    {
        private readonly IMediator _mediator;
        public UserController(ILogger logger, IIdentityService identityService, IMediator mediator) :
            base(logger, identityService)
        {
            _mediator = mediator;
        }

        [HttpGet("users")]
        [SwaggerHeader("CorrelationId", Description = "expects unique correlation id")]
        [SwaggerOperation(OperationId = "GetAllUser", Description = "Fetches all user list")]
        [RequirePermission(ApiAccess.UserRead)]
        public async Task<IActionResult> GetAllUsers()
        {
            Logger.Here().MethodEnterd();
            var request = new GetAllUserQuery();
            var result = await _mediator.Send(request);
            Logger.Here().MethodExited();
            return OkOrFailure(result);
        }

        [HttpGet("user/{id}")]
        [SwaggerHeader("CorrelationId", Description = "expects unique correlation id")]
        [SwaggerOperation(OperationId = "GetUserById", Description = "Fetches user details by id")]
        [RequirePermission(ApiAccess.UserRead)]
        public async Task<IActionResult> GetUserById([FromRoute] string id)
        {
            Logger.Here().MethodEnterd();
            var request = new GetUserByIdQuery(id);
            var result = await _mediator.Send(request);
            Logger.Here().MethodExited();
            return OkOrFailure(result);
        }
    }
}
