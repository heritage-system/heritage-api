using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Helpers;
using Cultural_Heritage_System.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class StreamingRoom : BaseEntity<int>, IUnsignedEntity
{
    [Required, Column("room_name")]
    public string RoomName { get; set; } = default!;

    [Column("title")]
    public string? Title { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("max_participants")]
    public int MaxParticipants { get; set; } = 1000;

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("start_at")]
    public DateTime StartAt { get; set; }

    public DateTime? ClosedAt { get; set; }

    // KHÔNG còn CreatedByUserId / User CreatedBy, chỉ dùng BaseEntity.CreatedBy (string)

    public StreamingRoomType Type { get; set; } = StreamingRoomType.UPCOMING;

    public ICollection<StreamingParticipant> Participants { get; set; }
        = new List<StreamingParticipant>();

    [Column("title_unsigned")] public string? TitleUnsigned { get; set; }

    [Column("event_id")]
    public long? EventId { get; set; }
    public Event? Event { get; set; }

    public void GenerateUnsignedFields()
    {
        TitleUnsigned = StringHelper.RemoveDiacritics(Title).ToLower();
    }
}
