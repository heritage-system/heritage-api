namespace Cultural_Heritage_System.Dtos.Response
{
    public class ApiResponse<T>
    {
        public int code { get; set; }
        public string? message { get; set; }
        public T? result { get; set; }

        public ApiResponse()
        {
        }

        public ApiResponse(int code, string message)
        {
            this.code = code;
            this.message = message;
        }

        public ApiResponse(int code, string message, T result)
        {
            this.code = code;
            this.message = message;
            this.result = result;
        }
    }
    
}
