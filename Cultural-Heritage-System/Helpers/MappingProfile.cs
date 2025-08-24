using AutoMapper;
using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Response;
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
            .ForMember(dest => dest.HeritageOccurrences,
                opt => opt.MapFrom(src => src.HeritageOccurrences))
            .ForMember(dest => dest.Media,
                opt => opt.MapFrom(src => src.Media))
            .ForMember(dest => dest.HeritageLocations,
                opt => opt.MapFrom(src => src.HeritageLocations.Select(hl => hl.Location)))
            .ForMember(dest => dest.Coordinates,
                opt => opt.MapFrom(src => src.Coordinates));

            // HeritageOccurrence → HeritageOccurrenceDto
            CreateMap<HeritageOccurrence, HeritageOccurrenceDto>()              
                .ForMember(dest => dest.OccurrenceTypeName, opt => opt.MapFrom(src => src.OccurrenceType.ToString()))               
                .ForMember(dest => dest.CalendarTypeName, opt => opt.MapFrom(src => src.CalendarType.HasValue ? src.CalendarType.Value.ToString() : null))               
                .ForMember(dest => dest.FrequencyName, opt => opt.MapFrom(src => src.Frequency.HasValue ? src.Frequency.Value.ToString() : null));

            // HeritageMedia → HeritageMediaDto
            CreateMap<HeritageMedia, HeritageMediaDto>()             
                .ForMember(dest => dest.MediaTypeName, opt => opt.MapFrom(src => src.MediaType.ToString()));

            // HeritageCoordinate → HeritageCoordinateDto
            CreateMap<HeritageCoordinate, HeritageCoordinateDto>();

            // Location → HeritageLocationDto
            CreateMap<Location, HeritageLocationDto>();
        }
    }
}

