namespace CollabParty.Application.Common.Dtos.Avatar;

public class LockedAvatarDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string ImageUrl { get; set; }
    public bool IsUnlocked { get; set; }
    public int Tier { get; set; }
    public int UnlockCost { get; set; }
    public int UnlockLevel { get; set; }
}