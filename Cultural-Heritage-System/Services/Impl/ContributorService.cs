using AutoMapper;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Request.Contributor;
using Cultural_Heritage_System.Dtos.Response.Contributor;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Helpers;
using Cultural_Heritage_System.Middlewares;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Cultural_Heritage_System.Services;
using Microsoft.EntityFrameworkCore;

public class ContributorService : IContributorService
{
    private readonly ContributorRepository contributorRepository;
    private readonly ILogger<ContributorService> logger;
    private readonly IMapper mapper;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly ProfileRepository profileRepository;
    private readonly UserRepository userRepository;
    private readonly RoleRepository roleRepository;
    private readonly IUserCacheService userCacheService;

    public ContributorService(
        ContributorRepository contributorRepository,
        ILogger<ContributorService> logger,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor,
        ProfileRepository profileRepository,
        UserRepository userRepository,
        RoleRepository roleRepository,
        IUserCacheService userCacheService
    )
    {
        this.contributorRepository = contributorRepository;
        this.logger = logger;
        this.mapper = mapper;
        this.httpContextAccessor = httpContextAccessor;
        this.profileRepository = profileRepository;
        this.userRepository = userRepository;
        this.roleRepository = roleRepository;
        this.userCacheService = userCacheService;
    }

    public async Task<PageResponse<ContributorResponse>> SearchContributorsAsync(ContributorSearchRequest request)
    {
        var query = contributorRepository.GetContributorsQueryable()
         .Select(c => new ContributorResponse
         {
             Id = c.Id,
             Bio = c.Bio,
             Expertise = c.Expertise,
             Status = c.Status.ToString(),
             UserId = c.UserId,
             UserEmail = c.User.Email,
             UserFullName = c.User.Profile.FullName,           
             FullNameUnsigned = c.User.Profile.FullNameUnsigned,
             ExpertiseUnsigned = c.ExpertiseUnsigned,
             Count = c.Contributions.Count,
             CreatedAt = c.CreatedAt,
             UpdatedAt = c.UpdatedAt,
             CreatedBy = c.CreatedBy,
             UpdatedBy = c.UpdatedBy
         });

        // ---- Filter ----
        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var searchTerm = request.Keyword.Trim().ToLower();
            var unsignedTerm = StringHelper.RemoveDiacritics(searchTerm);

            query = query.Where(x =>
                (x.Expertise != null && x.Expertise.ToLower().Contains(searchTerm)) ||
                (x.UserFullName != null && x.UserFullName.ToLower().Contains(searchTerm)) ||
                (x.FullNameUnsigned != null && x.FullNameUnsigned.Contains(unsignedTerm)) ||
                (x.ExpertiseUnsigned != null && x.ExpertiseUnsigned.Contains(unsignedTerm)) ||
                (x.UserEmail != null && x.UserEmail.ToLower().Contains(searchTerm))
            );
        }

        if (request.Status.HasValue)
        {
            var statusString = request.Status.Value.ToString();

            query = query.Where(x => x.Status == statusString);
        }

        // ---- Sort ----
        query = request.SortBy switch
        {
            SortBy.IDASC => query.OrderBy(x => x.Id),
            SortBy.IDDESC => query.OrderByDescending(x => x.Id),
            SortBy.NAMEASC => query.OrderBy(x => x.UserFullName),
            SortBy.NAMEDESC => query.OrderByDescending(x => x.UserFullName),
            SortBy.DATEASC => query.OrderBy(x => x.UpdatedAt ?? x.CreatedAt),
            SortBy.DATEDESC => query.OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt),
            _ => query.OrderBy(x => x.Id)
        };

        // ---- Paging ----
        var paged = await query.ToPagedResponseAsync(request.Page, request.PageSize);

        // ---- Resolve CreatedBy / UpdatedBy ----
        foreach (var item in paged.Items)
        {
            if (!string.IsNullOrEmpty(item.CreatedBy) && item.CreatedBy != "system")
            {
                var user = await userRepository.GetByIdAsync(int.Parse(item.CreatedBy));
                if (user != null)
                {
                    item.CreatedByEmail = user.Email;
                    var profile = await profileRepository.GetProfileByUserIdAsync(user.Id);
                    item.CreatedByName = profile?.FullName;
                }
            }
            else if (item.CreatedBy == "system")
            {
                item.CreatedByName = "System";
                item.CreatedByEmail = "system";
            }

            if (!string.IsNullOrEmpty(item.UpdatedBy) && item.UpdatedBy != "system")
            {
                var user = await userRepository.GetByIdAsync(int.Parse(item.UpdatedBy));
                if (user != null)
                {
                    item.UpdatedByEmail = user.Email;
                    var profile = await profileRepository.GetProfileByUserIdAsync(user.Id);
                    item.UpdatedByName = profile?.FullName;
                }
            }
            else if (item.UpdatedBy == "system")
            {
                item.UpdatedByName = "System";
                item.UpdatedByEmail = "system";
            }
        }

        return paged;
    }

    public async Task<ContributorResponse?> GetContributorDetail(int id)
    {
        var contributor = await contributorRepository.GetContributorById(id);
        if (contributor == null) return null;

        var response = mapper.Map<ContributorResponse>(contributor);
        response.UserFullName = contributor.User?.Profile?.FullName;

        return response;
    }

    public async Task<ContributorResponse> CreateContributor(ContributorCreateRequest request)
    {
        var targetUser = await userRepository.FindUserById(request.UserId);
        if (targetUser == null)
        {
            throw new AppException(ErrorCode.USER_NOT_EXISTED);
        }

        if (targetUser.Role?.Name != DefinitionRole.MEMBER)
        {
            throw new AppException(ErrorCode.INVALID_ROLE);
        }

        var existedContributor = await contributorRepository
            .GetContributorsQueryable()
            .FirstOrDefaultAsync(c => c.UserId == targetUser.Id);

        if (existedContributor != null)
        {
            throw new AppException(ErrorCode.CONTRIBUTOR_EXISTED);
        }

        var contributorRole = await roleRepository.FindByRoleName(DefinitionRole.CONTRIBUTOR);
        if (contributorRole == null)
        {
            contributorRole = new Role { Name = DefinitionRole.CONTRIBUTOR };
            await roleRepository.CreateRole(contributorRole);
        }

        targetUser.RoleId = contributorRole.Id;
        await userRepository.UpdateAsync(targetUser);

        var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value ?? "system";

        var contributor = new Contributor
        {
            UserId = targetUser.Id,
            Bio = request.Bio,
            Expertise = request.Expertise,
            Verified = false,
            Status = ContributorStatus.APPLIED,
            CreatedBy = accountIdClaim,
            UpdatedBy = accountIdClaim
        };

        await contributorRepository.AddAsync(contributor);

        return mapper.Map<ContributorResponse>(contributor);
    }

    public async Task<ContributorResponse> UpdateContributor(int id, ContributorUpdateRequest request)
    {
        var contributor = await contributorRepository.GetContributorById(id);
        if (contributor == null)
        {
            throw new AppException(ErrorCode.CONTRIBUTOR_NOT_EXISTED);
        }

        contributor.Bio = request.Bio;
        contributor.Expertise = request.Expertise;

        // parse string sang enum (nếu null thì giữ nguyên status cũ)
        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (Enum.TryParse<ContributorStatus>(request.Status, true, out var parsedStatus))
            {
                contributor.Status = parsedStatus;
            }
            else
            {
                throw new AppException(ErrorCode.INVALID_STATUS);
            }
        }

        var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value ?? "system";
        contributor.UpdatedBy = accountIdClaim;
        contributor.UpdatedAt = DateTime.UtcNow;

        var targetUser = await userRepository.FindUserById(contributor.UserId);
        if (targetUser == null)
        {
            throw new AppException(ErrorCode.USER_NOT_EXISTED);
        }

        // giữ logic role update như cũ
        if (contributor.Status == ContributorStatus.ACTIVE)
        {
            var contributorRole = await roleRepository.FindByRoleName(DefinitionRole.CONTRIBUTOR);
            if (contributorRole == null)
            {
                contributorRole = new Role { Name = DefinitionRole.CONTRIBUTOR };
                await roleRepository.CreateRole(contributorRole);
            }
            targetUser.RoleId = contributorRole.Id;
            await userRepository.UpdateAsync(targetUser);
        }
        else if (contributor.Status == ContributorStatus.REJECTED || contributor.Status == ContributorStatus.SUSPENDED)
        {
            var memberRole = await roleRepository.FindByRoleName(DefinitionRole.MEMBER);
            if (memberRole == null)
            {
                memberRole = new Role { Name = DefinitionRole.MEMBER };
                await roleRepository.CreateRole(memberRole);
            }
            targetUser.RoleId = memberRole.Id;
            await userRepository.UpdateAsync(targetUser);
        }

        await contributorRepository.UpdateAsync(contributor);
        return mapper.Map<ContributorResponse>(contributor);
    }

    public async Task DisableContributor(int id)
    {
        var contributor = await contributorRepository.GetContributorById(id);
        if (contributor == null)
        {
            throw new AppException(ErrorCode.CONTRIBUTOR_NOT_EXISTED);
        }

        contributor.Status = ContributorStatus.SUSPENDED;
        await contributorRepository.UpdateAsync(contributor);
    }

    public async Task<List<DropdownUserResponse>> SearchDropdownUserAsync(string? keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            // Lấy từ cache trước
            var cached = await userCacheService.GetCachedUsersAsync();
            if (cached.Count > 0)
                return cached.Take(10).ToList();

            // Cache miss -> query DB
            var latestUsers = await userRepository.GetQueryable()
                .Where(u => u.Role.Name == DefinitionRole.MEMBER)
                .OrderByDescending(u => u.CreatedAt)
                .Take(50)
                .Select(u => new DropdownUserResponse
                {
                    Id = u.Id,
                    Email = u.Email,
                    FullName = u.Profile.FullName
                })
                .ToListAsync();

            await userCacheService.SetCachedUsersAsync(latestUsers);
            return latestUsers.Take(10).ToList();
        }
        else
        {
            var search = keyword.Trim().ToLower();
            return await userRepository.GetQueryable()
                .Where(u => u.Role.Name == DefinitionRole.MEMBER &&
                       ((u.Email != null && u.Email.ToLower().Contains(search)) ||
                        (u.Profile.FullName != null && u.Profile.FullName.ToLower().Contains(search))))
                .OrderByDescending(u => u.CreatedAt)
                .Take(10)
                .Select(u => new DropdownUserResponse
                {
                    Id = u.Id,
                    Email = u.Email,
                    FullName = u.Profile.FullName
                })
                .ToListAsync();
        }
    }

    public async Task<ContributorResponse> ApproveContributor(int id)
    {
        var contributor = await contributorRepository.GetContributorById(id);
        if (contributor == null)
        {
            throw new AppException(ErrorCode.CONTRIBUTOR_NOT_EXISTED);
        }

        if (contributor.Status == ContributorStatus.SUSPENDED)
        {
            throw new AppException(ErrorCode.CONTRIBUTOR_DISABLED);
        }

        contributor.Verified = true;
        contributor.Status = ContributorStatus.ACTIVE;

        var targetUser = await userRepository.FindUserById(contributor.UserId);
        if (targetUser == null)
        {
            throw new AppException(ErrorCode.USER_NOT_EXISTED);
        }

        var contributorRole = await roleRepository.FindByRoleName(DefinitionRole.CONTRIBUTOR);
        if (contributorRole == null)
        {
            contributorRole = new Role { Name = DefinitionRole.CONTRIBUTOR };
            await roleRepository.CreateRole(contributorRole);
        }

        targetUser.RoleId = contributorRole.Id;
        await userRepository.UpdateAsync(targetUser);

        await contributorRepository.UpdateAsync(contributor);

        return mapper.Map<ContributorResponse>(contributor);
    }

    public async Task<ContributorResponse> RejectContributor(int id)
    {
        var contributor = await contributorRepository.GetContributorById(id);
        if (contributor == null)
        {
            throw new AppException(ErrorCode.CONTRIBUTOR_NOT_EXISTED);
        }

        // Reject thì vẫn giữ record, chỉ đổi trạng thái
        contributor.Verified = false;
        contributor.Status = ContributorStatus.REJECTED;

        await contributorRepository.UpdateAsync(contributor);
        return mapper.Map<ContributorResponse>(contributor);
    }

    public async Task<ContributorResponse> ApplyContributor(ContributorApplyRequest request)
    {
        var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(accountIdClaim))
        {
            throw new AppException(ErrorCode.UNAUTHORIZED);
        }

        var userId = int.Parse(accountIdClaim);

        var targetUser = await userRepository.FindUserById(userId);
        if (targetUser == null)
        {
            throw new AppException(ErrorCode.USER_NOT_EXISTED);
        }

        // Chỉ member mới được apply
        if (targetUser.Role?.Name != DefinitionRole.MEMBER)
        {
            throw new AppException(ErrorCode.INVALID_ROLE);
        }

        // Check nếu đã apply / contributor rồi
        var existedContributor = await contributorRepository
            .GetContributorsQueryable()
            .FirstOrDefaultAsync(c => c.UserId == targetUser.Id);

        if (existedContributor != null)
        {
            throw new AppException(ErrorCode.CONTRIBUTOR_EXISTED);
        }

        var contributor = new Contributor
        {
            UserId = targetUser.Id,
            Bio = request.Bio,
            Expertise = request.Expertise,
            DocumentsUrl = request.DocumentsUrl,
            Verified = false,
            Status = ContributorStatus.APPLIED,
            CreatedBy = accountIdClaim,
            UpdatedBy = accountIdClaim
        };

        await contributorRepository.AddAsync(contributor);

        return mapper.Map<ContributorResponse>(contributor);
    }

    public async Task<ContributorApplyResponse?> GetContributorApplication()
    {
        var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(accountIdClaim))
        {
            throw new AppException(ErrorCode.UNAUTHORIZED);
        }

        var userId = int.Parse(accountIdClaim);

        var contributor = await contributorRepository
            .GetContributorsQueryable()
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (contributor == null)
            return null;

        return mapper.Map<ContributorApplyResponse>(contributor);
    }

}