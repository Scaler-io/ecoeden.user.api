namespace Ecoeden.User.Domain.ConfigurationOptions.Cloudinary;
public class CloudinaryOption
{
    public const string OptionName = "CloudinarySettings";
    public string CloudName { get; set; }
    public string ApiKey { get; set; }
    public string ApiSecret { get; set; }
    public string UploadBasePath { get; set; }
}
