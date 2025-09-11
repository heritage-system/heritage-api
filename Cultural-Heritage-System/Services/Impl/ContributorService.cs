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

    public ContributorService(
        ContributorRepository contributorRepository,
        ILogger<ContributorService> logger,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor,
        ProfileRepository profileRepository,
        UserRepository userRepository,
        RoleRepository roleRepository
    )
    {
        this.contributorRepository = contributorRepository;
        this.logger = logger;
        this.mapper = mapper;
        this.httpContextAccessor = httpContextAccessor;
        this.profileRepository = profileRepository;
        this.userRepository = userRepository;
        this.roleRepository = roleRepository;
    }

    public async Task<PageResponse<ContributorResponse>> SearchContributorsAsync(ContributorSearchRequest request)
    {
        var query = from c in contributorRepository.GetContributorsQueryable()
                    join p in profileRepository.GetQueryable() on c.UserId equals p.UserId into cp
                    from profile in cp.DefaultIfEmpty()
                    select new ContributorResponse
                    {
                        Id = c.Id,
                        Bio = c.Bio,
                        Expertise = c.Expertise,
                        Verified = c.Verified,
                        Status = c.Status,
                        UserId = c.UserId,
                        UserEmail = c.User.Email,
                        UserFullName = profile.FullName,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt,
                        CreatedBy = c.CreatedBy,
                        UpdatedBy = c.UpdatedBy,
                        FullNameUnsigned = profile.FullNameUnsigned,
                        ExpertiseUnsigned = c.ExpertiseUnsigned,
                        Count = c.Contributions.Count()
                    };

        // ---- Filter ----
        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var searchTerm = request.Keyword.Trim().ToLower();
            var unsignedTerm = StringHelper.RemoveDiacritics(searchTerm);

            query = query.Where(x =>
                (x.Expertise != null && x.Expertise.ToLower().Contains(searchTerm)) ||
                (x.UserFullName != null && x.UserFullName.ToLower().Contains(searchTerm)) ||
                (x.FullNameUnsigned != null && x.FullNameUnsigned.Contains(unsignedTerm)) ||
                (x.ExpertiseUnsigned != null && x.ExpertiseUnsigned.Contains(unsignedTerm))
            );
        }

        if (request.Verified.HasValue)
        {
            query = query.Where(x => x.Verified == request.Verified.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(x => x.Status == request.Status.Value);
        }

        // ---- Sort ----
        query = request.SortBy switch
        {
            SortBy.IDASC => query.OrderBy(x => x.Id),
            SortBy.IDDESC => query.OrderByDescending(x => x.Id),
            SortBy.NAMEASC => query.OrderBy(x => x.UserFullName),
            SortBy.NAMEDESC => query.OrderByDescending(x => x.UserFullName),
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

        var profile = await profileRepository.GetProfileByUserIdAsync(contributor.UserId);
        if (profile != null)
        {
            response.UserFullName = profile.FullName;
        }

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
        contributor.Verified = request.Verified;
        contributor.Status = request.Status;

        var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value ?? "system";
        contributor.UpdatedBy = accountIdClaim;
        contributor.UpdatedAt = DateTime.UtcNow;

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
        var query = from u in userRepository.GetQueryable()
                    join p in profileRepository.GetQueryable()
                        on u.Id equals p.UserId into up
                    from profile in up.DefaultIfEmpty()
                    where u.Role.Name == DefinitionRole.MEMBER
                    select new { u, profile };

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var search = keyword.Trim().ToLower();
            query = query.Where(x =>
                (x.u.Email != null && x.u.Email.ToLower().Contains(search)) ||
                (x.profile.FullName != null && x.profile.FullName.ToLower().Contains(search))
            );
        }

        return await query
            .Take(50)
            .Select(x => new DropdownUserResponse
            {
                Id = x.u.Id,
                Email = x.u.Email,
                FullName = x.profile.FullName
            })
            .ToListAsync();
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

        // Đổi trạng thái contributor
        contributor.Verified = true;
        contributor.Status = ContributorStatus.ACTIVE;

        // Đổi role user thành Contributor
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

    public async Task<ContributorResponse> ApplyContributor(ContributorCreateRequest request)
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
            Verified = false,
            Status = ContributorStatus.APPLIED
        };

        await contributorRepository.AddAsync(contributor);

        return mapper.Map<ContributorResponse>(contributor);
    }
}