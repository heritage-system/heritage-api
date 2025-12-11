using AutoMapper;
using AutoMapper.QueryableExtensions;
using Azure.Core;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Models;
using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Request.ContribtutionReport;
using Cultural_Heritage_System.Dtos.Request.Contributor;
using Cultural_Heritage_System.Dtos.Request.Heritage;
using Cultural_Heritage_System.Dtos.Request.Panorama;
using Cultural_Heritage_System.Dtos.Request.Report;
using Cultural_Heritage_System.Dtos.Request.Review;
using Cultural_Heritage_System.Dtos.Request.Staff;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Heritage;
using Cultural_Heritage_System.Dtos.Response.Panorama;
using Cultural_Heritage_System.Dtos.Response.Report;
using Cultural_Heritage_System.Dtos.Response.Review;
using Cultural_Heritage_System.Helpers;
using Cultural_Heritage_System.Middlewares;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Cultural_Heritage_System.Repositories.Impl;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OfficeOpenXml.Packaging.Ionic.Zlib;
using System.Threading.Tasks;

namespace Cultural_Heritage_System.Services.Impl
{
    public class PanoramaTourService : IPanoramaTourService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IContributorRepository contributorRepository;
        private readonly IPanoramaTourRepository panoramaTourRepository;
        private readonly IPanoramaSceneRepository panoramaSceneRepository;
        private readonly IPanoramaSceneUnlockRepository panoramaSceneUnlockRepository;
        private readonly ISubscriptionRepository subscriptionRepository;
        private readonly IMapper mapper;
        private readonly IMailService mailService;
        private readonly ILogger<PanoramaTourService> logger;     
        private readonly IStaffRepository staffRepository;
        private readonly ISubscriptionUsageRepository subscriptionUsageRepository;
        private readonly IUserPointService userPointService;
        public PanoramaTourService(IContributorRepository contributorRepository, IPanoramaTourRepository panoramaTourRepository, ILogger<PanoramaTourService> logger, IMailService mailService,
            IMapper mapper, IHttpContextAccessor httpContextAccessor, ISubscriptionRepository subscriptionRepository,
            IStaffRepository staffRepository, IPanoramaSceneRepository panoramaSceneRepository, IPanoramaSceneUnlockRepository panoramaSceneUnlockRepository, ISubscriptionUsageRepository subscriptionUsageRepository, IUserPointService userPointService)
        {
            this.contributorRepository = contributorRepository;
            this.logger = logger;
            this.panoramaTourRepository = panoramaTourRepository;
            this.mailService = mailService;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
            this.subscriptionRepository = subscriptionRepository;         
            this.staffRepository = staffRepository;
            this.panoramaSceneRepository = panoramaSceneRepository;
            this.panoramaSceneUnlockRepository = panoramaSceneUnlockRepository;
            this.subscriptionUsageRepository = subscriptionUsageRepository;
            this.userPointService = userPointService;
        }

        public async Task<bool> CreatePanoramaTour(PanoramaTourCreationRequest request)
        {
            var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;


            if (string.IsNullOrEmpty(accountIdClaim))
            {
                throw new AppException(ErrorCode.UNAUTHORIZED);
            }
            PanoramaTour panoramaTour = mapper.Map<PanoramaTour>(request);
            panoramaTour.CreatedBy = accountIdClaim;  
                     
            await panoramaTourRepository.AddAsync(panoramaTour);

            return true;
        }

        public async Task<bool> UpdatePanoramaTour(long id, PanoramaTourCreationRequest request)
        {
           
            var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(accountIdClaim))
                throw new AppException(ErrorCode.UNAUTHORIZED);

           
            var panoramaTour = await panoramaTourRepository.GetPanoramaTourById(id);
            if (panoramaTour == null)
                throw new AppException(ErrorCode.PANORAMA_TOUR_NOT_FOUND);

            var oldScenes = panoramaTour.Scenes.ToList();
            foreach (var oldSc in oldScenes)
            {
                await panoramaSceneRepository.DeleteAsync(oldSc);
            }
           
            mapper.Map(request, panoramaTour);
            panoramaTour.UpdatedAt = DateTime.UtcNow;
            panoramaTour.UpdatedBy = accountIdClaim;

            await panoramaTourRepository.UpdateAsync(panoramaTour);

            return true;
        }

        public async Task<long?> DeletePanoramaTour(long id)
        {
            var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(accountIdClaim))
                throw new AppException(ErrorCode.UNAUTHORIZED);
            var panoramaTour = await panoramaTourRepository.GetPanoramaTourById(id);
            if (panoramaTour == null)
                throw new AppException(ErrorCode.PANORAMA_TOUR_NOT_FOUND);       

            await panoramaTourRepository.DeleteAsync(panoramaTour);
            return id;
        }

        public async Task<PanoramaTourDetailResponse> GetPanoramaTourDetail(long id)
        {
            var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
            var existingPanoramaTour = await panoramaTourRepository.GetPanoramaTourById(id);

            if (existingPanoramaTour == null)
                throw new AppException(ErrorCode.PANORAMA_TOUR_NOT_FOUND);

            var response = mapper.Map<PanoramaTourDetailResponse>(existingPanoramaTour);

            // Nếu toàn tour FREE -> không cần check sub
            if (existingPanoramaTour.PremiumType == PremiumType.FREE)
            {
                await HidePremiumScenesIfNeeded(response, accountIdClaim);
                return response;
            }

            // Nếu tour PREMIUM -> cũng cần check sub
            await HidePremiumScenesIfNeeded(response, accountIdClaim);

            return response;
        }

        private async Task HidePremiumScenesIfNeeded(PanoramaTourDetailResponse response, string? accountIdClaim)
        {
            // ===================== CASE 1: CHƯA LOGIN → PREVIEW =====================
            if (string.IsNullOrEmpty(accountIdClaim))
            {
                foreach (var scene in response.Scenes)
                {
                    if (scene.PremiumType != PremiumType.FREE)
                        scene.PanoramaUrl = null;
                }
                return;
            }

            int userId = int.Parse(accountIdClaim);

            // Lấy danh sách cảnh đã unlock
            var unlockedScenesQuery = panoramaSceneUnlockRepository
                .GetPanoramaSceneUnlocksQueryByUserId(userId);

            var unlockedSceneIds = unlockedScenesQuery
                .Select(x => x.PanoramaSceneId)
                .ToHashSet();

            // Tìm xem user đã unlock ÍT NHẤT 1 cảnh chưa
            bool hasAnyUnlock = unlockedSceneIds.Count > 0;

            // ===================== CASE 2: CHƯA UNLOCK → PREVIEW + SUB INFO =====================
            if (!hasAnyUnlock)
            {
                // Hidden preview
                foreach (var scene in response.Scenes)
                {
                    if (scene.PremiumType != PremiumType.FREE)
                        scene.PanoramaUrl = null;
                }

                response.UserPoint = (await userPointService.GetUserPointByUserId()).TotalPoints;
                // Lấy Subscription info giống Contribution
                var activeSub = await subscriptionRepository.GetActiveSubscription(userId);
                if (activeSub != null)
                {
                    var usage = activeSub.UsageRecords
                        .FirstOrDefault(c => c.BenefitName == BenefitName.TOUR);

                    if (usage != null)
                    {
                        response.Subscription = mapper.Map<SubscriptionDto>(activeSub);

                        if (usage.Total != null)
                            response.Subscription.Total = (int)usage.Total;
                        else
                            response.Subscription.IsUnlimited = true;

                        response.Subscription.Used = usage.Used;
                    }
                }

                return;
            }

            // ===================== CASE 3: ĐÃ UNLOCK → CHECK TỪNG CẢNH =====================
            // Đầu tiên lấy active subscription để xử lý unlock-by-sub
            var activeSubscription = await subscriptionRepository.GetActiveSubscription(userId);

            if (activeSubscription != null)
            {
                var usage = activeSubscription.UsageRecords
                    .FirstOrDefault(c => c.BenefitName == BenefitName.TOUR);

                if (usage != null)
                {
                    response.Subscription = mapper.Map<SubscriptionDto>(activeSubscription);

                    if (usage.Total != null)
                        response.Subscription.Total = (int)usage.Total;
                    else
                        response.Subscription.IsUnlimited = true;

                    response.Subscription.Used = usage.Used;
                }
            }

            response.UserPoint = (await userPointService.GetUserPointByUserId()).TotalPoints;
            foreach (var scene in response.Scenes)
            {
                // FREE luôn mở
                if (scene.PremiumType == PremiumType.FREE)
                    continue;

                // ĐÃ UNLOCK BẰNG POINT → MỞ MÃI
                var unlockRecord = unlockedScenesQuery
                    .FirstOrDefault(x => x.PanoramaSceneId == scene.Id);

                if (unlockRecord?.UnlockingMethod == UnlockingMethod.BY_POINT)
                {
                    // Không map subscription info
                    continue;
                }

                // ĐÃ UNLOCK BẰNG SUB → PHỤ THUỘC SUB ACTIVE
                if (unlockRecord?.UnlockingMethod == UnlockingMethod.BY_SUBSCRIPTION)
                {
                    if (activeSubscription == null)
                    {
                        // Sub hết hạn → Preview lại
                        scene.PanoramaUrl = null;
                        scene.UnSubscriptionLock = true;
                    }

                    continue;
                }

                // Chưa unlock gì cả → preview
                if (!unlockedSceneIds.Contains(scene.Id))
                {
                    scene.PanoramaUrl = null;
                }
            }

            // Khi đã unlock rồi (dù bằng sub hay point) → KHÔNG trả subscription info nữa
        }


        public async Task<PanoramaSceneResponse> GetPanoramaSceneDetail(long id)
        {
            var existingPanoramaScene = await panoramaSceneRepository.GetPanoramaSceneById(id);

            if (existingPanoramaScene == null)
                throw new AppException(ErrorCode.PANORAMA_SCENE_NOT_FOUND);



            var response = mapper.Map<PanoramaSceneResponse>(existingPanoramaScene);         
            return response;
        }

        public async Task<PageResponse<PanoramaTourSearchResponse>> SearchPanoramaTour(PanoramaTourSearchRequest request)
        {
            try
            {
                var query = panoramaTourRepository.GetPanoramaToursQueryable().Where(c => c.Status == PanoramaStatus.ACTIVE && c.Scenes.Any() == true);


                // Keyword search
                if (!string.IsNullOrWhiteSpace(request.Keyword))
                {
                    string keyword = request.Keyword.Trim().ToLower();
                    keyword = keyword
                        .Replace("thành phố ", "")
                        .Replace("tp. ", "")
                        .Replace("tp ", "")
                        .Replace("tỉnh ", "");

                    string unsigned = StringHelper.RemoveDiacritics(keyword).ToLower();

                    query = query.Where(t =>
                        // Search PanoramaTour fields
                        t.Name.ToLower().Contains(keyword) ||
                        t.NameUnsigned.Contains(unsigned) ||

                        // Search Heritage name
                        t.Heritage.Name.ToLower().Contains(keyword) ||
                        t.Heritage.NameUnsigned.Contains(unsigned) ||

                        // Search location (province/district/ward/address)
                        t.Heritage.HeritageLocations.Any(hl =>
                            hl.Location.Province.ToLower().Contains(keyword) ||
                            hl.Location.District.ToLower().Contains(keyword) ||
                            hl.Location.Ward.ToLower().Contains(keyword) ||
                            hl.Location.AddressDetail.ToLower().Contains(keyword) ||
                            hl.Location.ProvinceUnsigned.Contains(unsigned) ||
                            hl.Location.DistrictUnsigned.Contains(unsigned) ||
                            hl.Location.WardUnsigned.Contains(unsigned) ||
                            hl.Location.AddressDetailUnsigned.Contains(unsigned)
                        )
                    );
                }


                if (request.CategoryIds != null && request.CategoryIds.Any())
                {
                    query = query.Where(h => request.CategoryIds.Contains(h.Heritage.CategoryId));
                }


                switch (request.SortBy)
                {
                    case SortBy.IDASC:
                        query = query.OrderBy(h => h.Id);
                        break;
                    case SortBy.IDDESC:
                        query = query.OrderByDescending(h => h.Id);
                        break;
                    case SortBy.NAMEASC:
                        query = query.OrderBy(h => h.Name);
                        break;
                    case SortBy.NAMEDESC:
                        query = query.OrderByDescending(h => h.Name);
                        break;
                    default:
                        query = query.OrderByDescending(h => h.CreatedAt);
                        break;
                }


                var dtoQuery = query.ProjectTo<PanoramaTourSearchResponse>(mapper.ConfigurationProvider);

                // Pagination
                var response = await dtoQuery.ToPagedResponseAsync(request.Page, request.PageSize);
             
                return response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error searching heritages");
                throw;
            }
        }

        public async Task<PageResponse<PanoramaTourSearchForAdminResponse>> GetListPanoramaTourForAdmin(PanoramaTourSearchRequest request)
        {
            try
            {
                var query = panoramaTourRepository.GetPanoramaToursQueryable();


                // Keyword search
                if (!string.IsNullOrWhiteSpace(request.Keyword))
                {
                    string keyword = request.Keyword.Trim().ToLower();
                    keyword = keyword
                        .Replace("thành phố ", "")
                        .Replace("tp. ", "")
                        .Replace("tp ", "")
                        .Replace("tỉnh ", "");

                    string unsigned = StringHelper.RemoveDiacritics(keyword).ToLower();

                    query = query.Where(t =>
                        // Search PanoramaTour fields
                        t.Name.ToLower().Contains(keyword) ||
                        t.NameUnsigned.Contains(unsigned) ||

                        // Search Heritage name
                        t.Heritage.Name.ToLower().Contains(keyword) ||
                        t.Heritage.NameUnsigned.Contains(unsigned) ||

                        // Search location (province/district/ward/address)
                        t.Heritage.HeritageLocations.Any(hl =>
                            hl.Location.Province.ToLower().Contains(keyword) ||
                            hl.Location.District.ToLower().Contains(keyword) ||
                            hl.Location.Ward.ToLower().Contains(keyword) ||
                            hl.Location.AddressDetail.ToLower().Contains(keyword) ||
                            hl.Location.ProvinceUnsigned.Contains(unsigned) ||
                            hl.Location.DistrictUnsigned.Contains(unsigned) ||
                            hl.Location.WardUnsigned.Contains(unsigned) ||
                            hl.Location.AddressDetailUnsigned.Contains(unsigned)
                        )
                    );
                }


                if (request.CategoryIds != null && request.CategoryIds.Any())
                {
                    query = query.Where(h => request.CategoryIds.Contains(h.Heritage.CategoryId));
                }


                switch (request.SortBy)
                {
                    case SortBy.IDASC:
                        query = query.OrderBy(h => h.Id);
                        break;
                    case SortBy.IDDESC:
                        query = query.OrderByDescending(h => h.Id);
                        break;
                    case SortBy.NAMEASC:
                        query = query.OrderBy(h => h.Name);
                        break;
                    case SortBy.NAMEDESC:
                        query = query.OrderByDescending(h => h.Name);
                        break;
                    default:
                        query = query.OrderByDescending(h => h.CreatedAt);
                        break;
                }


                var dtoQuery = query.ProjectTo<PanoramaTourSearchForAdminResponse>(mapper.ConfigurationProvider);

                // Pagination
                var response = await dtoQuery.ToPagedResponseAsync(request.Page, request.PageSize);

                return response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error searching heritages");
                throw;
            }
        }

        public async Task<PanoramaTourDetailForAdminResponse> GetPanoramaTourDetailForAdmin(long id)
        {
            var existingPanoramaTour = await panoramaTourRepository.GetPanoramaTourById(id);

            if (existingPanoramaTour == null)
                throw new AppException(ErrorCode.PANORAMA_TOUR_NOT_FOUND);

            var response = mapper.Map<PanoramaTourDetailForAdminResponse>(existingPanoramaTour);
            return response;
        }

        public async Task<long> CreatePanoramaScene(PanoramaSceneCreationRequest request)
        {
            var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;


            if (string.IsNullOrEmpty(accountIdClaim))
            {
                throw new AppException(ErrorCode.UNAUTHORIZED);
            }
            PanoramaScene panoramaScene = mapper.Map<PanoramaScene>(request);
            panoramaScene.CreatedBy = accountIdClaim;

            await panoramaSceneRepository.AddAsync(panoramaScene);

            return panoramaScene.Id;
        }

        public async Task<bool> UpdatePanoramaScene(long id, PanoramaSceneCreationRequest request)
        {

            var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(accountIdClaim))
                throw new AppException(ErrorCode.UNAUTHORIZED);


            var panoramaScene = await panoramaSceneRepository.GetPanoramaSceneById(id);
            if (panoramaScene == null)
                throw new AppException(ErrorCode.PANORAMA_SCENE_NOT_FOUND);

            mapper.Map(request, panoramaScene);
            panoramaScene.UpdatedAt = DateTime.UtcNow;
            panoramaScene.UpdatedBy = accountIdClaim;

            await panoramaSceneRepository.UpdateAsync(panoramaScene);

            return true;
        }

        public async Task<long?> DeletePanoramaScene(long id)
        {
            var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(accountIdClaim))
                throw new AppException(ErrorCode.UNAUTHORIZED);         
            var panoramaScene = await panoramaSceneRepository.GetPanoramaSceneById(id);
            if (panoramaScene == null)
                throw new AppException(ErrorCode.PANORAMA_SCENE_NOT_FOUND);

            await panoramaSceneRepository.DeleteAsync(panoramaScene);
            return id;
        }

        public async Task<PanoramaSceneResponse> UnlockPanoramaScene(long sceneId)
        {
            var existingScene = await panoramaSceneRepository.GetPanoramaSceneById(sceneId);

            if (existingScene == null)
                throw new AppException(ErrorCode.PANORAMA_SCENE_NOT_FOUND);

            var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(accountIdClaim))
            {
                throw new AppException(ErrorCode.UNAUTHORIZED);
            }

            var userId = int.Parse(accountIdClaim);

            // Lấy subscription active
            var activeSub = await subscriptionRepository.GetActiveSubscription(userId);


            if (activeSub == null)
            {
                throw new AppException(ErrorCode.USER_NOT_PREMIUM);
            }

            var sceneUnlock = activeSub.UsageRecords.FirstOrDefault(c => c.BenefitName == BenefitName.TOUR);
            if (sceneUnlock == null)
            {
                throw new AppException(ErrorCode.SUBSCRIPTION_USAGE_NOT_FOUND);
            }
            if (sceneUnlock.Used >= sceneUnlock.Total)
            {
                throw new AppException(ErrorCode.OVER_OPEN_LIMIT);
            }

            sceneUnlock.Used++;
            await subscriptionUsageRepository.UpdateAsync(sceneUnlock);

            var unlock = new PanoramaSceneUnlock
            {
                UserId = userId,
                PanoramaSceneId = sceneId,
                UnlockingMethod = UnlockingMethod.BY_SUBSCRIPTION
            };
            await panoramaSceneUnlockRepository.AddAsync(unlock);            

            var result = mapper.Map<PanoramaSceneResponse>(existingScene);
            return result;
        }
    }
}
