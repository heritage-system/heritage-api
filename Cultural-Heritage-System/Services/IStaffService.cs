using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Request.Staff;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Staff;

namespace Cultural_Heritage_System.Services
{
    public interface IStaffService
    {             
        Task<PageResponse<StaffSearchResponse>> SearchStaffForAdmin(StaffSearchRequest request);     
        Task<StaffDetailResponse> GetStaffDetailForAdmin(int id);
        Task<bool> UpdateStaffForAdmin(int id, StaffUpdateRequest request);

    }
}
