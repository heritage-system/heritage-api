using AutoMapper;
using CloudinaryDotNet.Actions;
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

            CreateMap<HeritageCoordinate, CoordinateDto>();
            CreateMap<Location, LocationDto>();
            CreateMap<Heritage, HeritageLocationResponse>()
                .ForMember(dest => dest.HeritageId, opt => opt.MapFrom(src => src.Id)) 
                .ForMember(dest => dest.Coordinates, opt => opt.MapFrom(src => src.Coordinates))
                .ForMember(dest => dest.Locations, opt => opt.MapFrom(src =>
                    src.HeritageLocations.Select(hl => hl.Location)
                ));
        }
    }
}

