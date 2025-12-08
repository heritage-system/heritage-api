using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Models;
using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Request.Category;
using Cultural_Heritage_System.Dtos.Request.ContribtutionReport;
using Cultural_Heritage_System.Dtos.Request.Contributor;
using Cultural_Heritage_System.Dtos.Request.Event;
using Cultural_Heritage_System.Dtos.Request.Heritage;
using Cultural_Heritage_System.Dtos.Request.Location;
using Cultural_Heritage_System.Dtos.Request.Media;
using Cultural_Heritage_System.Dtos.Request.Occurrence;
using Cultural_Heritage_System.Dtos.Request.Panorama;
using Cultural_Heritage_System.Dtos.Request.PremiumBenefit;
using Cultural_Heritage_System.Dtos.Request.PremiumPackage;
using Cultural_Heritage_System.Dtos.Request.Quiz;
using Cultural_Heritage_System.Dtos.Request.Report;
using Cultural_Heritage_System.Dtos.Request.Review;
using Cultural_Heritage_System.Dtos.Request.Tag;
using Cultural_Heritage_System.Dtos.Request.User;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Category;
using Cultural_Heritage_System.Dtos.Response.Contributor;
using Cultural_Heritage_System.Dtos.Response.Event;
using Cultural_Heritage_System.Dtos.Response.Heritage;
using Cultural_Heritage_System.Dtos.Response.Location;
using Cultural_Heritage_System.Dtos.Response.Media;
using Cultural_Heritage_System.Dtos.Response.Occurence;
using Cultural_Heritage_System.Dtos.Response.Panorama;
using Cultural_Heritage_System.Dtos.Response.PremiumBenefit;
using Cultural_Heritage_System.Dtos.Response.PremiumPackage;
using Cultural_Heritage_System.Dtos.Response.Quiz;
using Cultural_Heritage_System.Dtos.Response.QuizQuestion;
using Cultural_Heritage_System.Dtos.Response.Report;
using Cultural_Heritage_System.Dtos.Response.Review;
using Cultural_Heritage_System.Dtos.Response.Staff;
using Cultural_Heritage_System.Dtos.Response.Streaming;
using Cultural_Heritage_System.Dtos.Response.Tag;
using Cultural_Heritage_System.Dtos.Response.User;
using Cultural_Heritage_System.Dtos.Response.UserPoint;
using Cultural_Heritage_System.Models;
using System.Text.Json;


namespace Cultural_Heritage_System.Helpers
{

    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            CreateMap<UserCreationRequest, User>();
            CreateMap<User, UserCreationResponse>();
            CreateMap<PasswordReset, ForgotPasswordResponse>();


            CreateMap<Heritage, HeritageSearchResponse>()
            .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
            .ForMember(dest => dest.HeritageTags,
                opt => opt.MapFrom(src => src.HeritageTags.Select(ht => ht.Tag.Name)))
            .ForMember(dest => dest.HeritageOccurrences,
                opt => opt.MapFrom(src => src.HeritageOccurrences))
            .ForMember(dest => dest.Media,
                       opt => opt.MapFrom(src =>
                           src.Media.FirstOrDefault(m => m.MediaType == MediaType.IMAGE)))
            .ForMember(dest => dest.HeritageLocations,
                opt => opt.MapFrom(src => src.HeritageLocations.Select(hl => hl.Location)));

            CreateMap<Heritage, HeritageDetailResponse>()
           .ForMember(dest => dest.CategoryName,
               opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
           .ForMember(dest => dest.HeritageTags,
               opt => opt.MapFrom(src => src.HeritageTags.Select(ht => ht.Tag.Name)))
           .ForMember(dest => dest.HeritageTagIds,
               opt => opt.MapFrom(src => src.HeritageTags.Select(ht => ht.Tag.Id)))
           .ForMember(dest => dest.HeritageOccurrences,
               opt => opt.MapFrom(src => src.HeritageOccurrences))
           .ForMember(dest => dest.Media,
               opt => opt.MapFrom(src => src.Media))
           .ForMember(dest => dest.HeritageLocations,
               opt => opt.MapFrom(src => src.HeritageLocations.Select(hl => hl.Location)));

            // HeritageOccurrence → HeritageOccurrenceDto
            CreateMap<HeritageOccurrence, HeritageOccurrenceDto>()
                .ForMember(dest => dest.OccurrenceTypeName, opt => opt.MapFrom(src => src.OccurrenceType.ToString()))
                .ForMember(dest => dest.CalendarTypeName, opt => opt.MapFrom(src => src.CalendarType.HasValue ? src.CalendarType.Value.ToString() : null))
                .ForMember(dest => dest.FrequencyName, opt => opt.MapFrom(src => src.Frequency.HasValue ? src.Frequency.Value.ToString() : null));

            // HeritageMedia → HeritageMediaDto
            CreateMap<HeritageMedia, HeritageMediaDto>()
                .ForMember(dest => dest.MediaTypeName, opt => opt.MapFrom(src => src.MediaType.ToString()));

            // Location → HeritageLocationDto
            CreateMap<Location, HeritageLocationDto>();

            // MappingProfile.cs
            CreateMap<UpdateProfileRequest, User>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<UpdateProfileRequest, Cultural_Heritage_System.Models.Profile>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<User, UpdateProfileResponse>();
            CreateMap<Cultural_Heritage_System.Models.Profile, UpdateProfileResponse>();

            CreateMap<CreateTagRequest, Tag>();
            CreateMap<Tag, CreateTagResponse>();

            CreateMap<UpdateTagRequest, Tag>();
            CreateMap<Tag, UpdateTagResponse>();

            CreateMap<DeleteTagRequest, Tag>();
            CreateMap<Tag, DeleteTagResponse>();

            CreateMap<Tag, TagSearchResponse>();


            CreateMap<CreateCategoryRequest, Category>();
            CreateMap<Category, CreateCategoryResponse>();

            CreateMap<UpdateCategoryRequest, Category>();
            CreateMap<Category, UpdateCategoryResponse>();

            CreateMap<DeleteCategoryRequest, Category>();
            CreateMap<Category, DeleteCategoryResponse>();

            CreateMap<Category, CategorySearchResponse>();
            // Contributor
            CreateMap<ContributorCreateRequest, Contributor>();
            CreateMap<ContributorUpdateRequest, Contributor>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));


            //Heritage
            CreateMap<HeritageCreateRequest, Heritage>()
            .ForMember(dest => dest.Content,
               opt => opt.MapFrom(src => JsonSerializer.Serialize(src.Content, (JsonSerializerOptions)null)));
            //.ForMember(dest => dest.Media, opt => opt.MapFrom(src => src.Media))
            //.ForMember(dest => dest.HeritageOccurrences, opt => opt.MapFrom(src => src.Occurrences))
            //.ForMember(dest => dest.HeritageTags, opt => opt.MapFrom(src => src.TagIds));
            CreateMap<HeritageUpdateRequest, Heritage>();
            CreateMap<Heritage, HeritageResponse>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
            .ForMember(dest => dest.Media, opt => opt.MapFrom(src => src.Media))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.HeritageTags.Select(ht => ht.Tag)))
            .ForMember(dest => dest.Locations, opt => opt.MapFrom(src => src.HeritageLocations.Select(hl => hl.Location)))
            .ForMember(dest => dest.Occurrences, opt => opt.MapFrom(src => src.HeritageOccurrences))
            .ForMember(dest => dest.Content, opt => opt.Ignore())
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content));

            CreateMap<HeritageMedia, MediaResponse>();
            CreateMap<Tag, TagResponse>();
            CreateMap<Location, LocationResponse>();
            CreateMap<HeritageOccurrence, OccurrenceResponse>();

            CreateMap<LocationRequest, Location>();
            CreateMap<MediaRequest, HeritageMedia>()
                .ForMember(dest => dest.MediaType, opt => opt.MapFrom(src => src.MediaType));
            CreateMap<OccurrenceRequest, HeritageOccurrence>();

            CreateMap(typeof(PageResponse<>), typeof(PageResponse<>));

            CreateMap<CreateReportRequest, Report>();
            CreateMap<Report, ReportResponse>()
                .ForMember(dest => dest.HeritageName, opt => opt.MapFrom(src => src.Heritage.Name))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));

            CreateMap<UpdateReportRequest, Report>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Favorite, FavoriteHeritageResponse>()
                .ForMember(dest => dest.HeritageId, opt => opt.MapFrom(src => src.HeritageId))
                .ForMember(dest => dest.HeritageName, opt => opt.MapFrom(src => src.Heritage.Name))
                .ForMember(dest => dest.HeritageDescription, opt => opt.MapFrom(src => src.Heritage.Description))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Heritage.Category.Name))
                .ForMember(dest => dest.IsFeatured, opt => opt.MapFrom(src => src.Heritage.IsFeatured))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));
            // Review → ReviewResponse
            CreateMap<Review, ReviewResponse>()
        .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.UserName))
        .ForMember(dest => dest.UserImageUrl, opt => opt.MapFrom(src => src.User.Profile.AvatarUrl))
        .ForMember(dest => dest.Likes, opt => opt.MapFrom(src => src.Likes != null ? src.Likes.Count : 0))
        .ForMember(dest => dest.LikedByMe, opt => opt.Ignore())
        .ForMember(dest => dest.CreatedByMe, opt => opt.Ignore())
        .ForMember(dest => dest.ReviewMedias, opt => opt.MapFrom(src => src.ReviewMedias))
        .ForMember(dest => dest.Replies, opt => opt.Ignore())
        .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
         .ForMember(dest => dest.IsUpdated,
            opt => opt.MapFrom(src => src.UpdatedAt != null
                && (src.UpdatedAt - src.CreatedAt).TotalSeconds > 1));

            CreateMap<ReviewResponse, Review>();
            // ReviewMedia → ReviewMediaResponse
            CreateMap<ReviewMedia, ReviewMediaResponse>()
    .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.MediaType));


            // ReviewCreateRequest → Review
            CreateMap<ReviewCreateRequest, Review>()
                .ForMember(dest => dest.HeritageId, opt => opt.MapFrom(src => src.HeritageId))
                .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment))
                .ForMember(dest => dest.ParentReviewId, opt => opt.MapFrom(src => src.ParentReviewId));

            // ReviewMediaRequest → ReviewMedia
            CreateMap<ReviewMediaRequest, ReviewMedia>();



            // Map from Review entity to LikeReviewResponse DTO
            CreateMap<Review, LikeReviewResponse>()
                .ForMember(dest => dest.ReviewId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.LikeCount, opt => opt.MapFrom(src => src.Likes != null ? src.Likes.Count : 0))
                .ForMember(dest => dest.LikedByMe, opt => opt.Ignore()); // Set manually in service

            // Map from LikeReviewRequest DTO to ReviewLike entity
            CreateMap<LikeReviewRequest, ReviewLike>()
                .ForMember(dest => dest.ReviewId, opt => opt.MapFrom(src => src.ReviewId))
                .ForMember(dest => dest.UserId, opt => opt.Ignore()) // Set manually in service
                .ForMember(dest => dest.Review, opt => opt.Ignore())  // Will be handled by EF
                .ForMember(dest => dest.User, opt => opt.Ignore());   // Will be handled by EF
            CreateMap<Review, LikeReviewResponse>()
                  .ForMember(dest => dest.ReviewId, opt => opt.MapFrom(src => src.Id))
                  .ForMember(dest => dest.LikeCount, opt => opt.MapFrom(src => src.Likes != null ? src.Likes.Count : 0))
                  .ForMember(dest => dest.LikedByMe, opt => opt.Ignore()); // set manually in service
            CreateMap<ReviewUpdateRequest, Review>()
            .ForMember(dest => dest.ReviewMedias, opt => opt.Ignore()); // handled manually
            CreateMap<Review, ReviewUpdateResponse>();
            CreateMap<ReviewDeleteRequest, Review>();
            CreateMap<Review, ReviewDeleteResponse>().
                ForMember(dest => dest.ReviewId, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.success, opt => opt.Ignore()) // Set manually
               .ForMember(dest => dest.message, opt => opt.Ignore()); // Set manually;

            CreateMap<ContributionCreationRequest, Contribution>();
            CreateMap<Contribution, ContributionResponse>()
             .ForMember(d => d.ContributorName, o => o.MapFrom(s => s.Contributor.User.UserName))
             .ForMember(d => d.AvatarUrl, o => o.MapFrom(s => s.Contributor.User.Profile.AvatarUrl))
             .ForMember(dest => dest.View,
                    opt => opt.MapFrom(src => src.ContributionAccessLogs != null ? src.ContributionAccessLogs.Count : 0));


            CreateMap<ContributionHeritageTag, HeritageNameSearchResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.HeritageId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Heritage.Name));

            CreateMap<Contribution, ContributionSearchResponse>()
             .ForMember(d => d.ContributorName, o => o.MapFrom(s => s.Contributor.User.UserName))
             .ForMember(d => d.AvatarUrl, o => o.MapFrom(s => s.Contributor.User.Profile.AvatarUrl))
             .ForMember(dest => dest.View,
                    opt => opt.MapFrom(src => src.ContributionAccessLogs != null ? src.ContributionAccessLogs.Count : 0))
            .ForMember(dest => dest.Comments,
                    opt => opt.MapFrom(src => src.Reviews != null ? src.Reviews.Count : 0))
            .ForMember(dest => dest.IsPremium, opt => opt.MapFrom(src =>
                src.PremiumType != PremiumType.FREE));


            CreateMap<Heritage, HeritageNameSearchResponse>();

            CreateMap<Subscription, SubscriptionDto>();

            CreateMap<ContributionSave, ContributionSaveResponse>()
                .ForMember(dest => dest.ContributionId, opt => opt.MapFrom(src => src.Contribution.Id))
                .ForMember(dest => dest.ContributorId, opt => opt.MapFrom(src => src.Contribution.Contributor.Id))
                .ForMember(dest => dest.ContributorName, opt => opt.MapFrom(src => src.Contribution.Contributor.User.Profile.FullName))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.Contribution.Contributor.User.Profile.AvatarUrl))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Contribution.Title))
                .ForMember(dest => dest.MediaUrl, opt => opt.MapFrom(src => src.Contribution.MediaUrl))
                .ForMember(dest => dest.ContributionHeritageTags, opt => opt.MapFrom(src => src.Contribution.ContributionHeritageTags.Select(ht => ht.Heritage)));

            CreateMap<ContributionReview, ContributionReviewResponse>()
            .ForMember(dest => dest.Username,
                opt => opt.MapFrom(src => src.User.UserName))
            .ForMember(dest => dest.UserImageUrl,
                opt => opt.MapFrom(src => src.User.Profile.AvatarUrl))
            .ForMember(dest => dest.Likes,
                opt => opt.MapFrom(src => src.Likes != null ? src.Likes.Count : 0))
            .ForMember(dest => dest.Replies,
                opt => opt.MapFrom(src => src.Replies))
            .ForMember(dest => dest.LikedByMe,
                opt => opt.Ignore())
            .ForMember(dest => dest.CreatedByMe,
                opt => opt.Ignore());

            CreateMap<ContributionReviewCreateRequest, ContributionReview>();


            CreateMap<ContributionReview, LikeReviewResponse>()
                  .ForMember(dest => dest.ReviewId, opt => opt.MapFrom(src => src.Id))
                  .ForMember(dest => dest.LikeCount, opt => opt.MapFrom(src => src.Likes != null ? src.Likes.Count : 0))
                  .ForMember(dest => dest.LikedByMe, opt => opt.Ignore());
            CreateMap<ContributionReviewUpdateRequest, ContributionReview>();
            CreateMap<ContributionReview, ContributionReviewUpdateResponse>();

            CreateMap<ContributionReportCreationRequest, ContributionReport>();

            CreateMap<Contributor, ContributorResponse>()
             .ForMember(dest => dest.Email,
                opt => opt.MapFrom(src => src.User != null ? src.User.Email : string.Empty))
                .ForMember(dest => dest.Phone,
                opt => opt.MapFrom(src => src.User.Profile != null ? src.User.Profile.Phone : string.Empty))
                .ForMember(dest => dest.FullName,
                opt => opt.MapFrom(src => src.User.Profile != null ? src.User.Profile.FullName : string.Empty))
                .ForMember(dest => dest.Address,
                opt => opt.MapFrom(src => src.User.Profile != null ? src.User.Profile.Address : string.Empty))
                .ForMember(dest => dest.DateOfBirth,
                opt => opt.MapFrom(src => src.User.Profile != null ? src.User.Profile.DateOfBirth : null))                
                .ForMember(dest => dest.UserName,
                opt => opt.MapFrom(src => src.User != null ? src.User.UserName : string.Empty))
                .ForMember(dest => dest.Phone,
                opt => opt.MapFrom(src => src.User.Profile != null ? src.User.Profile.Phone : string.Empty))
             .ForMember(dest => dest.Count,
                 opt => opt.MapFrom(src => src.Contributions != null ? src.Contributions.Count : 0))
              .ForMember(dest => dest.DocumentsUrl,
                    opt => opt.MapFrom(src => src.DocumentsUrl))
             .ForMember(dest => dest.CreatedByName, opt => opt.Ignore())
             .ForMember(dest => dest.CreatedByEmail, opt => opt.Ignore())
             .ForMember(dest => dest.UpdatedByName, opt => opt.Ignore())
             .ForMember(dest => dest.UpdatedByEmail, opt => opt.Ignore());

            CreateMap<Contributor, ContributorApplyResponse>()
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<Contribution, ContributionOverviewResponse>()
                .ForMember(dest => dest.View,
                    opt => opt.MapFrom(src => src.ContributionAccessLogs != null ? src.ContributionAccessLogs.Count : 0))
                .ForMember(dest => dest.Save,
                    opt => opt.MapFrom(src => src.ContributionSaves != null ? src.ContributionSaves.Count : 0))
            .ForMember(dest => dest.Reports,
                    opt => opt.MapFrom(src => src.ContributionReports != null ? src.ContributionReports.Count : 0))
            .ForMember(dest => dest.Comments,
                    opt => opt.MapFrom(src => src.Reviews != null ? src.Reviews.Count : 0))
            .ForMember(dest => dest.IsPremium, opt => opt.MapFrom(src =>
                src.PremiumType != PremiumType.FREE))
            .ForMember(dest => dest.AcceptanceId, opt => opt.MapFrom(src =>
                src.ContributionAcceptances
                    .OrderByDescending(a => a.CreatedAt)
                    .FirstOrDefault() != null
                        ? src.ContributionAcceptances
                            .OrderByDescending(a => a.CreatedAt)
                            .First().Id
                        : 0));

            CreateMap<Contribution, ContributionOverviewListItemResponse>()
             .ForMember(dest => dest.View,
                    opt => opt.MapFrom(src => src.ContributionAccessLogs != null ? src.ContributionAccessLogs.Count : 0))
             .ForMember(dest => dest.Comments,
                    opt => opt.MapFrom(src => src.Reviews != null ? src.Reviews.Count : 0))
             .ForMember(dest => dest.Saves,
                    opt => opt.MapFrom(src => src.ContributionSaves != null ? src.ContributionSaves.Count : 0))
            .ForMember(dest => dest.IsPremium, opt => opt.MapFrom(src =>
                src.PremiumType != PremiumType.FREE))
            .ForMember(dest => dest.AcceptanceId, opt => opt.MapFrom(src =>
                src.ContributionAcceptances
                    .OrderByDescending(a => a.CreatedAt)
                    .FirstOrDefault() != null
                        ? src.ContributionAcceptances
                            .OrderByDescending(a => a.CreatedAt)
                            .First().Id
                        : 0));

            CreateMap<Contribution, ContributionDetailUpdatedResponse>()
                .ForMember(dest => dest.IsPremium, opt => opt.MapFrom(src =>
                src.PremiumType != PremiumType.FREE));

            CreateMap<Heritage, HeritageRelatedResponse>()
            .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
            .ForMember(dest => dest.HeritageTags,
                opt => opt.MapFrom(src => src.HeritageTags.Select(ht => ht.Tag.Name)))
            .ForMember(dest => dest.Media,
                       opt => opt.MapFrom(src =>
                           src.Media.FirstOrDefault(m => m.MediaType == MediaType.IMAGE)))
            .ForMember(dest => dest.HeritageLocations,
                opt => opt.MapFrom(src => src.HeritageLocations.Select(hl => hl.Location)));

            CreateMap<QuizQuestion, QuizQuestionResponse>();
            CreateMap<Quiz, QuizDetailResponse>();
            CreateMap<Quiz, QuizListResponse>()
            .ForMember(dest => dest.TotalQuestions,
                opt => opt.MapFrom(src => src.Questions.Count))
            .ForMember(dest => dest.isPremium,
                opt => opt.MapFrom(src => src.PremiumType != PremiumType.FREE));

            CreateMap<QuizCreationRequest, Quiz>();
            CreateMap<QuizUpdateRequest, Quiz>();
            CreateMap<QuizQuestionCreationRequest, QuizQuestion>();
            CreateMap<QuizQuestionUpdateRequest, QuizQuestion>();

            CreateMap<Quiz, QuizDetailAdminResponse>()
            .ForMember(dest => dest.PremiumType, opt => opt.MapFrom(src => src.PremiumType))
            .ForMember(dest => dest.TotalQuestions, opt => opt.MapFrom(src => src.Questions.Count))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

            CreateMap<QuizResult, QuizResultInfo>();


            CreateMap<PanoramaScene, PanoramaSceneResponse>();

            CreateMap<UserCreationByAdminRequest, User>();
            CreateMap<User, UserSearchResponse>();
            CreateMap<User, UserDetailResponse>()
             .ForMember(dest => dest.Phone,
                opt => opt.MapFrom(src => src.Profile != null ? src.Profile.Phone : string.Empty))
             .ForMember(dest => dest.FullName,
                opt => opt.MapFrom(src => src.Profile != null ? src.Profile.FullName : string.Empty))
             .ForMember(dest => dest.Address,
                opt => opt.MapFrom(src => src.Profile != null ? src.Profile.Address : string.Empty))
             .ForMember(dest => dest.DateOfBirth,
                opt => opt.MapFrom(src => src.Profile != null ? src.Profile.DateOfBirth : null))
             .ForMember(dest => dest.AvatarUrl,
                opt => opt.MapFrom(src => src.Profile != null ? src.Profile.AvatarUrl : string.Empty))
             .ForMember(dest => dest.RoleName,
                opt => opt.MapFrom(src => src.Role != null ? src.Role.Name : string.Empty))
             .ForMember(dest => dest.NumberOfFavorites,
                    opt => opt.MapFrom(src => src.Favorites != null ? src.Favorites.Count : 0))
             .ForMember(dest => dest.NumberOfHeritageReviews,
                    opt => opt.MapFrom(src => src.Reviews != null ? src.Reviews.Count : 0))
             .ForMember(dest => dest.NumberOfReports,
                    opt => opt.MapFrom(src => src.Reports != null ? src.Reports.Count : 0))
             .ForMember(dest => dest.NumberOfSubscriptions,
                    opt => opt.MapFrom(src => src.Subscriptions != null ? src.Subscriptions.Count : 0))
             .ForMember(dest => dest.NumberOfContributionSaves,
                    opt => opt.MapFrom(src => src.ContributionSaves != null ? src.ContributionSaves.Count : 0))
             .ForMember(dest => dest.NumberOfContributionReviews,
                    opt => opt.MapFrom(src => src.ContributionReviews != null ? src.ContributionReviews.Count : 0))
             .ForMember(dest => dest.NumberOfContributionReports,
                    opt => opt.MapFrom(src => src.ContributionReports != null ? src.ContributionReports.Count : 0));

            CreateMap<Staff, StaffSearchResponse>()
                .ForMember(dest => dest.UserName,
                opt => opt.MapFrom(src => src.User != null ? src.User.UserName : string.Empty))
                .ForMember(dest => dest.Email,
                opt => opt.MapFrom(src => src.User != null ? src.User.Email : string.Empty));
            CreateMap<Staff, StaffDetailResponse>()
                .ForMember(dest => dest.Email,
                opt => opt.MapFrom(src => src.User != null ? src.User.Email : string.Empty))
                .ForMember(dest => dest.Phone,
                opt => opt.MapFrom(src => src.User.Profile != null ? src.User.Profile.Phone : string.Empty))
                .ForMember(dest => dest.FullName,
                opt => opt.MapFrom(src => src.User.Profile != null ? src.User.Profile.FullName : string.Empty))
                .ForMember(dest => dest.Address,
                opt => opt.MapFrom(src => src.User.Profile != null ? src.User.Profile.Address : string.Empty))
                .ForMember(dest => dest.DateOfBirth,
                opt => opt.MapFrom(src => src.User.Profile != null ? src.User.Profile.DateOfBirth : null))
                .ForMember(dest => dest.AvatarUrl,
                opt => opt.MapFrom(src => src.User.Profile != null ? src.User.Profile.AvatarUrl : string.Empty))
                .ForMember(dest => dest.UserName,
                opt => opt.MapFrom(src => src.User != null ? src.User.UserName : string.Empty))
                .ForMember(dest => dest.Phone,
                opt => opt.MapFrom(src => src.User.Profile != null ? src.User.Profile.Phone : string.Empty))
                .ForMember(dest => dest.NumberOfContributionAcceptances,
                    opt => opt.MapFrom(src => src.ContributionAcceptances != null ? src.ContributionAcceptances.Count : 0))
                .ForMember(dest => dest.NumberOfReportReplies,
                    opt => opt.MapFrom(src => src.ReportReplies != null ? src.ReportReplies.Count : 0));

            CreateMap<PanoramaTour, PanoramaTourDetailResponse>();
            CreateMap<PanoramaTourCreationRequest, PanoramaTour>();
            CreateMap<PanoramaSceneCreationRequest, PanoramaScene>();
            CreateMap<PanoramaTour, PanoramaTourSearchResponse>()
                .ForMember(dest => dest.HeritageName,
                    opt => opt.MapFrom(src => src.Heritage != null ? src.Heritage.Name : null))
                .ForMember(dest => dest.NumberOfScenes,
                    opt => opt.MapFrom(src => src.Scenes.Count))
                .ForMember(dest => dest.HeritageLocations,
                    opt => opt.MapFrom(src =>
                        src.Heritage.HeritageLocations.Select(hl => hl.Location)
                    ));

            CreateMap<PanoramaTour, PanoramaTourSearchForAdminResponse>()
                .ForMember(dest => dest.HeritageName,
                    opt => opt.MapFrom(src => src.Heritage != null ? src.Heritage.Name : null))
                .ForMember(dest => dest.NumberOfScenes,
                    opt => opt.MapFrom(src => src.Scenes.Count));
            CreateMap<PanoramaTour, PanoramaTourDetailForAdminResponse>();


            // Chỉ cần 1 cái map StreamingRoom -> Response
            CreateMap<StreamingRoom, StreamingRoomResponse>();
            CreateMap<StreamingRoom, StreamingRoomDetailResponse>();
            CreateMap<StreamingParticipant, StreamingParticipantResponse>();

            CreateMap<EventCreateRequest, Event>()
                .ForMember(dest => dest.Status, opt => opt.Ignore())  // set trong service
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore()); // service tự set Id user

            CreateMap<EventUpdateRequest, Event>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            // Event → EventResponse
            CreateMap<Event, EventResponse>()
                // Nếu EventResponse.CreatedByUserName là string -> map trực tiếp từ CreatedBy (userId string)
                .ForMember(dest => dest.CreatedByUserName,
                    opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.RegisteredCount,
                    opt => opt.MapFrom(src =>
                        src.Registrations != null
                            ? src.Registrations.Count(r => !r.IsCancelled)
                            : 0))
                .ForMember(dest => dest.StreamingRooms,
                    opt => opt.MapFrom(src => src.StreamingRooms));


            // StreamingRoom → StreamingRoomSummaryResponse
            CreateMap<StreamingRoom, StreamingRoomSummaryResponse>();

            // EventRegistration → EventRegistrationResponse
            CreateMap<EventRegistration, EventRegistrationResponse>()
                .ForMember(dest => dest.EventId, opt => opt.MapFrom(src => src.EventId))
                .ForMember(dest => dest.Registered, opt => opt.MapFrom(src => !src.IsCancelled));
            CreateMap<EventRegistration, EventRegistrationResponse>()
    .ForMember(dest => dest.EventId, opt => opt.MapFrom(src => src.EventId))
    .ForMember(dest => dest.Registered, opt => opt.MapFrom(src => !src.IsCancelled));

            // 🔥 NEW: EventRegistration → EventRegistrationUserResponse (có thông tin user)
            CreateMap<EventRegistration, EventRegistrationUserResponse>()
                .ForMember(dest => dest.EventId, opt => opt.MapFrom(src => src.EventId))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.RegisteredAt, opt => opt.MapFrom(src => src.RegisteredAt));

            // PremiumPackage
            CreateMap<PremiumPackageCreateRequest, PremiumPackage>();
            CreateMap<PremiumPackageUpdateRequest, PremiumPackage>();
            CreateMap<PremiumPackageBenefit, PremiumPackageBenefitResponse>()
            .ForMember(dest => dest.BenefitId, opt => opt.MapFrom(src => src.Benefit.Id))
            .ForMember(dest => dest.BenefitName, opt => opt.MapFrom(src => src.Benefit.BenefitName))
            .ForMember(dest => dest.BenefitType, opt => opt.MapFrom(src => src.Benefit.BenefitType))
            .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Benefit.Value));

            CreateMap<PremiumPackage, PremiumPackageResponse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Currency))
                .ForMember(dest => dest.DurationDays, opt => opt.MapFrom(src => src.DurationDays))
                .ForMember(dest => dest.MarketingMessage, opt => opt.MapFrom(src => src.MarketingMessage))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.Benefits, opt => opt.MapFrom(src => src.PackageBenefits));

            CreateMap<PremiumBenefit, PremiumBenefitResponse>();
            CreateMap<PremiumBenefitCreateRequest, PremiumBenefit>();
            CreateMap<PremiumBenefitUpdateRequest, PremiumBenefit>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<UserPoint, UserPointResponse>();
        }


    }
}

