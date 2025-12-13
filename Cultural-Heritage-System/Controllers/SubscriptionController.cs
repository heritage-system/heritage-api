using Cultural_Heritage_System.Dtos.Models;
using Cultural_Heritage_System.Dtos.Request.Subscription;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Subscription;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Services;
using Cultural_Heritage_System.Services.Impl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cultural_Heritage_System.Controllers
{
    [ApiController]
    [Route("api/v1/subscriptions")]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly ILogger<SubscriptionController> _logger;
        private readonly IConfiguration _configuration;

        public SubscriptionController(
            ISubscriptionService subscriptionService,
            ILogger<SubscriptionController> logger,
            IConfiguration configuration)
        {
            _subscriptionService = subscriptionService;
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Tạo subscription mới và payment link
        /// </summary>
        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreateSubscription([FromBody] CreateSubscriptionRequest request)
        {
            try
            {
                var result = await _subscriptionService.CreateSubscriptionAsync(request);

                return Ok(new ApiResponse<CreatePaymentResponse>(
                    code: 200,
                    message: "Subscription created",
                    result: result
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subscription");
                return BadRequest(new ApiResponse<CreatePaymentResponse>(
                    code: 400,
                    message: ex.Message,
                    result: null
                ));
            }
        }



        /// <summary>
        /// Lấy subscription đang active của user
        /// </summary>
        [HttpGet("active")]
        [Authorize]
        public async Task<IActionResult> GetActiveSubscription()
        {
            try
            {
                var subscription = await _subscriptionService.GetActiveSubscriptionAsync();

                if (subscription == null)
                    return NotFound(new ApiResponse<object>(code: 404, message: "No active subscription found"));

                return Ok(new ApiResponse<object>(
                    code: 200,
                    message: "Active subscription retrieved",
                    result: subscription
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active subscription");
                return BadRequest(new ApiResponse<object>(
                    code: 400,
                    message: ex.Message
                ));
            }
        }

        /// <summary>
        /// Hủy subscription
        /// </summary>
        [HttpPost("{subscriptionId}/cancel")]
        [Authorize]
        public async Task<IActionResult> CancelSubscription(int subscriptionId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");

                if (userId == 0)
                    return Unauthorized(new ApiResponse<object>(code: 401, message: "Unauthorized"));

                var result = await _subscriptionService.CancelSubscriptionAsync(userId, subscriptionId);

                if (!result)
                    return BadRequest(new ApiResponse<object>(code: 400, message: "Cannot cancel subscription"));

                return Ok(new ApiResponse<object>(
                    code: 200,
                    message: "Subscription cancelled successfully",
                    result: true
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling subscription");
                return BadRequest(new ApiResponse<object>(
                    code: 400,
                    message: ex.Message
                ));
            }
        }


        /// <summary>
        /// Kiểm tra trạng thái payment từ database
        /// </summary>
        [HttpGet("payment/{orderCode}")]
        [Authorize]
        public async Task<IActionResult> GetPaymentStatus(long orderCode)
        {
            try
            {
                var payment = await _subscriptionService.GetPaymentByOrderCodeAsync(orderCode);

                if (payment == null)
                    return NotFound(new ApiResponse<object>(code: 404, message: "Payment not found"));

                return Ok(new ApiResponse<object>(
                    code: 200,
                    message: "Payment retrieved",
                    result: new
                    {
                        orderCode = payment.OrderCode,
                        status = payment.PaymentStatus.ToString(),
                        amount = payment.Amount,
                        paidAt = payment.PaidAt,
                        transactionCode = payment.TransactionCode,
                        checkoutUrl = payment.CheckoutUrl,
                        qrCode = payment.QrCode
                    }
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payment status");
                return BadRequest(new ApiResponse<object>(
                    code: 400,
                    message: ex.Message
                ));
            }
        }

        /// <summary>
        /// Kiểm tra và cập nhật trạng thái payment từ PayOS
        /// </summary>
        [HttpPost("payment/{orderCode}/check")]
        [Authorize]
        public async Task<IActionResult> CheckPaymentStatus(long orderCode)
        {
            try
            {
                // Kiểm tra payment có tồn tại không
                var payment = await _subscriptionService.GetPaymentByOrderCodeAsync(orderCode);
                if (payment == null)
                    return NotFound(new ApiResponse<object>(code: 404, message: "Payment not found"));

                // Kiểm tra quyền truy cập - chỉ user sở hữu payment mới được check
                var userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
                if (userId == 0)
                    return Unauthorized(new ApiResponse<object>(code: 401, message: "Unauthorized"));

                // Lấy subscription từ payment để kiểm tra userId
                var subscription = await _subscriptionService.GetSubscriptionByIdAsync(payment.SubscriptionId);
                if (subscription == null)
                    return NotFound(new ApiResponse<object>(code: 404, message: "Subscription not found"));

                // Kiểm tra xem payment có thuộc về user không
                if (subscription.UserId != userId)
                    return Forbid("You don't have permission to check this payment");

                // Gọi PayOS để check và update status
                var result = await _subscriptionService.CheckPaymentStatusAsync(orderCode);

                // Lấy lại payment sau khi update
                var updatedPayment = await _subscriptionService.GetPaymentByOrderCodeAsync(orderCode);

                return Ok(new ApiResponse<object>(
                    code: 200,
                    message: result ? "Payment status updated successfully" : "Payment status checked",
                    result: new
                    {
                        orderCode = updatedPayment.OrderCode,
                        status = updatedPayment.PaymentStatus.ToString(),
                        amount = updatedPayment.Amount,
                        paidAt = updatedPayment.PaidAt,
                        transactionCode = updatedPayment.TransactionCode,
                        isUpdated = result
                    }
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking payment status");
                return BadRequest(new ApiResponse<object>(
                    code: 400,
                    message: ex.Message
                ));
            }
        }

        /// <summary>
        /// Endpoint xử lý return URL từ PayOS sau khi thanh toán
        /// </summary>
        [HttpGet("payment/return")]
        [AllowAnonymous]
        public async Task<IActionResult> PaymentReturn([FromQuery] long orderCode, [FromQuery] string? status)
        {
            try
            {
                _logger.LogInformation($"Payment return callback - OrderCode: {orderCode}, Status: {status}");

                // Kiểm tra và cập nhật trạng thái payment
                var result = await _subscriptionService.CheckPaymentStatusAsync(orderCode);

                var payment = await _subscriptionService.GetPaymentByOrderCodeAsync(orderCode);
                if (payment == null)
                {
                    return Redirect($"{_configuration["BaseUrl:FEUrl"]}{_configuration["PayOS:ReturnUrl"]}?error=payment_not_found");
                }

                // Redirect về frontend với thông tin payment
                var frontendUrl = $"{ _configuration["BaseUrl:FEUrl"] }{_configuration["PayOS:ReturnUrl"]}";
                var redirectUrl = $"{frontendUrl}?orderCode={orderCode}&status={payment.PaymentStatus}";

                return Redirect(redirectUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment return");
                var frontendUrl = $"{_configuration["BaseUrl:FEUrl"]}{_configuration["PayOS:ReturnUrl"]}";
                return Redirect($"{frontendUrl}?error=processing_error");
            }
        }

        /// <summary>
        /// Lấy subscription đang active của một user (theo userId)
        /// </summary>
        [HttpGet("SubscriptionByUserId")]
        [Authorize]
        public async Task<IActionResult> GetSubscriptionByUserId()
        {
            try
            {
                var subscription = await _subscriptionService.GetSubscriptionsByUserIdAsync();

                if (subscription == null)
                    return NotFound(new ApiResponse<object>(
                        code: 404,
                        message: "No active subscription found for this user"
                    ));

                return Ok(new ApiResponse<object>(
                    code: 200,
                    message: "Subscription retrieved successfully",
                    result: subscription
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription by user id");
                return BadRequest(new ApiResponse<object>(
                    code: 400,
                    message: ex.Message
                ));
            }
        }

        [HttpGet]
        //[AllowAnonymous]
        public async Task<ApiResponse<IEnumerable<SubscriptionResponse>>> GetAll()
        {
            var data = await _subscriptionService.GetAllSubscriptionsAsync();

            return new ApiResponse<IEnumerable<SubscriptionResponse>>(
                code : 200,
                message: "Get all subscriptions successfully",
                result: data
            );
        }

    }
}
