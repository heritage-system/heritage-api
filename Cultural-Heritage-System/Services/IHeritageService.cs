using Cultural_Heritage_System.Dtos.Request.Heritage;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Contribution;
using Cultural_Heritage_System.Dtos.Response.Heritage;

namespace Cultural_Heritage_System.Services.Impl
{
    public interface IHeritageService
    {
        Task<PageResponse<HeritageResponse>> GetAllAsync(int page,int pageSize,string? keyword = null,int? categoryId = null,int? tagId = null);
        Task<HeritageResponse> GetByIdAsync(long id);
        Task<HeritageResponse> CreateAsync(HeritageCreateRequest request);
        Task<HeritageResponse> UpdateAsync(HeritageUpdateRequest request);
        Task<long?> DeleteAsync(long id);
        Task<List<HeritageNameSearchResponse>> SearchListHeritageName(string? keyword);
        Task<PageResponse<HeritageSearchResponse>> SearchHeritagesAsync(HeritageSearchRequest request);
        Task<HeritageDetailResponse> GetHeritageDetail(long id);
        Task<List<HeritageRelatedResponse>> GetHeritageRelated(HeritageRelatedRequest request);
    }
}
