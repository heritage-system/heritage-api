using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Response;

namespace Cultural_Heritage_System.Services.Impl
{
    public interface IHeritageService
    {
        Task<PageResponse<HeritageResponse>> GetAllAsync(
    int page,
    int pageSize,
    string? keyword = null,
    int? categoryId = null,
    int? tagId = null);
        Task<HeritageResponse> GetByIdAsync(long id);
        Task<HeritageResponse> CreateAsync(HeritageCreateRequest request);
        Task<HeritageResponse> UpdateAsync(long id, HeritageUpdateRequest request);
        Task<bool> DeleteAsync(long id);
    }
}
