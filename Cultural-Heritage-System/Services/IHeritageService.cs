using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Response;

namespace Cultural_Heritage_System.Services.Impl
{
    public interface IHeritageService
    {
        Task<IEnumerable<HeritageResponse>> GetAllAsync();
        Task<HeritageResponse> GetByIdAsync(long id);
        Task<HeritageResponse> CreateAsync(HeritageCreateRequest request, string createdBy);
        Task<HeritageResponse> UpdateAsync(long id, HeritageUpdateRequest request, string updatedBy);
        Task<bool> DeleteAsync(long id);
    }
}
