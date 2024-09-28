using Ecoeden.User.Domain.Models.Responses.Users;
using Swashbuckle.AspNetCore.Filters;

namespace Ecoeden.Swagger.Examples.User;

public class CreateUserResponseExample : IExamplesProvider<CreateUserResponse>
{
    public CreateUserResponse GetExamples()
    {
        return new CreateUserResponse
        {
            Id = Guid.NewGuid().ToString(),
        };
    }
}
