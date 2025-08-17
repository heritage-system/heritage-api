namespace Cultural_Heritage_System.Dtos.Request
{
    public class Verify2FARequest
    {
        public string PhoneOrEmail { get; set; }
        public string Code { get; set; } = string.Empty;
    }
}
