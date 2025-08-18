using System.Net;

namespace Cultural_Heritage_System.Middlewares
{
    public class ErrorCode
    {
        public int Code { get; }
        public string Message { get; }
        public HttpStatusCode StatusCode { get; }

        private ErrorCode(int Code, string Message, HttpStatusCode StatusCode)
        {
            this.Code = Code;
            this.Message = Message;
            this.StatusCode = StatusCode;
        }

        public static readonly ErrorCode USER_NOT_EXISTED = new ErrorCode(404, "User not existed", HttpStatusCode.NotFound);
        public static readonly ErrorCode USER_EXISTED = new ErrorCode(404, "User existed", HttpStatusCode.NotFound);
        public static readonly ErrorCode UNAUTHORIZED = new ErrorCode(401, "Unauthorized", HttpStatusCode.Unauthorized);
        public static readonly ErrorCode ROLE_EXISTED = new ErrorCode(400, "Role existed", HttpStatusCode.BadRequest);
        public static readonly ErrorCode ROLE_NOT_EXISTED = new ErrorCode(404, "Role not existed", HttpStatusCode.NotFound);
        public static readonly ErrorCode FORBIDDEN = new ErrorCode(403, "Forbidden", HttpStatusCode.Forbidden);
        public static readonly ErrorCode ACCOUNT_LOCKED = new ErrorCode(403, "Account locked", HttpStatusCode.Forbidden);
        public static readonly ErrorCode INVALID_DATE_OF_BIRTH = new ErrorCode(400, "Date of birth must be less than current time", HttpStatusCode.BadRequest);
        public static readonly ErrorCode LICENSENUMBER_EXISTED = new ErrorCode(400, "License number already exists", HttpStatusCode.BadRequest);
        public static readonly ErrorCode SPECIALTY_NOT_FOUND = new ErrorCode(404, "Specialty not found", HttpStatusCode.NotFound);
        public static readonly ErrorCode SPECIALTY_EXISTED = new ErrorCode(400, "Specialty existed", HttpStatusCode.BadRequest);
        public static readonly ErrorCode TWO_FACTOR_SECRET_NOT_SET = new ErrorCode(400,"Two-factor authentication is not configured for this account.",HttpStatusCode.BadRequest);
        public static readonly ErrorCode INVALID_2FA_CODE = new ErrorCode(401, "The verification code is invalid or has expired.", HttpStatusCode.Unauthorized);
        public static readonly ErrorCode FILE_INVALID = new ErrorCode(400, "File invalid", HttpStatusCode.BadRequest);
        public static readonly ErrorCode OTP_NOT_TRUE = new ErrorCode(400, "Otp not true", HttpStatusCode.BadRequest);
        public static readonly ErrorCode ATTEMPTS_OVER_LIMIT = new ErrorCode(400, "Attempts was over limit", HttpStatusCode.BadRequest);
        public static readonly ErrorCode EXPIRED_OTP = new ErrorCode(400, "Expired otp", HttpStatusCode.BadRequest);
    }

}
