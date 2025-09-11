using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Dtos.Response.Review
{

    public class ReviewResponse
    {
        public long Id { get; set; }

        public int? UserId { get; set; }
        public string Username { get; set; }   // match frontend
        public string UserImageUrl { get; set; } // new: profile image


        public long HeritageId { get; set; }
        public string Comment { get; set; }
        public long? ParentReviewId { get; set; }

        public int Likes { get; set; }
        public bool LikedByMe { get; set; }
        public bool CreatedByMe { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public List<ReviewResponse> Replies { get; set; } = new();
        public List<ReviewMediaResponse> ReviewMedias { get; set; } = new(); // match frontend naming
    }
    public class ReviewMediaResponse
    {
        public long Id { get; set; }
        public string Url { get; set; }
        public MediaType Type { get; set; }
    }
}

