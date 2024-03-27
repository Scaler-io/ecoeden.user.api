using AutoMapper;
using Ecoeden.User.Application.Extensions;
using Ecoeden.User.Application.Helpers;
using Ecoeden.User.Domain.Entities;
using Ecoeden.User.Domain.Models.Core;
using Ecoeden.User.Domain.Models.Responses.Users;

namespace Ecoeden.User.Application.Mappers
{
    public sealed class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<ApplicationUser, UserResponse>()
                .ForMember(s => s.UserRoles, o => o.MapFrom(d => d.GetUserRoleMappings()))
                .ForMember(s => s.Permissions, o => o.MapFrom(d => d.GetUserPermissionMappings()))
                .ForMember(s => s.MetaData, o => o.MapFrom(d => new MetaData
                {
                    CreatedAt = DateTimeHelper.ConvertUtcToIst(d.CreatedAt).ToString("dd/MM/yyyy HH:mm:ss tt"),
                    UpdatedAt = DateTimeHelper.ConvertUtcToIst(d.UpdatedAt).ToString("dd/MM/yyyy HH:mm:ss tt"),
                    CreatedBy = d.CreatedBy,
                    UpdtedBy = d.UpdateBy
                }));
                
        }
    }
}
