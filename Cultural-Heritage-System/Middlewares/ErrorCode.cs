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
        public static readonly ErrorCode TWO_FACTOR_SECRET_NOT_SET = new ErrorCode(400, "Two-factor authentication is not configured for this account.", HttpStatusCode.BadRequest);
        public static readonly ErrorCode INVALID_2FA_CODE = new ErrorCode(401, "The verification code is invalid or has expired.", HttpStatusCode.Unauthorized);
        public static readonly ErrorCode FILE_INVALID = new ErrorCode(400, "File invalid", HttpStatusCode.BadRequest);
        public static readonly ErrorCode OTP_NOT_TRUE = new ErrorCode(400, "Otp not true", HttpStatusCode.BadRequest);
        public static readonly ErrorCode ATTEMPTS_OVER_LIMIT = new ErrorCode(400, "Attempts was over limit", HttpStatusCode.BadRequest);
        public static readonly ErrorCode EXPIRED_OTP = new ErrorCode(400, "Expired otp", HttpStatusCode.BadRequest);

        public static readonly ErrorCode TAG_EXISTED = new ErrorCode(400, "Tag already exists", HttpStatusCode.BadRequest);
        public static readonly ErrorCode TAG_NOT_EXISTED = new ErrorCode(404, "Tag not existed", HttpStatusCode.NotFound);
        public static readonly ErrorCode INVALID_TAG_NAME = new ErrorCode(400, "Invalid tag name", HttpStatusCode.BadRequest);

        public static readonly ErrorCode CATEGORY_EXISTED = new ErrorCode(400, "Category already exists", HttpStatusCode.BadRequest);
        public static readonly ErrorCode CATEGORY_NOT_EXISTED = new ErrorCode(404, "Category not existed", HttpStatusCode.NotFound);
        public static readonly ErrorCode INVALID_CATEGORY_NAME = new ErrorCode(400, "Invalid CATEGORY name", HttpStatusCode.BadRequest);
        public static readonly ErrorCode CATEGORY_ALREADY_USED = new ErrorCode(404, "Category already used cannot delete", HttpStatusCode.NotFound);

        public static readonly ErrorCode HERITAGE_NOT_EXISTED = new ErrorCode(404, "Heritage not existed", HttpStatusCode.NotFound);
        public static readonly ErrorCode CONTRIBUTOR_NOT_EXISTED = new ErrorCode(404, "Contributor not existed", HttpStatusCode.NotFound);
        public static readonly ErrorCode CONTRIBUTOR_EXISTED = new ErrorCode(400, "Contributor already exists", HttpStatusCode.BadRequest);
        public static readonly ErrorCode INVALID_CONTRIBUTOR_DATA = new ErrorCode(400, "Invalid contributor data", HttpStatusCode.BadRequest);
        public static readonly ErrorCode HERITAGE_NOT_FOUND = new ErrorCode(404, "Heritage not found", HttpStatusCode.NotFound);
        public static readonly ErrorCode FAVORITE_ALREADY_EXISTS = new ErrorCode(400, "Heritage is already in favorite", HttpStatusCode.BadRequest);
        public static readonly ErrorCode FAVORITE_NOT_FOUND = new ErrorCode(404, "Heritage is not in favorites",HttpStatusCode.NotFound);

        public static readonly ErrorCode CONTRIBUTION_NOT_EXISTED = new ErrorCode(404, "Contribution not existed", HttpStatusCode.NotFound);
        public static readonly ErrorCode CONTRIBUTOR_IS_NOT_PREMIUM_ELIGIBLE = new ErrorCode(404, "Contributor is not premium eligible", HttpStatusCode.NotFound);
    }

}
