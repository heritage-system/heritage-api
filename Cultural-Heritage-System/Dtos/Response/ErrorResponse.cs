namespace Cultural_Heritage_System.Dtos.Response
{
    public class ErrorResponse
    {
        public int Code;
        public string Message;
        public string Url;
        public DateTime Timestamp;
        public ErrorResponse() { }

        public ErrorResponse(int Code, string Message, string Url, DateTime Timestamp)
        {
            this.Code = Code;
            this.Message = Message;
            this.Url = Url;
            this.Timestamp = Timestamp;
        }
    }
}
