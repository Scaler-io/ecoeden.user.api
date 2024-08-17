using AutoMapper;
using Contracts.Events;
using Ecoeden.User.Application.Extensions;
using Ecoeden.User.Domain.Entities;

namespace Ecoeden.User.Application.Mappers;

public class UserEventMappingProfile : Profile
{
    public UserEventMappingProfile()
    {
        CreateMap<ApplicationUser, UserInvitationSent>()
            .ForMember(s => s.UserId, o => o.MapFrom(d => d.Id));

        CreateMap<ApplicationUser, UserGenericPasswordSent>()
            .ForMember(s => s.Name, o => o.MapFrom(d => $"{d.FirstName} {d.Lastname}"))
            .ForMember(s => s.Email, o => o.MapFrom(d => d.Email))
            .ForMember(s => s.DefaultPassword, o => o.MapFrom(_ => "P@ssw0rd"));

        CreateMap<ApplicationUser, UserCreated>()
            .ForMember(s => s.FullName, o => o.MapFrom(d => $"{d.FirstName} {d.Lastname}"))
            .ForMember(s => s.LastLogin, o => o.MapFrom(d => d.LastLogin))
            .ForMember(s => s.CreatedOn, o => o.MapFrom(d => d.CreatedAt))
            .ForMember(s => s.UpdatedOn, o => o.MapFrom(d => d.UpdatedAt))
            .ForMember(s => s.UserRoles, o => o.MapFrom(d => d.GetUserRoleMappings()))
            .ForMember(s => s.Permissions, o => o.MapFrom(d => d.GetUserPermissionMappings()));
    }
}
