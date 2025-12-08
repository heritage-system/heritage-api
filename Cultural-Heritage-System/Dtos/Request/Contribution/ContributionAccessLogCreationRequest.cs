using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Cultural_Heritage_System.Common;

namespace Cultural_Heritage_System.Models
{
    public class ContributionAccessLogCreationRequest
    {          
        public int ContributionId { get; set; }    

        // -------------------------------------------
        // UUID phía client lưu trong localStorage
        // → phân biệt lượt đọc anonymous
        // -------------------------------------------      
        public string? ClientUuid { get; set; }

        // -------------------------------------------
        // Thời gian xem (ms)
        // → dùng tính điểm / detect spam
        // -------------------------------------------      
        public long? TimeSpentMs { get; set; }

        
        // -------------------------------------------
        // Mức độ kéo trang (0–1)
        // → 1 = đọc hết
        // -------------------------------------------  
        public double? ScrollDepth { get; set; }

        // -------------------------------------------
        // Tốc độ kéo trung bình
        // → tốc độ quá nhanh = bot
        // -------------------------------------------     
        public double? ScrollVelocity { get; set; }

        // -------------------------------------------
        // Tổng số tương tác: click, mở ảnh, highlight…
        // → càng nhiều → càng là người thật
        // -------------------------------------------    
        public int? Interactions { get; set; }
     
    }

}
