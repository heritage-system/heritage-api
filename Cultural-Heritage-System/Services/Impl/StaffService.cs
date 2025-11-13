using AutoMapper;
using AutoMapper.QueryableExtensions;
using Azure.Core;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Request.Staff;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Heritage;
using Cultural_Heritage_System.Dtos.Response.Staff;
using Cultural_Heritage_System.Helpers;
using Cultural_Heritage_System.Middlewares;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Formats.Asn1;

namespace Cultural_Heritage_System.Services.Impl
{
    public class StaffService : IStaffService
    {
        private readonly IHttpContextAccessor httpContextAccessor;     
        private readonly IProfileRepository profileRepository;
        private readonly IRoleRepository roleRepository;
        private readonly PasswordHasher<Staff> passwordHasher;
        private readonly IMapper mapper;
        private readonly IMailService mailService;
        private readonly ILogger<StaffService> logger;
        private readonly ISubscriptionRepository subscriptionRepository;
        private readonly IContributorRepository contributorRepository;
        private readonly IStaffRepository staffRepository;
        public StaffService(IRoleRepository roleRepository, ILogger<StaffService> logger, IMailService mailService,
            IProfileRepository profileRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor, ISubscriptionRepository subscriptionRepository, IContributorRepository contributorRepository, IStaffRepository staffRepository)
        {         
            this.roleRepository = roleRepository;
            this.passwordHasher = new PasswordHasher<Staff>();
            this.logger = logger;
            this.profileRepository = profileRepository;
            this.mailService = mailService;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
            this.subscriptionRepository = subscriptionRepository;
            this.contributorRepository = contributorRepository;
            this.staffRepository = staffRepository;
        }

           
        public async Task<PageResponse<StaffSearchResponse>> SearchStaffForAdmin(StaffSearchRequest request)
        {
            var query = staffRepository.GetStaffsQueryable();      

            // ---- Filter ----
            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                var searchTerm = request.Keyword.Trim().ToLower();
                var unsignedTerm = StringHelper.RemoveDiacritics(searchTerm);

                query = query.Where(x =>                 
                    (x.User.Profile.FullName != null && x.User.Profile.FullName.ToLower().Contains(searchTerm)) ||
                    (x.User.Profile.FullNameUnsigned != null && x.User.Profile.FullNameUnsigned.Contains(unsignedTerm)) ||
                    (x.User.UserName != null && x.User.UserName.Contains(unsignedTerm)) ||
                    (x.User.UserNameUnsigned != null && x.User.UserNameUnsigned.Contains(unsignedTerm)) ||
                    (x.User.Email != null && x.User.Email.Contains(unsignedTerm))                
                );
            }

            if (request.Status.HasValue)
            {               
                query = query.Where(x => x.StaffStatus == request.Status);
            }

            // ---- Sort ----
            query = request.SortBy switch
            {
                SortBy.IDASC => query.OrderBy(x => x.Id),
                SortBy.IDDESC => query.OrderByDescending(x => x.Id),
                SortBy.NAMEASC => query.OrderBy(x => x.User.Profile.FullName),
                SortBy.NAMEDESC => query.OrderByDescending(x => x.User.Profile.FullName),
                SortBy.DATEASC => query.OrderBy(x => x.UpdatedAt.ToString() ?? x.CreatedAt.ToString()),
                SortBy.DATEDESC => query.OrderByDescending(x => x.UpdatedAt.ToString() ?? x.CreatedAt.ToString()),
                _ => query.OrderBy(x => x.Id)
            };

            // ---- Paging ----

            var dtoQuery = query.ProjectTo<StaffSearchResponse>(mapper.ConfigurationProvider);
            var paged = await dtoQuery.ToPagedResponseAsync(request.Page, request.PageSize);
            

            return paged;
        }

        public async Task<StaffDetailResponse> GetStaffDetailForAdmin(int id)
        {
            //var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
            //if (string.IsNullOrEmpty(accountIdClaim))
            //    throw new AppException(ErrorCode.UNAUTHORIZED);

            var staffOptinal = await staffRepository.GetStaffById(id);
            if (staffOptinal == null)
            {
                throw new AppException(ErrorCode.USER_NOT_EXISTED);
            }

            var response = mapper.Map<StaffDetailResponse>(staffOptinal);

            return response;
        }

        public async Task<bool> UpdateStaffForAdmin(int id, StaffUpdateRequest request)
        {
            // Lấy staffId của admin đang thao tác (người update)
            var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(accountIdClaim))
                throw new AppException(ErrorCode.UNAUTHORIZED);

            // Lấy Staff + User + Profile (đảm bảo repo Include đủ)
            var staff = await staffRepository.GetStaffById(id);
            if (staff == null)
                throw new AppException(ErrorCode.USER_NOT_EXISTED);

            // ---- Update vào Staff (các quyền & trạng thái) ----

            if (request.StaffStatus.HasValue)
            {
                staff.StaffStatus = request.StaffStatus.Value;
            }

            if (request.StaffRole.HasValue)
            {
                staff.StaffRole = request.StaffRole.Value;
            }

            if (request.CanManageEvents.HasValue)
            {
                staff.CanManageEvents = request.CanManageEvents.Value;
            }

            if (request.CanReplyReports.HasValue)
            {
                staff.CanReplyReports = request.CanReplyReports.Value;
            }

            if (request.CanAssignTasks.HasValue)
            {
                staff.CanAssignTasks = request.CanAssignTasks.Value;
            }

            staff.UpdatedAt = DateTime.UtcNow;
            staff.UpdatedBy = accountIdClaim;

            // ---- Update vào Profile (thông tin cá nhân) ----

            // Giả sử Staff luôn có User, và User có Profile (nếu không thì check null bảo vệ)
            var profile = staff.User?.Profile;
            if (profile != null)
            {
                bool profileChanged = false;

                if (!string.IsNullOrWhiteSpace(request.FullName))
                {
                    profile.FullName = request.FullName.Trim();
                    profile.GenerateUnsignedFields();
                    profileChanged = true;
                }

                if (!string.IsNullOrWhiteSpace(request.Phone))
                {
                    profile.Phone = request.Phone.Trim();
                    profileChanged = true;
                }

                if (!string.IsNullOrWhiteSpace(request.Address))
                {
                    profile.Address = request.Address.Trim();
                    profileChanged = true;
                }

                if (request.DateOfBirth.HasValue)
                {
                    profile.DateOfBirth = request.DateOfBirth.Value;
                    profileChanged = true;
                }

                if (profileChanged)
                {
                    profile.UpdatedAt = DateTime.UtcNow;
                    profile.UpdatedBy = accountIdClaim;
                    await profileRepository.UpdateAsync(profile);
                }
            }

            // ---- Lưu Staff ----
            await staffRepository.UpdateAsync(staff);

            return true;
        }

        private string ToVietnamese(string role)
        {
            return role switch
            {
                "GUEST" => "Khách",
                "MEMBER" => "Thành viên",
                "CONTRIBUTOR" => "Người đóng góp nội dung",
                "STAFF" => "Nhân viên kiểm duyệt",
                "ADMIN" => "Quản trị viên",
                _ => "Người dùng"
            };
        }
    }
}
