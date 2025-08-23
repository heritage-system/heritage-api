using AutoMapper;
using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;

namespace Cultural_Heritage_System.Services.Impl
{
    public class HeritageService : IHeritageService
    {
        private readonly HeritageRepository _heritageRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<HeritageService> _logger;

        public HeritageService(HeritageRepository heritageRepository, IMapper mapper, ILogger<HeritageService> logger)
        {
            _heritageRepository = heritageRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<HeritageResponse>> GetAllAsync()
        {
            var heritages = await _heritageRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<HeritageResponse>>(heritages);
        }

        public async Task<HeritageResponse> GetByIdAsync(long id)
        {
            var heritage = await _heritageRepository.GetByIdAsync(id);
            if (heritage == null)
                return null;

            return _mapper.Map<HeritageResponse>(heritage);
        }

        public async Task<HeritageResponse> CreateAsync(HeritageCreateRequest request, string createdBy)
        {
            var heritage = _mapper.Map<Heritage>(request);
            heritage.CreatedBy = createdBy;
            heritage.CreatedAt = DateTime.UtcNow;

            await _heritageRepository.AddAsync(heritage);

            return _mapper.Map<HeritageResponse>(heritage);
        }

        public async Task<HeritageResponse> UpdateAsync(long id, HeritageUpdateRequest request, string updatedBy)
        {
            var heritage = await _heritageRepository.GetByIdAsync(id);
            if (heritage == null)
                return null;

            _mapper.Map(request, heritage);
            heritage.UpdatedBy = updatedBy;
            heritage.UpdatedAt = DateTime.UtcNow;

            await _heritageRepository.UpdateAsync(heritage);

            return _mapper.Map<HeritageResponse>(heritage);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var heritage = await _heritageRepository.GetByIdAsync(id);
            if (heritage == null)
                return false;

            await _heritageRepository.DeleteAsync(heritage);
            return true;
        }
    }
}
