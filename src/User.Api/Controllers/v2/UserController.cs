using Asp.Versioning;
using Ecoeden.Swagger;
using Ecoeden.User.Application.Extensions;
using Ecoeden.User.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using User.Api.Services;

namespace User.Api.Controllers.v2
{
    [ApiVersion("2")]
    [Authorize]
    public class UserController : ApiBaseController
    {
        private readonly UserDbContext _userDbContext;
        public UserController(ILogger logger, IIdentityService identityService, 
            UserDbContext userDbContext) :
            base(logger, identityService)
        {
            _userDbContext = userDbContext;
        }

        [HttpGet("users")]
        [SwaggerHeader("CorrelationId", Description = "expects unique correlation id")]
        [SwaggerOperation(OperationId = "", Description = "")]
        //[RequirePermission("UserRead")]
        public async Task<IActionResult> GetAllUsers()
        {
            Logger.Here().MethodEnterd();

            var users = await _userDbContext.Users.ToListAsync();

            Logger.Here().MethodExited();
            return Ok(users);
        }
    }
}
