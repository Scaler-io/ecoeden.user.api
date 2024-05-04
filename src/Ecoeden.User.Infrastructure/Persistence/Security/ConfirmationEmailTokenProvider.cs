using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ecoeden.User.Infrastructure.Persistence.Security;
internal class ConfirmationEmailTokenProvider<TUser> : DataProtectorTokenProvider<TUser>
    where TUser : IdentityUser
{
    public ConfirmationEmailTokenProvider(IDataProtectionProvider dataProtectionProvider, 
        IOptions<DataProtectionTokenProviderOptions> options, 
        ILogger<DataProtectorTokenProvider<TUser>> logger) 
        : base(dataProtectionProvider, options, logger)
    {
    }
}
