using AutoMapper;
using CollabParty.Application.Common.Dtos.QuestComments;
using CollabParty.Application.Common.Dtos.QuestFiles;
using CollabParty.Domain.Entities;

namespace CollabParty.Application.Common.Mappings;

public class QuestFileMappingProfile: Profile
{
    public QuestFileMappingProfile()
    {
        CreateMap<QuestFile, QuestFilesResponseDto>()
            .ForMember(dest => dest.PartyMember, opt => opt.MapFrom(src => src.User));
    }
}