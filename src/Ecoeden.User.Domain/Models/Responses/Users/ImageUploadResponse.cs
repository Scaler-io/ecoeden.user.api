using Destructurama.Attributed;

namespace Ecoeden.User.Domain.Models.Responses.Users;
public class ImageUploadResponse
{
    public string Id { get; set; }
    public string Url { get; set; }
    [LogMasked]
    public string PublicId { get; set; }
}
