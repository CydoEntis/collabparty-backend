using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollabParty.Domain.Entities
{
    public class QuestStep
    {
        public int Id { get; set; }
        public int QuestId { get; set; }
        public Quest Quest { get; set; }
        public int StepNumber { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}