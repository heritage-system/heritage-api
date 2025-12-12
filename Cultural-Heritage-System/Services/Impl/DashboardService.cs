using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Response.Dashboard;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Services.Impl
{
    public class DashboardService : IDashboardService
    {
        private readonly IUserRepository userRepository;
        private readonly IStaffRepository staffRepository;
        private readonly IContributorRepository contributorRepository;
        private readonly IHeritageRepository heritageRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IQuizRepository quizRepository;
        private readonly IContributionRepository contributionRepository;
        private readonly IContributionHeritageTagRepository contributionHeritageTagRepository;
        private readonly IFavoriteRepository favoriteRepository;

        public DashboardService(
            IUserRepository userRepository,
            IStaffRepository staffRepository,
            IContributorRepository contributorRepository,
            IHeritageRepository heritageRepository,
            ICategoryRepository categoryRepository,
            IQuizRepository quizRepository,
            IContributionRepository contributionRepository,
            IContributionHeritageTagRepository contributionHeritageTagRepository,
            IFavoriteRepository favoriteRepository)
        {
            this.userRepository = userRepository;
            this.staffRepository = staffRepository;
            this.contributorRepository = contributorRepository;
            this.heritageRepository = heritageRepository;
            this.categoryRepository = categoryRepository;
            this.quizRepository = quizRepository;
            this.contributionRepository = contributionRepository;
            this.contributionHeritageTagRepository = contributionHeritageTagRepository;
            this.favoriteRepository = favoriteRepository;
        }

        public async Task<ExecutiveDashboardResponse> GetExecutiveDashboardAsync()
        {
            // NOTE: Avoid running concurrent queries on the same DbContext instance
            // to prevent "A second operation was started..." errors.
            var totalUsers = await userRepository.GetTotalUsersCount();
            var activeStaff = await staffRepository
                .GetStaffsQueryable()
                .CountAsync(s => s.StaffStatus == StaffStatus.ACTIVE);
            var activeContributors = await contributorRepository
                .GetContributorsQueryable()
                .CountAsync(c => c.Status == ContributorStatus.ACTIVE);

            var heritageCount = await heritageRepository.GetHeritagesQueryable().CountAsync();
            var categoryCount = await categoryRepository.GetCategoriesQueryable().CountAsync();
            var quizCount = await quizRepository.GetQuizQueryable().CountAsync();

            var contributionsQueryable = contributionRepository.GetContributionsQueryable();
            var totalContributions = await contributionsQueryable.CountAsync();
            var pendingContributions = await contributionsQueryable
                .Where(c => c.Status == ContributionStatus.PENDING)
                .CountAsync();
            var approvedContributions = await contributionsQueryable
                .Where(c => c.Status == ContributionStatus.APPROVED)
                .CountAsync();
            var rejectedContributions = await contributionsQueryable
                .Where(c => c.Status == ContributionStatus.REJECTED)
                .CountAsync();

            return new ExecutiveDashboardResponse
            {
                Users = new DashboardUserStats
                {
                    Total = totalUsers,
                    Employees = activeStaff,
                    Contributors = activeContributors
                },
                Heritage = new DashboardHeritageStats
                {
                    TotalHeritage = heritageCount,
                    Categories = categoryCount,
                    Quizzes = quizCount
                },
                Contributions = new DashboardContributionStats
                {
                    Total = totalContributions,
                    Pending = pendingContributions,
                    Approved = approvedContributions,
                    Rejected = rejectedContributions
                }
            };
        }

        public async Task<List<HeritagePopularItem>> GetTopFavoritedHeritagesAsync(int top)
        {
            var data = await favoriteRepository.GetQueryable()
                .GroupBy(f => new { f.HeritageId, f.Heritage.Name })
                .Select(g => new HeritagePopularItem
                {
                    HeritageId = g.Key.HeritageId,
                    Name = g.Key.Name,
                    Favorites = g.Count()
                })
                .OrderByDescending(x => x.Favorites)
                .Take(top)
                .ToListAsync();

            return data;
        }

        public async Task<List<ContributionPopularItem>> GetTopViewedContributionsAsync(int top)
        {
            var data = await contributionRepository.GetContributionsQueryable()
                .Select(c => new ContributionPopularItem
                {
                    ContributionId = c.Id,
                    Title = c.Title,
                    Views = c.ContributionAccessLogs.Count
                })
                .OrderByDescending(x => x.Views)
                .ThenByDescending(x => x.ContributionId)
                .Take(top)
                .ToListAsync();

            return data;
        }

        public async Task<List<HeritageProvinceBucket>> GetHeritageByProvinceAsync()
        {
            var data = await heritageRepository.GetHeritagesQueryable()
                .SelectMany(h => h.HeritageLocations.Select(hl => new
                {
                    Province = hl.Location.Province ?? "Không rõ"
                }))
                .GroupBy(x => x.Province)
                .Select(g => new HeritageProvinceBucket
                {
                    Province = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .ToListAsync();

            return data;
        }

        public async Task<List<ContributionTrendPoint>> GetContributionTrendAsync(int months)
        {
            var today = DateTime.UtcNow;
            var start = new DateTime(today.Year, today.Month, 1).AddMonths(-Math.Max(months - 1, 0));

            var data = await contributionRepository.GetContributionsQueryable()
                .Where(c => c.CreatedAt >= start)
                .GroupBy(c => new { c.CreatedAt.Year, c.CreatedAt.Month })
                .Select(g => new ContributionTrendPoint
                {
                    Period = $"{g.Key.Year}-{g.Key.Month:D2}",
                    Total = g.Count(),
                    Approved = g.Count(x => x.Status == ContributionStatus.APPROVED),
                    Pending = g.Count(x => x.Status == ContributionStatus.PENDING),
                    Rejected = g.Count(x => x.Status == ContributionStatus.REJECTED)
                })
                .OrderBy(x => x.Period)
                .ToListAsync();

            return data;
        }

        public async Task<List<EngagementByCategoryItem>> GetEngagementByCategoryAsync(int topCategories)
        {
            var data = await contributionRepository.GetContributionsQueryable()
                .SelectMany(c => c.ContributionHeritageTags.Select(ht => new
                {
                    CategoryName = ht.Heritage.Category.Name,
                    Views = c.ContributionAccessLogs.Count,
                    Saves = c.ContributionSaves.Count,
                    Comments = c.Reviews.Count
                }))
                .GroupBy(x => x.CategoryName)
                .Select(g => new EngagementByCategoryItem
                {
                    Category = g.Key,
                    Views = g.Sum(x => x.Views),
                    Saves = g.Sum(x => x.Saves),
                    Comments = g.Sum(x => x.Comments)
                })
                .OrderByDescending(x => x.Views)
                .Take(topCategories)
                .ToListAsync();

            return data;
        }

        public async Task<List<YearlyGrowthItem>> GetYearlyGrowthAsync(int years)
        {
            var currentYear = DateTime.UtcNow.Year;
            var startYear = currentYear - Math.Max(years - 1, 0);

            var heritageGrowth = await heritageRepository.GetHeritagesQueryable()
                .Where(h => h.CreatedAt.Year >= startYear)
                .GroupBy(h => h.CreatedAt.Year)
                .Select(g => new { g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Key, x => x.Count);

            var userGrowth = await userRepository.GetQueryable()
                .Where(u => u.CreatedAt.Year >= startYear)
                .GroupBy(u => u.CreatedAt.Year)
                .Select(g => new { g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Key, x => x.Count);

            var contributionGrowth = await contributionRepository.GetContributionsQueryable()
                .Where(c => c.CreatedAt.Year >= startYear)
                .GroupBy(c => c.CreatedAt.Year)
                .Select(g => new { g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Key, x => x.Count);

            var result = new List<YearlyGrowthItem>();
            for (int year = startYear; year <= currentYear; year++)
            {
                result.Add(new YearlyGrowthItem
                {
                    Year = year,
                    Heritage = heritageGrowth.TryGetValue(year, out var h) ? h : 0,
                    Users = userGrowth.TryGetValue(year, out var u) ? u : 0,
                    Contributions = contributionGrowth.TryGetValue(year, out var c) ? c : 0
                });
            }

            return result;
        }
    }
}

