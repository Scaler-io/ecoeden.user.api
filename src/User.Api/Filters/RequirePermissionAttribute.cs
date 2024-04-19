using Ecoeden.User.Application.Contracts.Security;
using Ecoeden.User.Application.Extensions;
using Ecoeden.User.Domain.Models.Constants;
using Ecoeden.User.Domain.Models.Core;
using Ecoeden.User.Domain.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using User.Api.Services;

namespace User.Api.Filters
{

    public class RequirePermissionAttribute : TypeFilterAttribute
    {
        public RequirePermissionAttribute(ApiAccess requiredPermission) : base(typeof(RequirePermissionExecutor))
        {
            Arguments = new object[] { requiredPermission };
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RequirePermissionExecutor : Attribute, IActionFilter
    {
        private readonly IIdentityService _identityService;
        private readonly ILogger _logger;
        private readonly ApiAccess _requiredRole;
        private readonly IPermissionMapper _permissionMapper;

        public RequirePermissionExecutor(ApiAccess role,
            IIdentityService identityService,
            ILogger logger,
            IPermissionMapper permissionMapper)
        {
            _identityService = identityService;
            _logger = logger;
            _requiredRole = role;
            _permissionMapper = permissionMapper;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.Here().MethodEnterd();
            List<string> requiredPermissions = new();
            requiredPermissions.AddRange(_permissionMapper.GetPermissionsForRole(_requiredRole));
            var currentUser = _identityService.PrepareUser();
            var commonPermissions = requiredPermissions.Intersect(currentUser.AuthorizationDto.Permissions).ToList();
            if (!commonPermissions.Any())
            {
                _logger.Here().Error("No matching permission found");
                context.Result = new UnauthorizedObjectResult(new ApiError
                {
                    Code = ErrorCodes.Unauthorized.ToString(),
                    Message = ErrorMessages.Unauthorized
                });
            }
            _logger.Here().MethodExited();
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            
        }
    }
}
