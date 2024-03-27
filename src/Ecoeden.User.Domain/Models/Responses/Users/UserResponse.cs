using Ecoeden.User.Domain.Models.Core;
using Ecoeden.User.Domain.Models.Enums;

namespace Ecoeden.User.Domain.Models.Responses.Users
{
    public class UserResponse
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }       
        public string PhoneNumber { get; set; }      
        public List<string> UserRoles { get; set; }
        public List<string> Permissions { get; set; }
        public DateTime LastLogin { get; set; }
        public MetaData MetaData { get; set; }
    }
}
