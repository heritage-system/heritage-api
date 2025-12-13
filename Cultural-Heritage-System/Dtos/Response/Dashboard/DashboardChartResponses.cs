using System.Text.Json.Serialization;

namespace Cultural_Heritage_System.Dtos.Response.Dashboard
{
    public class HeritagePopularItem
    {
        [JsonPropertyName("heritageId")]
        public long HeritageId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("favorites")]
        public int Favorites { get; set; }
    }

    public class ContributionPopularItem
    {
        [JsonPropertyName("contributionId")]
        public int ContributionId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("views")]
        public int Views { get; set; }
    }

    public class HeritageProvinceBucket
    {
        [JsonPropertyName("province")]
        public string Province { get; set; } = string.Empty;

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }

    public class ContributionTrendPoint
    {
        [JsonPropertyName("period")]
        public string Period { get; set; } = string.Empty; // e.g., "2025-01"

        [JsonPropertyName("contributions")]
        public int Total { get; set; }

        [JsonPropertyName("approved")]
        public int Approved { get; set; }

        [JsonPropertyName("pending")]
        public int Pending { get; set; }

        [JsonPropertyName("rejected")]
        public int Rejected { get; set; }
    }

    public class EngagementByCategoryItem
    {
        [JsonPropertyName("category")]
        public string Category { get; set; } = string.Empty;

        [JsonPropertyName("views")]
        public int Views { get; set; }

        [JsonPropertyName("saves")]
        public int Saves { get; set; }

        [JsonPropertyName("comments")]
        public int Comments { get; set; }
    }

    public class YearlyGrowthItem
    {
        [JsonPropertyName("year")]
        public int Year { get; set; }

        [JsonPropertyName("heritage")]
        public int Heritage { get; set; }

        [JsonPropertyName("users")]
        public int Users { get; set; }

        [JsonPropertyName("contributions")]
        public int Contributions { get; set; }
    }
}

