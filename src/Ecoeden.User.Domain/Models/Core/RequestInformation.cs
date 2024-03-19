namespace Ecoeden.User.Domain.Models.Core
{
    public sealed class RequestInformation
    {
        public string CorrelationId { get; set; }
        public UserDto CurrentUser { get; set; }
    }
}
