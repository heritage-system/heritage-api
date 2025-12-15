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

        public static readonly ErrorCode USER_NOT_EXISTED =
            new(404, "Người dùng không tồn tại", HttpStatusCode.NotFound);

        public static readonly ErrorCode USER_EXISTED =
            new(404, "Người dùng đã tồn tại", HttpStatusCode.NotFound);

        public static readonly ErrorCode UNAUTHORIZED =
            new(401, "Chưa xác thực", HttpStatusCode.Unauthorized);

        public static readonly ErrorCode ROLE_EXISTED =
            new(400, "Vai trò đã tồn tại", HttpStatusCode.BadRequest);

        public static readonly ErrorCode ROLE_NOT_EXISTED =
            new(404, "Vai trò không tồn tại", HttpStatusCode.NotFound);

        public static readonly ErrorCode FORBIDDEN =
            new(403, "Không có quyền truy cập", HttpStatusCode.Forbidden);

        public static readonly ErrorCode ACCOUNT_LOCKED =
            new(403, "Tài khoản đã bị khóa", HttpStatusCode.Forbidden);

        public static readonly ErrorCode INVALID_DATE_OF_BIRTH =
            new(400, "Ngày sinh phải nhỏ hơn thời điểm hiện tại", HttpStatusCode.BadRequest);

        public static readonly ErrorCode LICENSENUMBER_EXISTED =
            new(400, "Số giấy phép đã tồn tại", HttpStatusCode.BadRequest);

        public static readonly ErrorCode SPECIALTY_NOT_FOUND =
            new(404, "Chuyên môn không tồn tại", HttpStatusCode.NotFound);

        public static readonly ErrorCode SPECIALTY_EXISTED =
            new(400, "Chuyên môn đã tồn tại", HttpStatusCode.BadRequest);

        public static readonly ErrorCode TWO_FACTOR_SECRET_NOT_SET =
            new(400, "Tài khoản chưa được cấu hình xác thực hai yếu tố", HttpStatusCode.BadRequest);

        public static readonly ErrorCode INVALID_2FA_CODE =
            new(401, "Mã xác thực không hợp lệ hoặc đã hết hạn", HttpStatusCode.Unauthorized);

        public static readonly ErrorCode FILE_INVALID =
            new(400, "Tệp không hợp lệ", HttpStatusCode.BadRequest);

        public static readonly ErrorCode OTP_NOT_TRUE =
            new(400, "Mã OTP không chính xác", HttpStatusCode.BadRequest);

        public static readonly ErrorCode ATTEMPTS_OVER_LIMIT =
            new(400, "Đã vượt quá số lần thử cho phép", HttpStatusCode.BadRequest);

        public static readonly ErrorCode EXPIRED_OTP =
            new(400, "Mã OTP đã hết hạn", HttpStatusCode.BadRequest);

        public static readonly ErrorCode TAG_EXISTED =
            new(400, "Thẻ đã tồn tại", HttpStatusCode.BadRequest);

        public static readonly ErrorCode TAG_NOT_EXISTED =
            new(404, "Thẻ không tồn tại", HttpStatusCode.NotFound);

        public static readonly ErrorCode INVALID_TAG_NAME =
            new(400, "Tên thẻ không hợp lệ", HttpStatusCode.BadRequest);

        public static readonly ErrorCode CATEGORY_EXISTED =
            new(400, "Danh mục đã tồn tại", HttpStatusCode.BadRequest);

        public static readonly ErrorCode CATEGORY_NOT_EXISTED =
            new(404, "Danh mục không tồn tại", HttpStatusCode.NotFound);

        public static readonly ErrorCode INVALID_CATEGORY_NAME =
            new(400, "Tên danh mục không hợp lệ", HttpStatusCode.BadRequest);

        public static readonly ErrorCode CATEGORY_ALREADY_USED =
            new(404, "Danh mục đang được sử dụng, không thể xóa", HttpStatusCode.NotFound);

        public static readonly ErrorCode HERITAGE_NOT_EXISTED =
            new(404, "Di sản không tồn tại", HttpStatusCode.NotFound);

        public static readonly ErrorCode CONTRIBUTOR_NOT_EXISTED =
            new(404, "Cộng tác viên không tồn tại", HttpStatusCode.NotFound);

        public static readonly ErrorCode CONTRIBUTOR_EXISTED =
            new(400, "Cộng tác viên đã tồn tại", HttpStatusCode.BadRequest);

        public static readonly ErrorCode INVALID_CONTRIBUTOR_DATA =
            new(400, "Dữ liệu cộng tác viên không hợp lệ", HttpStatusCode.BadRequest);

        public static readonly ErrorCode HERITAGE_NOT_FOUND =
            new(404, "Không tìm thấy di sản", HttpStatusCode.NotFound);

        public static readonly ErrorCode FAVORITE_ALREADY_EXISTS =
            new(400, "Di sản đã có trong danh sách yêu thích", HttpStatusCode.BadRequest);

        public static readonly ErrorCode FAVORITE_NOT_FOUND =
            new(404, "Di sản không có trong danh sách yêu thích", HttpStatusCode.NotFound);

        public static readonly ErrorCode CONTRIBUTION_NOT_EXISTED =
            new(404, "Đóng góp không tồn tại", HttpStatusCode.NotFound);

        public static readonly ErrorCode INVALID_ROLE =
            new(400, "Chỉ có thể cấp quyền cộng tác viên cho người dùng thành viên", HttpStatusCode.NotFound);

        public static readonly ErrorCode CONTRIBUTOR_DISABLED =
            new(403, "Cộng tác viên này đã bị vô hiệu hóa và không thể phê duyệt", HttpStatusCode.NotFound);

        public static readonly ErrorCode CONTRIBUTOR_IS_NOT_PREMIUM_ELIGIBLE =
            new(400, "Cộng tác viên không đủ điều kiện đăng nội dung premium", HttpStatusCode.BadRequest);

        public static readonly ErrorCode USER_NOT_PREMIUM =
            new(400, "Người dùng chưa mua dịch vụ premium", HttpStatusCode.BadRequest);

        public static readonly ErrorCode OVER_OPEN_LIMIT =
            new(400, "Người dùng đã hết lượt mở ", HttpStatusCode.BadRequest);

        public static readonly ErrorCode CONTRIBUTION_SAVE_ALREADY_EXISTS =
            new(400, "Đóng góp này đã được lưu trước đó", HttpStatusCode.BadRequest);

        public static readonly ErrorCode CONTRIBUTION_SAVE_NOT_FOUND =
            new(404, "Đóng góp này không có trong danh sách đã lưu", HttpStatusCode.NotFound);

        public static readonly ErrorCode INVALID_STATUS =
            new(403, "Trạng thái cộng tác viên không hợp lệ", HttpStatusCode.BadRequest);

        public static readonly ErrorCode REVIEW_NOT_FOUND =
            new(404, "Không tìm thấy đánh giá", HttpStatusCode.NotFound);

        public static readonly ErrorCode ACCEPTANCE_NOT_FOUND =
            new(404, "Không tìm thấy thông tin phê duyệt", HttpStatusCode.NotFound);

        public static readonly ErrorCode NOTE_REQUIRED_WHEN_REJECT =
            new(400, "Cần ghi chú khi từ chối đóng góp", HttpStatusCode.BadRequest);

        public static readonly ErrorCode QUIZ_NOT_FOUND =
            new(404, "Không tìm thấy bài quiz", HttpStatusCode.NotFound);

        public static readonly ErrorCode QUIZ_QUESTION_NOT_FOUND =
            new(404, "Không tìm thấy câu hỏi quiz", HttpStatusCode.NotFound);

        public static readonly ErrorCode PANORAMA_TOUR_NOT_FOUND =
            new(404, "Không tìm thấy tour panorama", HttpStatusCode.NotFound);

        public static readonly ErrorCode PANORAMA_SCENE_NOT_FOUND =
            new(404, "Không tìm thấy cảnh panorama", HttpStatusCode.NotFound);

        public static readonly ErrorCode SUBSCRIPTION_USAGE_NOT_FOUND =
            new(404, "Không tìm thấy thông tin sử dụng gói dịch vụ", HttpStatusCode.NotFound);

        public static readonly ErrorCode INVALID_TOKEN =
            new(400, "Mã không hợp lệ", HttpStatusCode.BadRequest);

        public static readonly ErrorCode NO_CONFIRM_EMAIL =
            new(400, "Bạn chưa xác nhận email", HttpStatusCode.BadRequest);


        public static readonly ErrorCode STREAM_NOT_STARTED =
            new(400, "Buổi phát trực tiếp chưa bắt đầu", HttpStatusCode.NotFound);
        public static readonly ErrorCode DUPLICATE_USER =
           new(409, "Người dùng đã  vào phòng trước đó", HttpStatusCode.NotFound);

        public static readonly ErrorCode EVENT_NOT_FOUND =
            new(400, "Không tìm thấy sự kiện", HttpStatusCode.NotFound);

        public static readonly ErrorCode NO_ENOUGH_POINT =
            new(400, "Không đủ điểm để đổi", HttpStatusCode.NotFound);
        public static readonly ErrorCode BEEN_KICKED =
           new(400, "NGười dùng đã bị kick ", HttpStatusCode.NotFound);
        public static readonly ErrorCode ROOM_NOT_FOUND = new ErrorCode(404, "Chưa đến thời gian mở phòng", HttpStatusCode.NotFound);


        public static readonly ErrorCode ROOM_CLOSED = new ErrorCode(403, "Phòng đã đóng", HttpStatusCode.Forbidden);
        public static readonly ErrorCode EVENT_CLOSED = new ErrorCode(403, "Event đã đóng", HttpStatusCode.Forbidden);

    }
}
