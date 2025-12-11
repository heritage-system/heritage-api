using AutoMapper;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Request.Subscription;
using Cultural_Heritage_System.Dtos.Response.Heritage;
using Cultural_Heritage_System.Dtos.Response.Subscription;
using Cultural_Heritage_System.Middlewares;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Cultural_Heritage_System.Repositories.Impl;
using Net.payOS;
using Net.payOS.Types;

namespace Cultural_Heritage_System.Services.Impl
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly ISubscriptionPaymentRepository _paymentRepository;
        private readonly IPremiumPackageRepository _packageRepository;
        private readonly ISubscriptionUsageRepository _usageRepository;
        private readonly PayOS _payOS;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SubscriptionService> _logger;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IMapper mapper;

        public SubscriptionService(
            ISubscriptionRepository subscriptionRepository,
            ISubscriptionPaymentRepository paymentRepository,
            IPremiumPackageRepository packageRepository,
            ISubscriptionUsageRepository usageRepository,
            IConfiguration configuration,
            ILogger<SubscriptionService> logger,
            IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _subscriptionRepository = subscriptionRepository;
            _paymentRepository = paymentRepository;
            _packageRepository = packageRepository;
            _usageRepository = usageRepository;
            _configuration = configuration;
            _logger = logger;
            this.mapper = mapper;

            // Khởi tạo PayOS
            _payOS = new PayOS(
                _configuration["PayOS:ClientId"],
                _configuration["PayOS:ApiKey"],
                _configuration["PayOS:ChecksumKey"]
            );
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<CreatePaymentResponse> CreateSubscriptionAsync(CreateSubscriptionRequest request)
        {
            try
            {
                // 1. Kiểm tra gói Premium có tồn tại không
                var package = await _packageRepository.GetByIdAsync(request.PackageId);
                if (package == null)
                    throw new Exception("Gói Premium không tồn tại");
                
                // Validation package
                if (!package.IsActive)
                    throw new Exception("Gói Premium không còn hoạt động");
                
                if (package.Price <= 0)
                    throw new Exception("Giá gói Premium không hợp lệ");
                
                if (package.DurationDays == null || package.DurationDays <= 0)
                    throw new Exception("Thời hạn gói Premium không hợp lệ");
                
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    throw new AppException(ErrorCode.UNAUTHORIZED);
                }

                //2.Kiểm tra user có subscription đang active không
                var activeSubscription = await GetActiveSubscriptionAsync();
                if (activeSubscription != null)
                    throw new Exception("Bạn đang có gói đăng ký đang hoạt động");

                // 3. Tạo Subscription mới với status PENDING
                var subscription = new Subscription
                {
                    UserId = (int)userId,
                    PackageId = request.PackageId,
                    StartAt = DateTime.UtcNow,
                    EndAt = DateTime.UtcNow.AddDays((double)package.DurationDays),
                    Status = SubscriptionStatus.PENDING,
                    CreatedAt = DateTime.UtcNow
                };

                await _subscriptionRepository.AddAsync(subscription);
                await _subscriptionRepository.SaveChangesAsync();

                // 5. Tạo OrderCode unique (timestamp + random để tránh trùng lặp)
                var random = new Random();
                long orderCode = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() * 1000 + random.Next(0, 999);

                // 6. Tạo Payment Link với PayOS
                var paymentData = new PaymentData(
                    orderCode: orderCode,
                    amount: (int)package.Price, 
                    description: $"Thanh toán gói {package.Name}",
                    items: new List<ItemData>
                    {
                    new ItemData(package.Name, 1, (int)package.Price)
                    },
                    cancelUrl: _configuration["PayOS:CancelUrl"],
                    returnUrl: _configuration["PayOS:ReturnUrl"]
                );
                var createPaymentResult = await _payOS.createPaymentLink(paymentData);

                // 7. Lưu thông tin payment vào database
                var payment = new SubscriptionPayment
                {
                    SubscriptionId = subscription.Id,
                    OrderCode = orderCode,
                    PaymentLinkId = createPaymentResult.paymentLinkId,
                    Amount = package.Price,
                    PaymentStatus = PaymentStatus.PENDING,
                    CheckoutUrl = createPaymentResult.checkoutUrl,
                    QrCode = createPaymentResult.qrCode,
                    PaymentDescription = $"Thanh toán gói {package.Name}",
                    CreatedAt = DateTime.UtcNow
                };

                await _paymentRepository.AddAsync(payment);
                await _paymentRepository.SaveChangesAsync();

                // 8. Return response
                return new CreatePaymentResponse
                {
                    SubscriptionId = subscription.Id,
                    OrderCode = orderCode,
                    CheckoutUrl = createPaymentResult.checkoutUrl,
                    QrCode = createPaymentResult.qrCode,
                    Amount = package.Price
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating subscription payment");
                throw;
            }
        }

        public async Task<bool> CheckPaymentStatusAsync(long orderCode)
        {
            try
            {
                // 1. Tìm payment theo OrderCode
                var payment = await _paymentRepository.GetByOrderCodeAsync(orderCode);
                if (payment == null)
                {
                    _logger.LogWarning($"Payment not found: {orderCode}");
                    return false;
                }

                // 2. Nếu đã thanh toán rồi thì không cần check lại
                if (payment.PaymentStatus == PaymentStatus.PAID)
                {
                    _logger.LogInformation($"Payment already paid: {orderCode}");
                    return true;
                }

                // 3. Gọi PayOS để lấy thông tin thanh toán
                var payosInfo = await _payOS.getPaymentLinkInformation(orderCode);

                if (payosInfo == null)
                {
                    _logger.LogWarning($"PayOS return null for orderCode {orderCode}");
                    return false;
                }

                // 4. Kiểm tra trạng thái từ PayOS
                // PayOS trả về status: "PENDING", "PAID", "CANCELLED"
                if (payosInfo.status == "PAID")
                {
                    // Nếu đã cập nhật rồi thì bỏ qua (double check)
                    if (payment.PaymentStatus == PaymentStatus.PAID)
                    {
                        _logger.LogInformation($"Payment already processed: {orderCode}");
                        return true;
                    }

                    // Cập nhật payment
                    payment.PaymentStatus = PaymentStatus.PAID;
                    payment.PaidAt = DateTime.UtcNow;
                    payment.TransactionCode = payosInfo.transactions?.FirstOrDefault()?.reference;
                    payment.WebhookReceivedAt = DateTime.UtcNow;
                    payment.UpdatedAt = DateTime.UtcNow;

                    // Lấy thông tin payment method nếu có
                    if (payosInfo.transactions != null && payosInfo.transactions.Any())
                    {
                        var transaction = payosInfo.transactions.First();
                        payment.PaymentMethod = transaction.counterAccountBankName;
                    }

                    await _paymentRepository.UpdateAsync(payment);
                    await _paymentRepository.SaveChangesAsync();

                    // 5. Cập nhật subscription status to ACTIVE
                    var subscription = await _subscriptionRepository.GetByIdAsync(payment.SubscriptionId);
                    if (subscription != null)
                    {
                        subscription.Status = SubscriptionStatus.ACTIVE;
                        subscription.UpdatedAt = DateTime.UtcNow;
                        await _subscriptionRepository.UpdateAsync(subscription);

                        // 6. Tạo subscription usage records
                        var package = await _packageRepository.GetByIdAsync(subscription.PackageId);
                        if (package?.PackageBenefits != null)
                        {
                            foreach (var benefit in package.PackageBenefits)
                            {
                                // Kiểm tra đã có chưa để tránh tạo trùng
                                var exists = await _usageRepository.ExistsAsync(subscription.Id, benefit.Benefit.BenefitName.ToString());
                                if (!exists)
                                {
                                    var usage = new SubscriptionUsage
                                    {
                                        SubscriptionId = subscription.Id,
                                        BenefitName = benefit.Benefit.BenefitName,
                                        Total = benefit.Benefit.Value,
                                        Used = 0,
                                        Status = SubscriptionStatus.ACTIVE,
                                        CreatedAt = DateTime.UtcNow
                                    };
                                    await _usageRepository.AddAsync(usage);
                                }
                            }
                            await _usageRepository.SaveChangesAsync();
                        }
                    }

                    await _subscriptionRepository.SaveChangesAsync();

                    _logger.LogInformation($"Payment status updated to PAID: {orderCode}");
                    return true;
                }
                else if (payosInfo.status == "CANCELLED")
                {
                    // Thanh toán bị hủy
                    if (payment.PaymentStatus != PaymentStatus.CANCELLED)
                    {
                        payment.PaymentStatus = PaymentStatus.CANCELLED;
                        payment.CanceledAt = DateTime.UtcNow;
                        payment.CancelReason = "Payment cancelled on PayOS";
                        payment.UpdatedAt = DateTime.UtcNow;

                        await _paymentRepository.UpdateAsync(payment);
                        await _paymentRepository.SaveChangesAsync();

                        // Cập nhật subscription status
                        var subscription = await _subscriptionRepository.GetByIdAsync(payment.SubscriptionId);
                        if (subscription != null && subscription.Status == SubscriptionStatus.PENDING)
                        {
                            subscription.Status = SubscriptionStatus.CANCELLED;
                            subscription.UpdatedAt = DateTime.UtcNow;
                            await _subscriptionRepository.UpdateAsync(subscription);
                            await _subscriptionRepository.SaveChangesAsync();
                        }

                        _logger.LogInformation($"Payment cancelled: {orderCode}");
                    }
                    return false;
                }
                else
                {
                    // Status vẫn là PENDING
                    _logger.LogInformation($"Payment still pending: {orderCode}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking payment status: {orderCode}");
                return false;
            }
        }


        public async Task<Subscription?> GetActiveSubscriptionAsync()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                throw new AppException(ErrorCode.UNAUTHORIZED);
            }

            return await _subscriptionRepository.GetActiveSubscription((int)userId);
        }

        public async Task<Subscription?> GetSubscriptionByIdAsync(int subscriptionId)
        {
            return await _subscriptionRepository.GetByIdAsync(subscriptionId);
        }

        public async Task<bool> CancelSubscriptionAsync(int userId, int subscriptionId)
        {
            var subscription = await _subscriptionRepository.GetByIdAsync(subscriptionId);

            if (subscription == null || subscription.UserId != userId)
                return false;

            if (subscription.Status != SubscriptionStatus.ACTIVE)
                return false;

            subscription.Status = SubscriptionStatus.CANCELLED;
            subscription.UpdatedAt = DateTime.UtcNow;

            await _subscriptionRepository.UpdateAsync(subscription);
            await _subscriptionRepository.SaveChangesAsync();

            return true;
        }

        public async Task<SubscriptionPayment?> GetPaymentByOrderCodeAsync(long orderCode)
        {
            return await _paymentRepository.GetByOrderCodeAsync(orderCode);
        }

        private int? GetCurrentUserId()
        {
            var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(accountIdClaim))
            {
                return null;
            }

            if (int.TryParse(accountIdClaim, out int userId))
            {
                return userId;
            }

            return null;
        }

        public async Task<List<SubscriptionResponse>> GetSubscriptionsByUserIdAsync()
        {
            var userId = GetCurrentUserId();
            var subscriptions = await _subscriptionRepository.GetAllSubscriptionsByUserIdAsync((int)userId);

            return mapper.Map<List<SubscriptionResponse>>(subscriptions);
        }


    }

}
