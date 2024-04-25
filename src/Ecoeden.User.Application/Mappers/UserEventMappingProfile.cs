using AutoMapper;
using Contracts.Events;
using Ecoeden.User.Domain.Entities;

namespace Ecoeden.User.Application.Mappers
{
    public class UserEventMappingProfile : Profile
    {
        public UserEventMappingProfile()
        {
            CreateMap<ApplicationUser, UserInvitationSent>()
                .ForMember(s => s.UserId, o => o.MapFrom(d => d.Id));
        }
    }
}
