using AutoMapper;
using CollabParty.Application.Common.Mappings;

namespace Questlog.Api.Mappings;

public class MappingConfig
{
    public static MapperConfiguration RegisterMappings()
    {
        return new MapperConfiguration(cfg =>

        {
            cfg.AddProfile(new AuthMappingProfile());
            cfg.AddProfile(new UserMappingProfile());
            cfg.AddProfile(new AvatarMappingProfile());
            cfg.AddProfile(new PartyMappingProfile());
            cfg.AddProfile(new PartyMemberMappingProfile());
            cfg.AddProfile(new QuestMappingProfile());
            cfg.AddProfile(new QuestStepMappingProfile());
            cfg.AddProfile(new QuestCommentMappingProfile());
            cfg.AddProfile(new QuestFileMappingProfile());

        });
    }
}