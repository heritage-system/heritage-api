using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Models;

namespace Cultural_Heritage_System.Services
{
    public interface IContributionService
    {
        Task<ContributionCreationResponse> PostContribution(ContributionCreationRequest request);

    }
}

