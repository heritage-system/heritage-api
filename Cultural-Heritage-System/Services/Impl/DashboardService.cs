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

        public DashboardService(
            IUserRepository userRepository,
            IStaffRepository staffRepository,
            IContributorRepository contributorRepository,
            IHeritageRepository heritageRepository,
            ICategoryRepository categoryRepository,
            IQuizRepository quizRepository,
            IContributionRepository contributionRepository)
        {
            this.userRepository = userRepository;
            this.staffRepository = staffRepository;
            this.contributorRepository = contributorRepository;
            this.heritageRepository = heritageRepository;
            this.categoryRepository = categoryRepository;
            this.quizRepository = quizRepository;
            this.contributionRepository = contributionRepository;
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
    }
}

