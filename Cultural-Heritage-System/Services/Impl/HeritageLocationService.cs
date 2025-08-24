using AutoMapper;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;

namespace Cultural_Heritage_System.Services.Impl
{
    public class HeritageLocationService : IHeritageLocationService
    {
        private readonly HeritageLocationRepository heritageLocationRepository;
        private readonly IMapper mapper;

        public HeritageLocationService(HeritageLocationRepository heritageLocationRepository, IMapper mapper)
        {
            this.heritageLocationRepository = heritageLocationRepository;
            this.mapper = mapper;
        }

        public async Task<List<HeritageLocationResponse>> GetHeritagesWithCoordinates()
        {
            var heritages = await heritageLocationRepository.GetAllWithCoordinatesAndLocations();

            return heritages.Select(h => new HeritageLocationResponse
            {
                HeritageId = h.Id,
                Name = h.Name,
                Description = h.Description,
                Coordinates = mapper.Map<List<HeritageCoordinateDto>>(h.Coordinates),
                Locations = mapper.Map<List<HeritageLocationDto>>(h.HeritageLocations),
            }).ToList();
        }
    }
}
