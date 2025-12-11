namespace Cultural_Heritage_System.Dtos.Response.Subscription
{
    public class SubscriptionPaymentDto
    {
        public int Id { get; set; }

        public long OrderCode { get; set; }
        public string? PaymentLinkId { get; set; }

        public decimal Amount { get; set; }
        public string PaymentStatus { get; set; }

        public string? TransactionCode { get; set; }
        public DateTime? PaidAt { get; set; }

        public string? PaymentLinkUrl { get; set; }
        public string? CheckoutUrl { get; set; }
        public string? QrCode { get; set; }

        public string? PaymentMethod { get; set; }

        public DateTime? CanceledAt { get; set; }
        public string? CancelReason { get; set; }

        public DateTime? WebhookReceivedAt { get; set; }

        public string? PaymentDescription { get; set; }
    }
}
