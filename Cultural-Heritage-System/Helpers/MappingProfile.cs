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

            CreateMap<Favorite, FavoriteHeritageResponse>()
                .ForMember(dest => dest.HeritageId, opt => opt.MapFrom(src => src.HeritageId))
                .ForMember(dest => dest.HeritageName, opt => opt.MapFrom(src => src.Heritage.Name))
                .ForMember(dest => dest.HeritageDescription, opt => opt.MapFrom(src => src.Heritage.Description))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Heritage.Category.Name))
                .ForMember(dest => dest.IsFeatured, opt => opt.MapFrom(src => src.Heritage.IsFeatured))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));
        }
    }
}

