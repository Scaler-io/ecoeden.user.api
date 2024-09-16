using Asp.Versioning;
using Ecoeden.Swagger;
using Ecoeden.Swagger.Examples;
using Ecoeden.Swagger.Examples.User;
using Ecoeden.User.Application.Extensions;
using Ecoeden.User.Application.Features.User.Commands.AddUser;
using Ecoeden.User.Application.Features.User.Commands.EnableUser;
using Ecoeden.User.Application.Features.User.Commands.UploadImage;
using Ecoeden.User.Application.Features.User.Queries.GetAllUsers;
using Ecoeden.User.Application.Features.User.Queries.GetUserById;
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

namespace User.Api.Controllers.v2;

[ApiVersion("2")]
[Authorize]
public class UserController : ApiBaseController
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateUserRequest> _createSchemaValidator;

    public UserController(ILogger logger,
        IIdentityService identityService,
        IMediator mediator,
        IValidator<CreateUserRequest> createSchemaValidator) :
        base(logger, identityService)
    {
        _mediator = mediator;
        _createSchemaValidator = createSchemaValidator;
    }

    [HttpGet]
    [SwaggerHeader("CorrelationId", Description = "expects unique correlation id")]
    [SwaggerOperation(OperationId = "GetAllUser", Description = "Fetches all user list")]
    // 200
    [ProducesResponseType(typeof(List<UserResponse>), (int)HttpStatusCode.OK)]
    [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(GetAllUsersExample))]
    // 404
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
    [SwaggerResponseExample((int)HttpStatusCode.NotFound, typeof(NotFoundResponseExample))]
    // 500
    [ProducesResponseType(typeof(ApiExceptionResponse), (int)HttpStatusCode.InternalServerError)]
    [SwaggerResponseExample((int)HttpStatusCode.InternalServerError, typeof(InternalServerResponseExample))]
    public async Task<IActionResult> GetAllUsers()
    {
        Logger.Here().MethodEnterd();
        var request = new GetAllUserQuery();
        var result = await _mediator.Send(request);
        Logger.Here().MethodExited();
        return OkOrFailure(result);
    }

    [HttpGet("{id}")]
    [SwaggerHeader("CorrelationId", Description = "expects unique correlation id")]
    [SwaggerOperation(OperationId = "GetUserById", Description = "Fetches user details by id")]
    // 200
    [ProducesResponseType(typeof(UserResponse), (int)HttpStatusCode.OK)]
    [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(GetUserByIdExample))]
    // 404
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
    [SwaggerResponseExample((int)HttpStatusCode.NotFound, typeof(NotFoundResponseExample))]
    // 500
    [ProducesResponseType(typeof(ApiExceptionResponse), (int)HttpStatusCode.InternalServerError)]
    [SwaggerResponseExample((int)HttpStatusCode.InternalServerError, typeof(InternalServerResponseExample))]
    [RequirePermission(ApiAccess.UserRead)]
    public async Task<IActionResult> GetUserById([FromRoute] string id)
    {
        Logger.Here().MethodEnterd();
        var request = new GetUserByIdQuery(id);
        var result = await _mediator.Send(request);
        Logger.Here().MethodExited();
        return OkOrFailure(result);
    }

    [HttpPost]
    [SwaggerHeader("CorrelationId", Description = "expects unique correlation id")]
    [SwaggerOperation(OperationId = "CreateUser", Description = "Creates new user record")]
    // 200
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(CreateUserResponseExample))]
    // 404
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
    [SwaggerResponseExample((int)HttpStatusCode.NotFound, typeof(NotFoundResponseExample))]
    // 500
    [ProducesResponseType(typeof(ApiExceptionResponse), (int)HttpStatusCode.InternalServerError)]
    [SwaggerResponseExample((int)HttpStatusCode.InternalServerError, typeof(InternalServerResponseExample))]
    [RequirePermission(ApiAccess.UserWrite)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        Logger.Here().MethodEnterd();
        var validationResult = _createSchemaValidator.Validate(request);
        if (!validationResult.IsValid)
        {
            return ProcessValidationResult(validationResult);
        }

        var command = new AddUserCommand(request, RequestInformation);
        var result = await _mediator.Send(command);
        Logger.Here().MethodExited();
        return OkOrFailure(result);
    }

    [HttpPost("enable/{id}")]
    [SwaggerHeader("CorrelationId", Description = "expects unique correlation id")]
    [SwaggerOperation(OperationId = "UpdateVisibility", Description = "Toggles user visibility")]
    // 200
    [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
    [SwaggerResponseExample((int)HttpStatusCode.OK, typeof(bool))]
    // 404
    [ProducesResponseType(typeof(ApiResponse), (int)HttpStatusCode.NotFound)]
    [SwaggerResponseExample((int)HttpStatusCode.NotFound, typeof(NotFoundResponseExample))]
    // 500
    [ProducesResponseType(typeof(ApiExceptionResponse), (int)HttpStatusCode.InternalServerError)]
    [SwaggerResponseExample((int)HttpStatusCode.InternalServerError, typeof(InternalServerResponseExample))]
    [RequirePermission(ApiAccess.UserWrite)]
    public async Task<IActionResult> UpdateVisibility([FromRoute] string id)
    {
        Logger.Here().MethodEnterd();
        var command = new EnableUserCommand(id, RequestInformation.CurrentUser, RequestInformation.CorrelationId);
        var result = await _mediator.Send(command);
        Logger.Here().MethodExited();
        return OkOrFailure(result);
    }

    [HttpPost("image/upload/{id}")]
    [SwaggerHeader("CorrelationId", Description = "expects unique correlation id")]
    [SwaggerOperation(OperationId = "UploadUserImage", Description = "Uploads user image as aavatar")]
    public async Task<IActionResult> UploadUserImage([FromForm] ImageUploadRequest request, [FromRoute] string id)
    {
        Logger.Here().MethodEnterd();
        var command = new UploadImageCommand(request.File, id, RequestInformation.CorrelationId);
        var result = await _mediator.Send(command);
        Logger.Here().MethodExited();
        return OkOrFailure(result);
    }
}
