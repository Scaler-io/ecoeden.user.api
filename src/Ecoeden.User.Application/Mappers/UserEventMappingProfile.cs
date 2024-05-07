using AutoMapper;
using Contracts.Events;
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
    }
}
