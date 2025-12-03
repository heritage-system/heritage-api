using Cultural_Heritage_System.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cultural_Heritage_System.Models
{
    public class SubscriptionPayment : BaseEntity<int>
    {
        [Required]
        [ForeignKey("Subscription")]
        [Column("subscription_id")]
        public int SubscriptionId { get; set; }
        public Subscription Subscription { get; set; }

        // orderCode bạn tạo để gửi sang PayOS
        [Required]
        [Column("order_code")]
        public long OrderCode { get; set; }

        // Link ID PayOS trả về
        [Column("payment_link_id")]
        public string? PaymentLinkId { get; set; }

        [Required]
        [Column("amount", TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [Column("payment_status", TypeName = "nvarchar(20)")]
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.PENDING;

        // Mã giao dịch trả về sau khi thanh toán thành công
        [Column("transaction_code")]
        public string? TransactionCode { get; set; }

        //thời gian thanh toán thành công
        [Column("paid_at")]
        public DateTime? PaidAt { get; set; }
        //link thanh toán cho frontend

        [Column("payment_link_url")]
        public string? PaymentLinkUrl { get; set; }

        [Column("checkout_url")]
        public string? CheckoutUrl { get; set; }  // URL để redirect user đến trang thanh toán

        [Column("qr_code")]
        public string? QrCode { get; set; }  // QR code để quét thanh toán

        [Column("payment_method", TypeName = "nvarchar(50)")]
        public string? PaymentMethod { get; set; }  // ATM, CREDIT_CARD, MOMO, etc.

        [Column("canceled_at")]
        public DateTime? CanceledAt { get; set; }  // Thời điểm hủy thanh toán

        [Column("cancel_reason")]
        public string? CancelReason { get; set; }  // Lý do hủy

        [Column("webhook_received_at")]
        public DateTime? WebhookReceivedAt { get; set; }  // Thời điểm nhận webhook từ PayOS

        [Column("payment_description")]
        public string? PaymentDescription { get; set; }  // Mô tả thanh toán
    }

}
