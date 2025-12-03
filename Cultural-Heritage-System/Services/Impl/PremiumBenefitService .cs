using AutoMapper;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Request.PremiumBenefit;
using Cultural_Heritage_System.Dtos.Response.PremiumBenefit;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;

namespace Cultural_Heritage_System.Services.Impl
{
    public class PremiumBenefitService : IPremiumBenefitService
    {
        private readonly IPremiumBenefitRepository _repo;
        private readonly IMapper _mapper;

        public PremiumBenefitService(IPremiumBenefitRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PremiumBenefitResponse>> GetAllAsync()
        {
            var data = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<PremiumBenefitResponse>>(data);
        }

        public async Task<PremiumBenefitResponse?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<PremiumBenefitResponse>(entity);
        }

        public async Task<PremiumBenefitResponse> CreateAsync(PremiumBenefitCreateRequest req)
        {
            if (req.BenefitType == BenefitType.LIMITINCREASE && string.IsNullOrWhiteSpace(req.Value.ToString()))
            {
                throw new ArgumentException("Value là bắt buộc khi BenefitType là LIMITINCREASE");
            }

            if (req.BenefitType == BenefitType.FEATUREUNLOCK)
            {
                req.Value = null;
            }

            var entity = _mapper.Map<PremiumBenefit>(req);
            await _repo.AddAsync(entity);
            return _mapper.Map<PremiumBenefitResponse>(entity);
        }



        public async Task<PremiumBenefitResponse> UpdateAsync(int id, PremiumBenefitUpdateRequest req)
        {
            var entity = await _repo.GetByIdAsync(id) ?? throw new Exception("Benefit not found");

            _mapper.Map(req, entity); 

            await _repo.UpdateAsync(entity);
            return _mapper.Map<PremiumBenefitResponse>(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return false;
            await _repo.DeleteAsync(entity);
            return true;
        }
    }

}
