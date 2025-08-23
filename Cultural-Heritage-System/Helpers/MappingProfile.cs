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

            //Heritage
            CreateMap<HeritageCreateRequest, Heritage>();
            CreateMap<HeritageUpdateRequest, Heritage>();
            CreateMap<Heritage, HeritageResponse>();
            CreateMap<HeritageMedia, MediaResponse>();
            CreateMap<Tag, TagResponse>();
            CreateMap<Location, LocationResponse>();
            CreateMap<HeritageOccurrence, OccurrenceResponse>();
            CreateMap<HeritageCoordinate, CoordinateResponse>();

        }
    }
}

