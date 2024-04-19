using Asp.Versioning;
using Ecoeden.Swagger.Examples.User;
using Ecoeden.Swagger.Examples;
using Ecoeden.Swagger;
using Ecoeden.User.Application.Extensions;
using Ecoeden.User.Application.Features.Role.Commands.UpdateRole;
using Ecoeden.User.Domain.Models.Core;
using Ecoeden.User.Domain.Models.Enums;
using Ecoeden.User.Domain.Models.Requests;
using Ecoeden.User.Domain.Models.Responses.Users;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.Net;
using User.Api.Filters;
using User.Api.Services;
using Ecoeden.User.Application.Features.Role.Commands.RemoveRole;

namespace User.Api.Controllers.v2
{
    [ApiVersion("2")]
    [Authorize]
    public class RoleController : ApiBaseController
    {
        private readonly IValidator<UpdateRoleRequest> _updateRoleValidator;
        private readonly IValidator<RemoveRoleRequest> _removeRoleValidator;
        private readonly IMediator _mediator;

        public RoleController(ILogger logger,
            IIdentityService identityService,
            IMediator mediator,
            IValidator<UpdateRoleRequest> updateRoleValidator,
            IValidator<RemoveRoleRequest> removeRoleValidator)
            : base(logger, identityService)
        {
            _mediator = mediator;
            _updateRoleValidator = updateRoleValidator;
            _removeRoleValidator = removeRoleValidator;
        }

        [HttpPut("role/update")]
        [SwaggerHeader("CorrelationId", Description = "expects unique correlation id")]
        [SwaggerOperation(OperationId = "UpdateUserRole", Description = "Updates user role")]
        // 200
        [ProducesResponseType(typeof(List<UserResponse>), (int)HttpStatusCode.OK)]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(bool))]
        // 404
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        [SwaggerResponseExample((int)HttpStatusCode.NotFound, typeof(NotFoundResponseExample))]
        // 500
        [ProducesResponseType(typeof(ApiExceptionResponse), (int)HttpStatusCode.InternalServerError)]
        [SwaggerResponseExample((int)HttpStatusCode.InternalServerError, typeof(InternalServerResponseExample))]
        [RequirePermission(ApiAccess.RoleWrite)]
        public async Task<IActionResult> UpdateUserRole([FromBody] UpdateRoleRequest request)
        {
            Logger.Here().MethodEnterd();
            var validationResult = _updateRoleValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                return ProcessValidationResult(validationResult);
            }

            UpdateRoleCommand command = new(request, RequestInformation.CurrentUser);
            var result = await _mediator.Send(command);
            Logger.Here().MethodExited();
            return OkOrFailure(result);
        }

        [HttpDelete("role/delete")]
        [SwaggerHeader("CorrelationId", Description = "expects unique correlation id")]
        [SwaggerOperation(OperationId = "RemoveUserRole", Description = "Removes user role")]
        // 200
        [ProducesResponseType(typeof(List<UserResponse>), (int)HttpStatusCode.OK)]
        [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(bool))]
        //
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.BadRequest)]
        [SwaggerResponseExample((int)HttpStatusCode.BadRequest, typeof(BadRequestResponseExample))]
        // 404
        [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
        [SwaggerResponseExample((int)HttpStatusCode.NotFound, typeof(NotFoundResponseExample))]
        // 500
        [ProducesResponseType(typeof(ApiExceptionResponse), (int)HttpStatusCode.InternalServerError)]
        [SwaggerResponseExample((int)HttpStatusCode.InternalServerError, typeof(InternalServerResponseExample))]
        [RequirePermission(ApiAccess.RoleWrite)]
        public async Task<IActionResult> RemoveUserRole([FromBody] RemoveRoleRequest request)
        {
            Logger.Here().MethodEnterd();
            var validationResult = _removeRoleValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                return ProcessValidationResult(validationResult);
            }
            RemoveRoleCommand command = new(request, RequestInformation.CurrentUser);
            var result = await _mediator.Send(command);
            Logger.Here().MethodExited();
            return OkOrFailure(result);
        }
    }
}
