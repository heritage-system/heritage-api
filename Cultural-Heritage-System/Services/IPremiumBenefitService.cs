using Cultural_Heritage_System.Dtos.Request.PremiumBenefit;
using Cultural_Heritage_System.Dtos.Response.PremiumBenefit;

namespace Cultural_Heritage_System.Services
{
    public interface IPremiumBenefitService
    {
        Task<IEnumerable<PremiumBenefitResponse>> GetAllAsync();
        Task<PremiumBenefitResponse?> GetByIdAsync(int id);
        Task<PremiumBenefitResponse> CreateAsync(PremiumBenefitCreateRequest req);
        Task<PremiumBenefitResponse> UpdateAsync(int id, PremiumBenefitUpdateRequest req);
        Task<bool> DeleteAsync(int id);
    }
}
