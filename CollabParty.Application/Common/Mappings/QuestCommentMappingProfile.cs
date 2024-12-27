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

        // Mapping AddCommentRequestDto to QuestComment
        CreateMap<AddCommentRequestDto, QuestComment>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
    }
}