using System.Text.Json.Serialization;

namespace Cultural_Heritage_System.Dtos.Response.Dashboard
{
    public class ExecutiveDashboardResponse
    {
        [JsonPropertyName("users")]
        public DashboardUserStats Users { get; set; } = new();

        [JsonPropertyName("heritage")]
        public DashboardHeritageStats Heritage { get; set; } = new();

        [JsonPropertyName("contributions")]
        public DashboardContributionStats Contributions { get; set; } = new();
    }

    public class DashboardUserStats
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("employees")]
        public int Employees { get; set; }

        [JsonPropertyName("contributors")]
        public int Contributors { get; set; }
    }

    public class DashboardHeritageStats
    {
        [JsonPropertyName("totalHeritage")]
        public int TotalHeritage { get; set; }

        [JsonPropertyName("categories")]
        public int Categories { get; set; }

        [JsonPropertyName("quizzes")]
        public int Quizzes { get; set; }
    }

    public class DashboardContributionStats
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("pending")]
        public int Pending { get; set; }

        [JsonPropertyName("approved")]
        public int Approved { get; set; }

        [JsonPropertyName("rejected")]
        public int Rejected { get; set; }
    }
}

