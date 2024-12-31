namespace CollabParty.Application.Common.Dtos.Member;

public class MemberUpdateDto
{
    public string Id { get; set; }
    public int Role { get; set; }
    public bool Delete { get; set; }
}