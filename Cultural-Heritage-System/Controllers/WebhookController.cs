using Cultural_Heritage_System.Dtos.Response.Subscription;
using Cultural_Heritage_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cultural_Heritage_System.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WebhookController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly ILogger<WebhookController> _logger;

        public WebhookController(
            ISubscriptionService subscriptionService,
            ILogger<WebhookController> logger)
        {
            _subscriptionService = subscriptionService;
            _logger = logger;
        }

        /// <summary>
        /// Webhook endpoint để nhận thông báo từ PayOS
        /// PayOS sẽ gọi endpoint này khi thanh toán thành công/thất bại
        /// </summary>
        [HttpPost("payos")]
        [AllowAnonymous]
        public async Task<IActionResult> PayOSWebhook([FromBody] PayOSWebhookData webhookData)
        {
            try
            {
                _logger.LogInformation($"Received PayOS webhook for OrderCode: {webhookData.OrderCode}");

                // Log toàn bộ webhook data để debug
                _logger.LogInformation($"Webhook Data: {System.Text.Json.JsonSerializer.Serialize(webhookData)}");

                var result = await _subscriptionService.HandlePaymentWebhookAsync(webhookData);

                // Luôn return 200 để PayOS không retry liên tục
                // PayOS sẽ retry nếu nhận được status code khác 200
                return Ok(new
                {
                    success = result,
                    message = result ? "Webhook processed successfully" : "Failed to process webhook"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PayOS webhook");

                // Vẫn return 200 để PayOS không retry liên tục
                return Ok(new
                {
                    success = false,
                    message = "Error processing webhook",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Test webhook endpoint (chỉ dùng trong development)
        /// </summary>
        [HttpPost("payos/test")]
        [AllowAnonymous]
        public async Task<IActionResult> TestWebhook([FromBody] PayOSWebhookData webhookData)
        {
#if DEBUG
            return await PayOSWebhook(webhookData);
#else
        return NotFound();
#endif
        }
    }
}
