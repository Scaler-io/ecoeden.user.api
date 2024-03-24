using Ecoeden.User.Domain.Models.Core;
using Ecoeden.User.Domain.Models.Enums;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace User.Api.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public IdentityService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public const string IdClaim = ClaimTypes.NameIdentifier;
        public const string RoleClaim = ClaimTypes.Role;
        public const string FirstNameClaim = ClaimTypes.GivenName;
        public const string LastNameClaim = ClaimTypes.Surname;
        public const string UsernameClaim = JwtRegisteredClaimNames.Name;
        public const string EmailClaim = ClaimTypes.Email;
        public const string PermissionClaim = "permissions";

        public UserDto PrepareUser()
        {
            var claims = _contextAccessor.HttpContext.User.Claims;
            var token = _contextAccessor.HttpContext.Request.Headers.Authorization;

            var roleString = claims.Where(c => c.Type == RoleClaim).FirstOrDefault().Value;
            var permissionsString = claims.Where(c => c.Type == PermissionClaim).First().Value;

            return new UserDto
            {
                Id = claims.Where(c => c.Type == IdClaim).FirstOrDefault().Value,
                FirstName = claims.Where(c => c.Type == FirstNameClaim).FirstOrDefault().Value,
                LastName = claims.Where(c => c.Type == LastNameClaim).FirstOrDefault().Value,
                UserName = claims.Where(c => c.Type == UsernameClaim).FirstOrDefault().Value,
                AuthorizationDto = new AuthorizationDto
                {
                    Roles = JsonConvert.DeserializeObject<List<string>>(roleString),
                    Permissions = JsonConvert.DeserializeObject<List<string>>(permissionsString),
                    Token = token
                }
        };
        }
    }
}
