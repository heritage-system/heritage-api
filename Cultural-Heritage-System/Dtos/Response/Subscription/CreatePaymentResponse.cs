namespace Cultural_Heritage_System.Dtos.Response.Subscription
{
    public class CreatePaymentResponse
    {
        public int SubscriptionId { get; set; }
        public long OrderCode { get; set; }
        public string CheckoutUrl { get; set; }
        public string QrCode { get; set; }
        public decimal Amount { get; set; }
    }

}
