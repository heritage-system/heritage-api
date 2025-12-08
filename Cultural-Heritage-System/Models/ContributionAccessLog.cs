using Cultural_Heritage_System.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Models
{
    public class ContributionAccessLog : BaseEntity<long>
    {
        // -------------------------------------------
        // Người xem contribution (nếu đã đăng nhập)
        // -------------------------------------------
        [ForeignKey("User")]
        [Column("user_id")]
        public int? UserId { get; set; }
        public User? User { get; set; }


        // -------------------------------------------
        // Contribution được xem
        // -------------------------------------------
        [Required]
        [ForeignKey("Contribution")]
        [Column("contribution_id")]
        public int ContributionId { get; set; }
        public Contribution Contribution { get; set; }


        // -------------------------------------------
        // Thuộc subscription nào (nếu user đang dùng gói)
        // -------------------------------------------
        [ForeignKey("Subscription")]
        [Column("subscription_id")]
        public int? SubscriptionId { get; set; }
        public Subscription? Subscription { get; set; }


        // -------------------------------------------
        // UUID phía client lưu trong localStorage
        // → phân biệt lượt đọc anonymous
        // -------------------------------------------
        [Column("client_uuid")]
        public string? ClientUuid { get; set; }
      

        // -------------------------------------------
        // Thời gian xem (ms)
        // → dùng tính điểm / detect spam
        // -------------------------------------------
        [Column("time_spent_ms")]
        public long? TimeSpentMs { get; set; }


        // -------------------------------------------
        // Địa chỉ IP
        // → nhận dạng network, fallback khi thiếu fingerprint
        // -------------------------------------------
        [Column("ip_address")]
        public string? IpAddress { get; set; }


        // -------------------------------------------
        // Mức độ kéo trang (0–1)
        // → 1 = đọc hết
        // -------------------------------------------
        [Column("scroll_depth")]
        public double? ScrollDepth { get; set; }


        // -------------------------------------------
        // Tốc độ kéo trung bình
        // → tốc độ quá nhanh = bot
        // -------------------------------------------
        [Column("scroll_velocity")]
        public double? ScrollVelocity { get; set; }


        // -------------------------------------------
        // Tổng số tương tác: click, mở ảnh, highlight…
        // → càng nhiều → càng là người thật
        // -------------------------------------------
        [Column("interactions")]
        public int? Interactions { get; set; }


        // -------------------------------------------
        // Đã xử lý tính điểm chưa?
        // → tránh tính lại khi chạy Background Worker
        // -------------------------------------------
        [Column("processed")]
        public bool? Processed { get; set; }


        // -------------------------------------------
        // Điểm tính được từ lượt xem này
        // -------------------------------------------
        [Column("calculated_points")]
        public int? CalculatedPoints { get; set; }


        // -------------------------------------------
        // Đánh dấu spam: đọc quá nhanh, auto-scroll…
        // -------------------------------------------
        [Column("flagged_as_spam")]
        public bool FlaggedAsSpam { get; set; } = false;
    }
}
