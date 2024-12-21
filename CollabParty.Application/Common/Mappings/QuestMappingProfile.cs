using AutoMapper;
using CollabParty.Application.Common.Dtos.Quest;
using CollabParty.Application.Common.Dtos.User;
using CollabParty.Application.Common.Dtos.Avatar;
using System.Linq;
using CollabParty.Application.Common.Dtos.Member;

namespace CollabParty.Application.Common.Mappings
{
    public class QuestMappingProfile : Profile
    {
        public QuestMappingProfile()
        {
            // Mapping for CreateQuestRequestDto to Quest
            CreateMap<CreateQuestRequestDto, Quest>()
                .ForMember(dest => dest.PriorityLevel, opt => opt.MapFrom(src => src.PriorityLevel));

            // Mapping for Quest to QuestResponseDto
            CreateMap<Quest, QuestResponseDto>()
                .ForMember(dest => dest.TotalPartyMembers, opt =>
                    opt.MapFrom(src => src.QuestAssignments.Count))
                .ForMember(dest => dest.PartyMembers, opt => opt.MapFrom(src =>
                    src.QuestAssignments.Select(qa =>
                        new PartyMemberResponseDto()
                        {
                            PartyId = qa.Quest.PartyId,
                            UserId = qa.UserId,
                            Username = qa.User.UserName,
                            CurrentLevel = qa.User.CurrentLevel,
                            Avatar = qa.User.UnlockedAvatars
                                .Where(ua => ua.IsActive)
                                .Select(a => new AvatarResponseDto
                                {
                                    Id = a.Avatar.Id,
                                    Name = a.Avatar.Name,
                                    DisplayName = a.Avatar.DisplayName,
                                    ImageUrl = a.Avatar.ImageUrl
                                })
                                .FirstOrDefault()
                        }))
                )
                .ForMember(dest => dest.TotalSteps, opt =>
                    opt.MapFrom(src => src.QuestSteps.Count))
                .ForMember(dest => dest.CompletedSteps, opt =>
                    opt.MapFrom(src => src.QuestSteps.Count(q => q.IsCompleted)))
                .ForMember(dest => dest.CompletedBy, opt =>
                    opt.MapFrom(src => src.CompletedBy != null ? src.CompletedBy.UserName : null))
                .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.DueDate));


            CreateMap<Quest, QuestDetailResponseDto>()
       .ForMember(
                    dest => dest.TotalPartyMembers, opt =>
                        opt.MapFrom(src => src.Party.PartyMembers.Count))
                .ForMember(dest => dest.AssignedMembers, opt => opt.MapFrom(src =>
                    src.QuestAssignments.Select(qa =>
                        new PartyMemberResponseDto()
                        {
                            PartyId = qa.Quest.PartyId,
                            UserId = qa.UserId,
                            Username = qa.User.UserName,
                            CurrentLevel = qa.User.CurrentLevel,
                            Avatar = qa.User.UnlockedAvatars
                                .Where(ua => ua.IsActive)
                                .Select(a => new AvatarResponseDto
                                {
                                    Id = a.Avatar.Id,
                                    Name = a.Avatar.Name,
                                    DisplayName = a.Avatar.DisplayName,
                                    ImageUrl = a.Avatar.ImageUrl
                                })
                                .FirstOrDefault()
                        }))
                );
        }
    }
}