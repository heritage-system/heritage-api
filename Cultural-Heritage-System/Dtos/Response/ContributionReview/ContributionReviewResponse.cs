using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Dtos.Response.Review
{

    public class ContributionReviewResponse
    {
        public long Id { get; set; }
        public int? UserId { get; set; }
        public string Username { get; set; }   // match frontend
        public string UserImageUrl { get; set; } // new: profile image
        public int ContributionId { get; set; }
        public string Comment { get; set; }
        public long? ParentReviewId { get; set; }
        public int Likes { get; set; }
        public bool LikedByMe { get; set; }
        public bool CreatedByMe { get; set; }
        public DateTime CreatedAt { get; set; }     
        public List<ContributionReviewResponse> Replies { get; set; } = new();   
    }
   
}

