using Cultural_Heritage_System.Dtos.Request.PremiumPackage;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.PremiumPackage;
using Cultural_Heritage_System.Models;

namespace Cultural_Heritage_System.Services
{
    public interface IPremiumPackageService
    {
        Task<IEnumerable<PremiumPackageResponse>> GetAllAsync();
        IQueryable<PremiumPackage> GetPremiumPackagesQueryable();
        Task<PremiumPackageResponse?> GetByIdAsync(int id);
        Task<PremiumPackageResponse> CreateAsync(PremiumPackageCreateRequest request);
        Task<PremiumPackageResponse> UpdateAsync(int id, PremiumPackageUpdateRequest request);
        Task<bool> DeleteAsync(int id);
        Task<PageResponse<PremiumPackageResponse>> SearchPremiumPackagesAsync(PremiumPackageSearchRequest request);
        Task<IEnumerable<PremiumPackageResponse>> GetActivePackagesAsync();
    }

}
