using Cultural_Heritage_System.Dtos.Models;
using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Request.ContribtutionReport;
using Cultural_Heritage_System.Dtos.Request.Heritage;
using Cultural_Heritage_System.Dtos.Request.Review;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Panorama;
using Cultural_Heritage_System.Dtos.Response.Heritage;
using Cultural_Heritage_System.Dtos.Response.Review;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Dtos.Request.Panorama;

namespace Cultural_Heritage_System.Services
{
    public interface IPanoramaTourService
    {
        //Task<bool> CreatePanoramaTour(PanoramaTourCreationRequest request);
        Task<PanoramaSceneResponse> GetPanoramaSceneDetail(long id);    
    }
}

