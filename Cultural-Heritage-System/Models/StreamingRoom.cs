using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Models
{
    public class StreamingRoom : BaseEntity<int>
    {
        [Required, Column("room_name")] public string RoomName { get; set; } = default!;
        [Column("title")] public string? Title { get; set; }
        [Column("Description")] public string? Description { get; set; }
        [Column("max_participants")] public int MaxParticipants { get; set; } = 1000;
        [Column("is_active")] public bool IsActive { get; set; } = true;

        [ForeignKey("CreatedBy"), Column("created_by")] public int CreatedByUserId { get; set; }
        public User CreatedBy { get; set; } = default!;

        public ICollection<StreamingParticipant> Participants { get; set; } = new List<StreamingParticipant>();
        public ICollection<RaiseHandRequest> RaiseHands { get; set; } = new List<RaiseHandRequest>();
        public ICollection<RoomChatMessage> ChatMessages { get; set; } = new List<RoomChatMessage>();
    }





}
