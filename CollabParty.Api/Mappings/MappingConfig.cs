using AutoMapper;

namespace CollabParty.Api.Mappings;

public class MappingConfig
{
    public static MapperConfiguration RegisterMappings()
    {
        return new MapperConfiguration(cfg => { cfg.AddProfile(new AuthMappingProfile()); });
    }
}