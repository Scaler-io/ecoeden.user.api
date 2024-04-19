using Swashbuckle.AspNetCore.Filters;

namespace Ecoeden.Swagger.Examples.User
{
    public class CreateUserResponseExample : IExamplesProvider<bool>
    {
        public bool GetExamples()
        {
            return true;
        }
    }
}
