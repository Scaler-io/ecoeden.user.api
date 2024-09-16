using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Ecoeden.User.Application.Contracts.Data.Repositories;
using Ecoeden.User.Application.Extensions;
using Ecoeden.User.Domain.ConfigurationOptions.Cloudinary;
using Ecoeden.User.Domain.Models.Core;
using Ecoeden.User.Domain.Models.Enums;
using Ecoeden.User.Domain.Models.Responses.Users;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using static App.Metrics.Health.HealthCheck;

namespace Ecoeden.User.Application.Features.User.Commands.UploadImage;
public class UploadImageCommandHandler : IRequestHandler<UploadImageCommand, Result<ImageUploadResponse>>
{
    private readonly ILogger _logger;
    private readonly IUserRepository _userRepository;
    private readonly Cloudinary _cloudinary;
    private readonly CloudinaryOption _cloudinaryOption;

    public UploadImageCommandHandler(ILogger logger, IUserRepository userRepository, IOptions<CloudinaryOption> cloudinaryOption)
    {
        _cloudinaryOption = cloudinaryOption.Value;
        var account = new Account
        {
            Cloud = _cloudinaryOption.CloudName,
            ApiKey =_cloudinaryOption.ApiKey,
            ApiSecret =_cloudinaryOption.ApiSecret,
        };
        _logger = logger;
        _userRepository = userRepository;
        _cloudinary = new Cloudinary(account);
    }

    public async Task<Result<ImageUploadResponse>> Handle(UploadImageCommand request, CancellationToken cancellationToken)
    {
        _logger.Here().MethodEnterd();
        _logger.Here().Information("Request - upload user image");

        var userData = await _userRepository.GetUserById(request.UserId);
        if(userData == null)
        {
            _logger.Here().Information("No user was found with {id}", request.UserId);
            return Result<ImageUploadResponse>.Failure(ErrorCodes.NotFound);
        }

        var file = request.File;
        var uploadResult = new ImageUploadResult();
        var uploadParams = new ImageUploadParams();

        //if (!string.IsNullOrEmpty(userData.ImageId))
        //{
        //    var deletionParams = new DeletionParams(userData.ImageId);
        //    var deletionResult = await _cloudinary.DestroyAsync(deletionParams);
        //    if(deletionResult.Error != null)
        //    {
        //        _logger.Here().Error("{0} Image deletion failed. {1}", ErrorCodes.InternalServerError, deletionResult.Error);
        //        return Result<ImageUploadResponse>.Failure(ErrorCodes.InternalServerError);
        //    }
        //}

        if(file.Length > 0)
        {
            using var stream = file.OpenReadStream();
            uploadParams = PrepareImageUploadParams(file, stream, userData.UserName);
            uploadResult = await _cloudinary.UploadAsync(uploadParams);
        }

        if (uploadResult.Error != null)
        {
            _logger.Here().Error("Image upload to cloudinary failed for {0}, [Error:{1}]",
                    userData.UserName, uploadResult.Error.Message);
            return Result<ImageUploadResponse>.Failure(ErrorCodes.InternalServerError, "Image upload failed");
        }

        var imageUploadResult = HandleImageUploadResponse(uploadResult, userData.Id);
        userData.Image = imageUploadResult.Value.Url;
        userData.ImageId = imageUploadResult.Value.PublicId;
        await _userRepository.UpdateUser(userData);

        _logger.Here().Information("Image upload to cloudinary success. {@publicId}", uploadParams.PublicId);
        _logger.Here().MethodExited();

        return imageUploadResult;
    }

    private ImageUploadParams PrepareImageUploadParams(IFormFile file, Stream stream, string userName)
    {
        var newFileName = $"{file.FileName}_{DateTime.UtcNow}";
        return new ImageUploadParams
        {
            File = new(newFileName, stream),
            Transformation = new Transformation()
                             .Height(500)
                             .Width(500)
                             .Crop("fill")
                             .Gravity("face"),
            Folder = $"Ecoeden/users/{userName}"
        };
    }

    private Result<ImageUploadResponse> HandleImageUploadResponse(ImageUploadResult uploadResult, string userId)
    {
        var imageUploadResponse = new ImageUploadResponse
        {
            Id = userId,
            Url = uploadResult.SecureUrl.AbsoluteUri,
            PublicId = uploadResult.PublicId,
        };

        return Result<ImageUploadResponse>.Success(imageUploadResponse);
    }
}
