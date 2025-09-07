using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Request.Category;
using Cultural_Heritage_System.Dtos.Request.Contributor;
using Cultural_Heritage_System.Dtos.Request.Heritage;
using Cultural_Heritage_System.Dtos.Request.Location;
using Cultural_Heritage_System.Dtos.Request.Media;
using Cultural_Heritage_System.Dtos.Request.Occurrence;
using Cultural_Heritage_System.Dtos.Request.Report;
using Cultural_Heritage_System.Dtos.Request.Review;
using Cultural_Heritage_System.Dtos.Request.Tag;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Category;
using Cultural_Heritage_System.Dtos.Response.Contributor;
using Cultural_Heritage_System.Dtos.Response.Heritage;
using Cultural_Heritage_System.Dtos.Response.Location;
using Cultural_Heritage_System.Dtos.Response.Media;
using Cultural_Heritage_System.Dtos.Response.Occurence;
using Cultural_Heritage_System.Dtos.Response.Report;
using Cultural_Heritage_System.Dtos.Response.Review;
using Cultural_Heritage_System.Dtos.Response.Tag;
using Cultural_Heritage_System.Models;


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
            CreateMap<HeritageCreateRequest, Heritage>();
            //.ForMember(dest => dest.HeritageLocations, opt => opt.MapFrom(src => src.Locations))
            //.ForMember(dest => dest.Media, opt => opt.MapFrom(src => src.Media))
            //.ForMember(dest => dest.HeritageOccurrences, opt => opt.MapFrom(src => src.Occurrences))
            //.ForMember(dest => dest.HeritageTags, opt => opt.MapFrom(src => src.TagIds));
            CreateMap<HeritageUpdateRequest, Heritage>();
            CreateMap<Heritage, HeritageResponse>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
            .ForMember(dest => dest.Media, opt => opt.MapFrom(src => src.Media))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.HeritageTags.Select(ht => ht.Tag)))
            .ForMember(dest => dest.Locations, opt => opt.MapFrom(src => src.HeritageLocations.Select(hl => hl.Location)))
            .ForMember(dest => dest.Occurrences, opt => opt.MapFrom(src => src.HeritageOccurrences));

            CreateMap<HeritageMedia, MediaResponse>();
            CreateMap<Tag, TagResponse>();
            CreateMap<Location, LocationResponse>();
            CreateMap<HeritageOccurrence, OccurrenceResponse>();

            CreateMap<LocationRequest, HeritageLocation>();
            CreateMap<MediaRequest, HeritageMedia>();
            CreateMap<OccurrenceRequest, HeritageOccurrence>();

            CreateMap(typeof(PageResponse<>), typeof(PageResponse<>));

            CreateMap<CreateReportRequest, Report>();
            CreateMap<Report, ReportResponse>();
            CreateMap<UpdateReportRequest, Report>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Contributor, ContributorResponse>()
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User != null ? src.User.Email : null))
                .ForMember(dest => dest.Count, opt => opt.MapFrom(src => src.Contributions.Count));

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
             .ForMember(dest => dest.UserImageUrl, opt => opt.MapFrom(src => src.User.UserImageUrl))
             .ForMember(dest => dest.Likes, opt => opt.MapFrom(src => src.Likes != null ? src.Likes.Count : 0))
             .ForMember(dest => dest.LikedByMe, opt => opt.Ignore()) // set manually based on current user
             .ForMember(dest => dest.ReviewMedias, opt => opt.MapFrom(src => src.ReviewMedias))
             .ForMember(dest => dest.Replies, opt => opt.MapFrom(src => src.Replies))
             .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));
            CreateMap<ReviewResponse, Review>();
            // ReviewMedia → ReviewMediaResponse
            CreateMap<ReviewMedia, ReviewMediaResponse>()
                .ForMember(dest => dest.MediaType, opt => opt.MapFrom(src => src.MediaType.ToString()));

            // ReviewCreateRequest → Review
            CreateMap<ReviewCreateRequest, Review>()
                .ForMember(dest => dest.HeritageId, opt => opt.MapFrom(src => src.HeritageId))
                .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment))
                .ForMember(dest => dest.ParentReviewId, opt => opt.MapFrom(src => src.ParentReviewId))
                .ForMember(dest => dest.ReviewMedias, opt => opt.Ignore()) // handled manually after file upload
                .ForMember(dest => dest.Likes, opt => opt.Ignore())
                .ForMember(dest => dest.Reports, opt => opt.Ignore())
                .ForMember(dest => dest.Replies, opt => opt.Ignore());

            // ReviewMediaRequest → ReviewMedia
            CreateMap<ReviewMediaRequest, ReviewMedia>()
                .ForMember(dest => dest.Url, opt => opt.Ignore()) // file upload decides the URL
                .ForMember(dest => dest.MediaType, opt => opt.MapFrom(src => src.Type));


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
        }

    }
}

