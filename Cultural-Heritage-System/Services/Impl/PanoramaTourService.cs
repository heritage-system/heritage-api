using AutoMapper;
using AutoMapper.QueryableExtensions;
using Azure.Core;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Models;
using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Request.ContribtutionReport;
using Cultural_Heritage_System.Dtos.Request.Heritage;
using Cultural_Heritage_System.Dtos.Request.Report;
using Cultural_Heritage_System.Dtos.Request.Review;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Panorama;
using Cultural_Heritage_System.Dtos.Response.Heritage;
using Cultural_Heritage_System.Dtos.Response.Report;
using Cultural_Heritage_System.Dtos.Response.Review;
using Cultural_Heritage_System.Helpers;
using Cultural_Heritage_System.Middlewares;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OfficeOpenXml.Packaging.Ionic.Zlib;
using System.Threading.Tasks;
using Cultural_Heritage_System.Dtos.Request.Panorama;

namespace Cultural_Heritage_System.Services.Impl
{
    public class PanoramaTourService : IPanoramaTourService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IContributorRepository contributorRepository;
        private readonly IPanoramaTourRepository panoramaTourRepository;
        private readonly IPanoramaSceneRepository panoramaSceneRepository;
        private readonly IPanoramaInteractionPointRepository panoramaInteractionPointRepository;
        private readonly ISubscriptionRepository subscriptionRepository;
        private readonly IMapper mapper;
        private readonly IMailService mailService;
        private readonly ILogger<PanoramaTourService> logger;     
        private readonly IStaffRepository staffRepository;
        public PanoramaTourService(IContributorRepository contributorRepository, IPanoramaTourRepository panoramaTourRepository, ILogger<PanoramaTourService> logger, IMailService mailService,
            IMapper mapper, IHttpContextAccessor httpContextAccessor, ISubscriptionRepository subscriptionRepository,
            IStaffRepository staffRepository, IPanoramaSceneRepository panoramaSceneRepository, IPanoramaInteractionPointRepository panoramaInteractionPointRepository)
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
            this.panoramaInteractionPointRepository = panoramaInteractionPointRepository;
        }    

        //public async Task<bool> CreatePanoramaTour(PanoramaTourCreationRequest request)
        //{
        //    var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
        //    PanoramaTour panoramaTour = mapper.Map<PanoramaTour>(request);

        //    if (string.IsNullOrEmpty(accountIdClaim))
        //    {
        //        throw new AppException(ErrorCode.UNAUTHORIZED);
        //    }
        //    var currentContributor = await contributorRepository.GetContributorByUserId(int.Parse(accountIdClaim));
        //    if (currentContributor == null)
        //    {
        //        throw new AppException(ErrorCode.UNAUTHORIZED);
        //    }         
        //    if (request.Scenes?.Any() == true)
        //    {
        //        panoramaTour.PanoramaTourHeritageTags = request.TagHeritageIds.Select(o => new PanoramaTourHeritageTag
        //        {
        //            HeritageId = o
        //        }).ToList();
        //    }
        //    panoramaTour.ContributorId = currentContributor.Id;

        //    panoramaTour.PreviewContent = DeltaHelper.GeneratePreviewDelta(panoramaTour.Content);

        //    panoramaTour.FirstContent = DeltaHelper.ExtractFirstLongParagraph(panoramaTour.Content);

        //    var nextStaffId = await GetNextStaffForPanoramaTourAsync();
        //    if (nextStaffId != null)
        //    {
        //        panoramaTour.PanoramaTourAcceptances.Add(new PanoramaTourAcceptance
        //        {
        //            StaffId = nextStaffId.Value,                 
        //            Note = "Bài viết mới, chờ duyệt"
        //        });
        //    }


        //    await panoramaTourRepository.AddAsync(panoramaTour);

        //    return mapper.Map<PanoramaTourResponse>(panoramaTour);
        //}    

        public async Task<PanoramaSceneResponse> GetPanoramaSceneDetail(long id)
        {
            var existingPanoramaScene = await panoramaSceneRepository.GetPanoramaSceneById(id);

            if (existingPanoramaScene == null)
                throw new AppException(ErrorCode.PANORAMA_SCENE_NOT_FOUND);

            var response = mapper.Map<PanoramaSceneResponse>(existingPanoramaScene);         
            return response;
        }
      
    }
}
