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
        public static readonly ErrorCode FORBIDDEN = new ErrorCode(403, "Forbitdden", HttpStatusCode.Forbidden);
        public static readonly ErrorCode ACCOUNT_LOCKED = new ErrorCode(403, "Account locked", HttpStatusCode.Forbidden);
        public static readonly ErrorCode INVALID_DATE_OF_BIRTH = new ErrorCode(400, "Date of birth must be less than current time", HttpStatusCode.BadRequest);
        public static readonly ErrorCode LICENSENUMBER_EXISTED = new ErrorCode(400, "License number already exists", HttpStatusCode.BadRequest);
        public static readonly ErrorCode SPECIALTY_NOT_FOUND = new ErrorCode(404, "Specialty not found", HttpStatusCode.NotFound);
        public static readonly ErrorCode SPECIALTY_EXISTED = new ErrorCode(400, "Specialty existed", HttpStatusCode.BadRequest);
        public static readonly ErrorCode TWO_FACTOR_SECRET_NOT_SET = new ErrorCode(400,"Two-factor authentication is not configured for this account.",HttpStatusCode.BadRequest);
        public static readonly ErrorCode INVALID_2FA_CODE = new ErrorCode(401, "The verification code is invalid or has expired.", HttpStatusCode.Unauthorized);
        public static readonly ErrorCode FILE_INVALID = new ErrorCode(400, "File invalid", HttpStatusCode.BadRequest);

        public static readonly ErrorCode PATIENT_NOT_FOUND = new ErrorCode(404, "Patient not found", HttpStatusCode.NotFound);
        public static readonly ErrorCode DOCTOR_NOT_FOUND = new ErrorCode(404, "Doctor not found or not available", HttpStatusCode.NotFound);
        public static readonly ErrorCode TIMESLOT_NOT_AVAILABLE = new ErrorCode(400, "Time slot not available", HttpStatusCode.BadRequest);
        public static readonly ErrorCode INVALID_APPOINTMENT_DATE = new ErrorCode(400, "Invalid appointment date", HttpStatusCode.BadRequest);
        public static readonly ErrorCode PAST_APPOINTMENT_DATE = new ErrorCode(400, "Cannot schedule appointment in the past", HttpStatusCode.BadRequest);
        public static readonly ErrorCode SERVICE_PACKAGE_NOT_FOUND = new ErrorCode(404, "Service package not found", HttpStatusCode.NotFound);
        public static readonly ErrorCode APPOINTMENT_CONFLICT = new ErrorCode(409, "You already have an appointment at this time", HttpStatusCode.Conflict);
        public static readonly ErrorCode TIMESLOT_DOCTOR_MISMATCH = new ErrorCode(400, "Time slot does not match the selected doctor and date", HttpStatusCode.BadRequest);
        public static readonly ErrorCode APPOINTMENT_NOT_FOUND = new ErrorCode(404, "Appointment not found", HttpStatusCode.NotFound);
        public static readonly ErrorCode APPOINTMENT_CANNOT_BE_CANCELLED = new ErrorCode(400, "Appointment cannot be cancelled. Only scheduled appointments can be cancelled at least 24 hours in advance", HttpStatusCode.BadRequest);
        public static readonly ErrorCode APPOINTMENT_CANNOT_BE_RESCHEDULED = new ErrorCode(400, "Appointment cannot be rescheduled. Only scheduled appointments can be rescheduled at least 24 hours in advance", HttpStatusCode.BadRequest);
        public static readonly ErrorCode INVALID_DAYS_AHEAD_RANGE = new ErrorCode(400, "Days ahead must be between 1 and 365", HttpStatusCode.BadRequest);

        public static readonly ErrorCode INVALID_DATE_RANGE = new ErrorCode(400,"Start date cannot be greater than end date", HttpStatusCode.BadRequest);

        public static readonly ErrorCode FUTURE_APPOINTMENT_ARRIVAL = new ErrorCode(400,"Cannot mark patient as arrived for future appointments", HttpStatusCode.BadRequest);

        public static readonly ErrorCode INVALID_APPOINTMENT_STATUS_FOR_COMPLETION = new ErrorCode(400, "Appointment cannot be completed. Invalid status.", HttpStatusCode.BadRequest);

        public static readonly ErrorCode LEAVE_DATE_IN_PAST = new ErrorCode(400,"Leave date must be in the future", HttpStatusCode.BadRequest);

        public static readonly ErrorCode LEAVE_CONFLICT = new ErrorCode(400, "Leave request already exists for this date", HttpStatusCode.BadRequest);
    }

}
