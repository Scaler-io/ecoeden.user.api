namespace Ecoeden.User.Domain.Models.Requests;

public sealed class RemoveRoleRequest
{
    public string UserId { get; set; }
    public IEnumerable<string> Roles { get; set; }
}
