//using Microsoft.EntityFrameworkCore;

//namespace Cultural_Heritage_System.Services.Impl
//{
//    public class TestSearchService : ITestSearchService
//    {
//        private readonly IHttpContextAccessor httpContextAccessor;
//        private readonly TestSearchRepository heritageRepository;
//        private readonly ILogger<TestSearchService> logger;

//        public TestSearchService(
//            TestSearchRepository heritageRepository,
//            ILogger<TestSearchService> logger,
//            IHttpContextAccessor httpContextAccessor)
//        {
//            this.heritageRepository = heritageRepository;
//            this.logger = logger;
//            this.httpContextAccessor = httpContextAccessor;
//        }

//        public async Task<PageResponse<Heritage>> SearchHeritagesAsync(HeritageSearchRequest request)
//        {
//            try
//            {
//                var query = heritageRepository.GetHeritagesQueryable();

//                //Keyword search
//                if (!string.IsNullOrEmpty(request.Keyword))
//                {
//                    var searchTerm = request.Keyword.Trim().ToLower();
//                    query = query.Where(h =>
//                        h.Name.ToLower().Contains(searchTerm) ||
//                        h.Description.ToLower().Contains(searchTerm));
//                }

//                // Category filter
//                if (request.CategoryIds != null && request.CategoryIds.Any())
//                {
//                    query = query.Where(h => request.CategoryIds.Contains(h.CategoryId));
//                }

//                // Tag filter
//                if (request.TagIds != null && request.TagIds.Any())
//                {
//                    query = query.Where(h => h.HeritageTags.Any(t => request.TagIds.Contains(t.TagId)));
//                }

//                // Location filter 
//                if (request.Lat.HasValue && request.Lng.HasValue && request.Radius.HasValue)
//                {
//                    query = query.Where(h =>
//                        h.Coordinates.Any(loc =>
//                            CalculateDistance(
//                                request.Lat.Value, request.Lng.Value,
//                                (double)loc.Latitude, (double)loc.Longitude) <= request.Radius.Value));
//                }


//                // Date filter
//                if (request.StartDate.HasValue && request.EndDate.HasValue)
//                {
//                    query = query.Where(h =>
//                        h.HeritageOccurrences.Any(o =>
//                            o.StartDate.HasValue &&
//                            o.EndDate.HasValue &&
//                            o.StartDate.Value >= request.StartDate.Value &&
//                            o.EndDate.Value <= request.EndDate.Value
//                        ));
//                }
//                else if (request.StartDate.HasValue)
//                {
//                    query = query.Where(h =>
//                        h.HeritageOccurrences.Any(o =>
//                            o.StartDate.HasValue &&
//                            o.StartDate.Value.Date == request.StartDate.Value.Date
//                        ));
//                }

//                switch (request.SortBy)
//                {
//                    case SortBy.NAMEASC:
//                        query = query.OrderBy(h => h.Name);
//                        break;
//                    case SortBy.NAMEDESC:
//                        query = query.OrderByDescending(h => h.Name);
//                        break;
//                    case SortBy.DATEASC:
//                        query = query.OrderBy(h => h.HeritageOccurrences.Min(o => o.StartDate));
//                        break;
//                    case SortBy.DATEDESC:
//                        query = query.OrderByDescending(h => h.HeritageOccurrences.Min(o => o.StartDate));
//                        break;
//                    default:
//                        query = query.OrderBy(h => h.Name);
//                        break;
//                }

//                // Pagination
//                var response = await query.ToPagedResponseAsync(request.Page, request.PageSize);
//                return response;
//            }
//            catch (Exception ex)
//            {
//                logger.LogError(ex, "Error searching heritages");
//                throw;
//            }
//        }

//        private const double EarthRadiusKm = 6371.0;

//        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
//        {
//            double dLat = ToRadians(lat2 - lat1);
//            double dLon = ToRadians(lon2 - lon1);

//            lat1 = ToRadians(lat1);
//            lat2 = ToRadians(lat2);

//            double a = Math.Pow(Math.Sin(dLat / 2), 2) +
//                       Math.Cos(lat1) * Math.Cos(lat2) *
//                       Math.Pow(Math.Sin(dLon / 2), 2);

//            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

//            return EarthRadiusKm * c;
//        }

//        private double ToRadians(double angle) => Math.PI * angle / 180.0;
//    }
//}
