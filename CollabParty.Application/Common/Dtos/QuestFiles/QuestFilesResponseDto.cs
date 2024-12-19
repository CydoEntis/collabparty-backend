using CollabParty.Application.Common.Dtos.Member;
using CollabParty.Application.Common.Dtos.User;

namespace CollabParty.Application.Common.Dtos.QuestFiles;

public class QuestFilesResponseDto
{
    public int Id { get; set; }
    public int QuestId { get; set; }
    public UserDtoResponse PartyMember { get; set; }
    public string FilePath { get; set; }
    public string FileName { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}