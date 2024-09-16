using Ecoeden.User.Domain.Models.Core;
using Ecoeden.User.Domain.Models.Responses.Users;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Ecoeden.User.Application.Features.User.Commands.UploadImage;
public class UploadImageCommand : IRequest<Result<ImageUploadResponse>>
{
    public UploadImageCommand(IFormFile file, string userId, string correlationId)
    {
        File = file;
        UserId = userId;
        CorrelationId = correlationId;
    }

    public IFormFile File { get; set; }
    public string UserId { get; set; }
    public string CorrelationId { get; set; }
}
