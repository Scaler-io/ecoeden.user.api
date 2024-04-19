namespace Ecoeden.User.Domain.Models.Requests;

public sealed class UpdateRoleRequest
{
    public string UserId { get; set; }
    public IEnumerable<string> Roles { get; set; }
}
