using Microsoft.AspNetCore.Http;

namespace Ecoeden.User.Domain.Models.Requests;
public class ImageUploadRequest
{
    public IFormFile File { get; set; }
}
