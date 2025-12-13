using Cultural_Heritage_System.Dtos.Models;
using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Request.ContribtutionReport;
using Cultural_Heritage_System.Dtos.Request.Contributor;
using Cultural_Heritage_System.Dtos.Request.Heritage;
using Cultural_Heritage_System.Dtos.Request.Panorama;
using Cultural_Heritage_System.Dtos.Request.Review;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Heritage;
using Cultural_Heritage_System.Dtos.Response.Panorama;
using Cultural_Heritage_System.Dtos.Response.Review;
using Cultural_Heritage_System.Models;

namespace Cultural_Heritage_System.Services
{
    public interface IPanoramaTourService
    {
        Task<PageResponse<PanoramaTourSearchResponse>> SearchPanoramaTour(PanoramaTourSearchRequest request);
        Task<bool> UpdatePanoramaTour(long id, PanoramaTourCreationRequest request);
        Task<long?> DeletePanoramaTour(long id);
        Task<bool> CreatePanoramaTour(PanoramaTourCreationRequest request);
        Task<PanoramaSceneResponse> GetPanoramaSceneDetail(long id);
        Task<PanoramaTourDetailResponse> GetPanoramaTourDetail(long id);
        Task<PageResponse<PanoramaTourSearchForAdminResponse>> GetListPanoramaTourForAdmin(PanoramaTourSearchRequest request);
        Task<PanoramaTourDetailForAdminResponse> GetPanoramaTourDetailForAdmin(long id);
        Task<bool> UpdatePanoramaScene(long id, PanoramaSceneCreationRequest request);
        Task<long?> DeletePanoramaScene(long id);
        Task<long> CreatePanoramaScene(PanoramaSceneCreationRequest request);
        Task<PanoramaSceneResponse> UnlockPanoramaScene(long sceneId);
    }
}

