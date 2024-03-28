using Ecoeden.User.Domain.Models.Core;
using Ecoeden.User.Domain.Models.Responses.Users;
using Swashbuckle.AspNetCore.Filters;

namespace Ecoeden.Swagger.Examples.User
{
    public sealed class GetUserByIdExample : IExamplesProvider<UserResponse>
    {
        public UserResponse GetExamples()
        {
            return new UserResponse
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = "john",
                LastName = "Doe",
                UserName = "john123",
                NormalizedUserName = "JOHN123",
                Email = "john@email.com",
                NormalizedEmail = "JOHN@EMAIL.COM",
                EmailConfirmed = true,
                PhoneNumber = "1234567890",
                LastLogin = DateTime.Now,
                UserRoles = new List<string> { "role1", "role2" },
                Permissions = new List<string> { "permission:read", "permission:write" },
                MetaData = new MetaData
                {
                    CreatedAt = DateTime.Now.ToString("dd MM yyyy HH:mm:ss tt"),
                    UpdatedAt = DateTime.Now.ToString("dd MM yyyy HH:mm:ss tt"),
                    CreatedBy = "Default",
                    UpdtedBy = "Default"
                }
            };
        }
    }
}
