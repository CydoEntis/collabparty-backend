using AutoMapper;
using CollabParty.Application.Common.Dtos.QuestComments;
using CollabParty.Domain.Entities;

namespace CollabParty.Application.Common.Mappings;

public class QuestCommentMappingProfile : Profile
{
    public QuestCommentMappingProfile()
    {
        CreateMap<QuestComment, QuestCommentResponseDto>()
            .ForMember(dest => dest.PartyMember, opt => opt.MapFrom(src => src.User));

        CreateMap<AddCommentRequestDto, QuestComment>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
    }
}