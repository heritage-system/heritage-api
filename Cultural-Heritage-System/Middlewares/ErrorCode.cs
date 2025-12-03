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
        public static readonly ErrorCode INVALID_ROLE = new ErrorCode(400, "Contributor rights can only be granted to Members.", HttpStatusCode.NotFound);
        public static readonly ErrorCode CONTRIBUTOR_DISABLED = new ErrorCode(403, "This contributor has been disabled and cannot be approved.", HttpStatusCode.NotFound);
        public static readonly ErrorCode CONTRIBUTOR_IS_NOT_PREMIUM_ELIGIBLE = new ErrorCode(400, "Contributor is not premium eligible", HttpStatusCode.BadRequest);
        public static readonly ErrorCode USER_NOT_PREMIUM = new ErrorCode(400, "User not buy premium service yet", HttpStatusCode.BadRequest);
        public static readonly ErrorCode OVER_OPEN_LIMIT = new ErrorCode(400, "User has no open contribution token", HttpStatusCode.BadRequest);
        public static readonly ErrorCode CONTRIBUTION_SAVE_ALREADY_EXISTS = new ErrorCode(400, "Contribution save is already existed", HttpStatusCode.BadRequest);
        public static readonly ErrorCode CONTRIBUTION_SAVE_NOT_FOUND = new ErrorCode(404, "Contribution save is not in favorites", HttpStatusCode.NotFound);
        public static readonly ErrorCode INVALID_STATUS = new ErrorCode(403, "Invalid contributor status.", HttpStatusCode.BadRequest);

        public static readonly ErrorCode REVIEW_NOT_FOUND = new ErrorCode(404, "Review not found", HttpStatusCode.NotFound);
        public static readonly ErrorCode ACCEPTANCE_NOT_FOUND = new(404, "Acceptance not found", HttpStatusCode.NotFound);
        public static readonly ErrorCode NOTE_REQUIRED_WHEN_REJECT = new(400, "Note is required when rejecting a contribution", HttpStatusCode.BadRequest);

        public static readonly ErrorCode QUIZ_NOT_FOUND = new ErrorCode(404, "Quiz not found", HttpStatusCode.NotFound);
        public static readonly ErrorCode QUIZ_QUESTION_NOT_FOUND = new ErrorCode(404, "Quiz question not found", HttpStatusCode.NotFound);

        public static readonly ErrorCode PANORAMA_TOUR_NOT_FOUND = new ErrorCode(404, "Panorama tour not found", HttpStatusCode.NotFound);
        public static readonly ErrorCode PANORAMA_SCENE_NOT_FOUND = new ErrorCode(404, "Panorama scene not found", HttpStatusCode.NotFound);

        public static readonly ErrorCode SUBSCRIPTION_USAGE_NOT_FOUND = new ErrorCode(404, " Subscription usage not found", HttpStatusCode.NotFound);

        public static readonly ErrorCode INVALID_TOKEN = new ErrorCode(400, "Mã không hợp lệ", HttpStatusCode.BadRequest);
        public static readonly ErrorCode NO_CONFIRM_EMAIL = new ErrorCode(400, "Bạn chưa xác nhận email", HttpStatusCode.BadRequest);

    }
}

