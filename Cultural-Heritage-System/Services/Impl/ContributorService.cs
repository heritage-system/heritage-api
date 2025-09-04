using AutoMapper;
using AutoMapper.QueryableExtensions;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Request.Contributor;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Contributor;
using Cultural_Heritage_System.Helpers;
using Cultural_Heritage_System.Middlewares;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Services.Impl
{
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
                            DocumentsUrl = c.DocumentsUrl,
                            Rating = c.Rating,
                            Verified = c.Verified,
                            Status = c.Status,
                            UserId = c.UserId,
                            UserEmail = c.User.Email,
                            UserFullName = profile.FullName,
                            CreatedAt = c.CreatedAt,
                            UpdatedAt = c.UpdatedAt,
                            CreatedBy = c.CreatedBy,
                            UpdatedBy = c.UpdatedBy,
                            Count = c.Contributions.Count()
                        };

            // ---- Filter ----
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                var searchTerm = request.Keyword.Trim().ToLower();
                query = query.Where(x =>
                    (x.Expertise != null && x.Expertise.ToLower().Contains(searchTerm)) ||
                    (x.UserFullName != null && x.UserFullName.ToLower().Contains(searchTerm)));
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
                if (!string.IsNullOrEmpty(item.CreatedBy))
                {
                    if (item.CreatedBy == "system")
                    {
                        item.CreatedByName = "System";
                        item.CreatedByEmail = "system";
                    }
                    else
                    {
                        var user = await userRepository.GetByIdAsync(int.Parse(item.CreatedBy));
                        if (user != null)
                        {
                            item.CreatedByEmail = user.Email;

                            // Lấy fullName từ ProfileRepository
                            var profile = await profileRepository.GetProfileByUserIdAsync(user.Id);
                            item.CreatedByName = profile?.FullName;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(item.UpdatedBy))
                {
                    if (item.UpdatedBy == "system")
                    {
                        item.UpdatedByName = "System";
                        item.UpdatedByEmail = "system";
                    }
                    else
                    {
                        var user = await userRepository.GetByIdAsync(int.Parse(item.UpdatedBy));
                        if (user != null)
                        {
                            item.UpdatedByEmail = user.Email;

                            // Lấy fullName từ ProfileRepository
                            var profile = await profileRepository.GetProfileByUserIdAsync(user.Id);
                            item.UpdatedByName = profile?.FullName;
                        }
                    }
                }
            }

            return paged;
        }

        public async Task<ContributorResponse?> GetContributorDetail(int id)
        {
            var contributor = await contributorRepository.GetContributorById(id);
            if (contributor == null) return null;

            var response = mapper.Map<ContributorResponse>(contributor);

            // Load Profile thủ công
            var profile = await profileRepository.GetProfileByUserIdAsync(contributor.UserId);
            if (profile != null)
            {
                response.UserFullName = profile.FullName;
            }

            return response;
        }


        public async Task<ContributorResponse> CreateContributor(ContributorCreateRequest request)
        {
            // 1. Lấy user đang login
            var accountIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(accountIdClaim))
            {
                throw new AppException(ErrorCode.UNAUTHORIZED);
            }

            var currentUser = await userRepository.FindUserById(int.Parse(accountIdClaim));
            if (currentUser == null)
            {
                throw new AppException(ErrorCode.UNAUTHORIZED);
            }

            User targetUser;

            // 2. Nếu request có email => Admin cấp Contributor cho user khác
            if (!string.IsNullOrEmpty(request.UserEmail))
            {
                if (currentUser.Role?.Name != DefinitionRole.ADMIN)
                {
                    throw new AppException(ErrorCode.FORBIDDEN);
                }

                targetUser = await userRepository.FindUserByEmail(request.UserEmail);
                if (targetUser == null)
                {
                    throw new AppException(ErrorCode.USER_NOT_EXISTED);
                }
            }
            else
            {
                // 3. Nếu không có email => user tự apply Contributor
                targetUser = currentUser;
            }

            // 4. Nếu user chưa có role Contributor thì gán
            if (targetUser.Role?.Name != DefinitionRole.CONTRIBUTOR)
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

            // 5. Check user đã có Contributor record chưa
            var existedContributor = await contributorRepository
                .GetContributorsQueryable()
                .FirstOrDefaultAsync(c => c.UserId == targetUser.Id);

            if (existedContributor != null)
            {
                throw new AppException(ErrorCode.CONTRIBUTOR_EXISTED);
            }

            // 6. Tạo contributor mới
            var contributor = mapper.Map<Contributor>(request);
            contributor.UserId = targetUser.Id;

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

            mapper.Map(request, contributor);
            await contributorRepository.UpdateAsync(contributor);
            return mapper.Map<ContributorResponse>(contributor);
        }

        public async Task DeleteContributor(int id)
        {
            var contributor = await contributorRepository.GetContributorById(id);
            if (contributor == null)
            {
                throw new AppException(ErrorCode.CONTRIBUTOR_NOT_EXISTED);
            }

            await contributorRepository.DeleteAsync(contributor);
        }
    }
}
