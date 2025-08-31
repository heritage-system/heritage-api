using AutoMapper;
using AutoMapper.QueryableExtensions;
using CloudinaryDotNet.Actions;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Helpers;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq;

namespace Cultural_Heritage_System.Services.Impl
{
    public class TestSearchService : ITestSearchService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly TestSearchRepository heritageRepository;
        private readonly ILogger<TestSearchService> logger;
        private readonly IMapper mapper;

        public TestSearchService(
            TestSearchRepository heritageRepository,
            ILogger<TestSearchService> logger,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            this.heritageRepository = heritageRepository;
            this.logger = logger;
            this.httpContextAccessor = httpContextAccessor;
            this.mapper = mapper;
        }

        public async Task<PageResponse<HeritageSearchResponse>> SearchHeritagesAsync(HeritageSearchRequest request)
        {
            try
            {
                var query = heritageRepository.GetHeritagesQueryable();

                // Keyword search
                if (!string.IsNullOrEmpty(request.Keyword))
                {
                    var searchTerm = request.Keyword.Trim().ToLower();
                    var unsignedTerm = StringHelper.RemoveDiacritics(searchTerm);

                    query = query.Where(h =>                       
                        h.Name.ToLower().Contains(searchTerm) ||
                        h.Description.ToLower().Contains(searchTerm) ||

                     
                        h.NameUnsigned.Contains(unsignedTerm) ||
                        h.DescriptionUnsigned.Contains(unsignedTerm));
                }





                // Location filter
                if (request.Locations != null && request.Locations.Any())
                {                  
                    request.Locations = request.Locations
                                               .Select(loc => StringHelper.RemoveDiacritics(loc))
                                               .ToList();

                    query = query.Where(h => h.HeritageLocations
                                               .Any(hl => request.Locations.Contains(hl.Location.ProvinceUnsigned)));
                }

                // Category filter
                if (request.CategoryIds != null && request.CategoryIds.Any())
                {
                    query = query.Where(h => request.CategoryIds.Contains(h.CategoryId));
                }

                // Tag filter
                if (request.TagIds != null && request.TagIds.Any())
                {
                    query = query.Where(h => h.HeritageTags.Any(t => request.TagIds.Contains(t.TagId)));
                }

                // Location filter 
                if (request.Lat.HasValue && request.Lng.HasValue && request.Radius.HasValue)
                {
                    double lat = request.Lat.Value;
                    double lng = request.Lng.Value;
                    double radius = request.Radius.Value;

                    query = query.Where(h =>
                        h.HeritageLocations.Any(loc =>
                            (6371 * 2 *
                                Math.Asin(
                                    Math.Sqrt(
                                        Math.Pow(Math.Sin(((double)loc.Location.Latitude - lat) * Math.PI / 180 / 2), 2) +
                                        Math.Cos(lat * Math.PI / 180) * Math.Cos((double)loc.Location.Latitude * Math.PI / 180) *
                                        Math.Pow(Math.Sin(((double)loc.Location.Longitude - lng) * Math.PI / 180 / 2), 2)
                                    )
                                )
                            ) <= radius 
                        ));
                  
                }



                int targetYear = DateTime.Now.Year;

              
                query = query.AsEnumerable()
                    .Select(h =>
                    {
                        ConvertOccurrencesToRequestCalendarType(h.HeritageOccurrences, request.CalendarType, targetYear);
                        return h;
                    })
                    .AsQueryable();

                // Date filter
                if ((request.StartDay.HasValue && request.StartMonth.HasValue ||
                    request.EndDay.HasValue && request.EndMonth.HasValue))
                {
                    var filterStart = request.StartDay.HasValue && request.StartMonth.HasValue
                        ? new DateTime(targetYear, request.StartMonth.Value, request.StartDay.Value)
                        : new DateTime(targetYear, 1, 1);

                    var filterEnd = request.EndDay.HasValue && request.EndMonth.HasValue
                        ? new DateTime(targetYear, request.EndMonth.Value, request.EndDay.Value)
                        : new DateTime(targetYear, 12, 30);

                    query = query.AsEnumerable()
                        .Where(h => h.HeritageOccurrences.Any(o =>
                        {
                            var occStart = o.StartDay.HasValue && o.StartMonth.HasValue
                                ? new DateTime(targetYear, o.StartMonth.Value, o.StartDay.Value)
                                : new DateTime(targetYear, 1, 1);

                            var occEnd = o.EndDay.HasValue && o.EndMonth.HasValue
                                ? new DateTime(targetYear, o.EndMonth.Value, o.EndDay.Value)
                                : occStart;

                            return IsInRange(occStart, filterStart, filterEnd);
                        }))
                        .AsQueryable();
                }






                switch (request.SortBy)
                {
                    case SortBy.IDASC:
                        query = query.OrderBy(h => h.Id);
                        break;
                    case SortBy.IDDESC:
                        query = query.OrderByDescending(h => h.Id);
                        break;
                    case SortBy.NAMEASC:
                        query = query.OrderBy(h => h.Name);
                        break;
                    case SortBy.NAMEDESC:
                        query = query.OrderByDescending(h => h.Name);
                        break;                   
                    default:
                        query = query.OrderBy(h => h.Name);
                        break;
                }
                

                var dtoQuery = query.ProjectTo<HeritageSearchResponse>(mapper.ConfigurationProvider);
                // Pagination
                var response = dtoQuery.ToPagedResponse(request.Page, request.PageSize);
                return response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error searching heritages");
                throw;
            }
        }

        private const double EarthRadiusKm = 6371.0;

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);

            lat1 = ToRadians(lat1);
            lat2 = ToRadians(lat2);

            double a = Math.Pow(Math.Sin(dLat / 2), 2) +
                       Math.Cos(lat1) * Math.Cos(lat2) *
                       Math.Pow(Math.Sin(dLon / 2), 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return EarthRadiusKm * c;
        }

        private double ToRadians(double angle) => Math.PI * angle / 180.0;

        private DateTime? ToReferenceDate(HeritageOccurrence occ, int targetYear, CalendarType requestType)
        {
            if (!occ.StartDay.HasValue || !occ.StartMonth.HasValue)
                return null;

            int month = occ.StartMonth.Value;
            int day = occ.StartDay.Value;

            var lunarCal = new ChineseLunisolarCalendar();

           
            if (occ.CalendarType == requestType)
            {
                if (occ.CalendarType == CalendarType.LUNAR)
                {
                    try
                    {
                        return lunarCal.ToDateTime(targetYear, month, day, 0, 0, 0, 0);
                    }
                    catch
                    {
                        return null;
                    }
                }
                else 
                {
                    return new DateTime(targetYear, month, day);
                }
            }

           
            if (occ.CalendarType == CalendarType.LUNAR && requestType == CalendarType.SOLAR)
            {
                
                try
                {
                    return lunarCal.ToDateTime(targetYear, month, day, 0, 0, 0, 0);
                }
                catch
                {
                    return null;
                }
            }
            else if (occ.CalendarType == CalendarType.SOLAR && requestType == CalendarType.LUNAR)
            {
               
                try
                {
                    var solarDate = new DateTime(targetYear, month, day);
                    int lunarYear = lunarCal.GetYear(solarDate);
                    int lunarMonth = lunarCal.GetMonth(solarDate);
                    int lunarDay = lunarCal.GetDayOfMonth(solarDate);

                    
                    return lunarCal.ToDateTime(lunarYear, lunarMonth, lunarDay, 0, 0, 0, 0);
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }



        private DateTime? ToDateTime(int? day, int? month, CalendarType? calendarType, int year = 2025)
        {
            if (!day.HasValue || !month.HasValue) return null;

            if (calendarType == CalendarType.LUNAR)
            {
                // Convert lunar -> solar 
                return LunarToSolar(day.Value, month.Value, year);
            }

            return new DateTime(year, month.Value, day.Value);
        }

        private DateTime LunarToSolar(int day, int month, int year)
        {

            var cal = new System.Globalization.ChineseLunisolarCalendar();
            return cal.ToDateTime(year, month, day, 0, 0, 0, 0);
        }

        private bool IsInRange(DateTime date, DateTime start, DateTime end)
        {
            if (start <= end)
            {
                return date >= start && date <= end;
            }
            else // qua năm mới
            {
                return date >= start || date <= end;
            }
        }

        private void ConvertOccurrencesToRequestCalendarType(IEnumerable<HeritageOccurrence> occurrences, CalendarType targetType, int year)
        {
            if (occurrences == null) return;

            foreach (var occ in occurrences)
            {
                if (occ.CalendarType != targetType)
                {
                    var start = ToReferenceDate(occ, year, targetType);
                    var end = occ.EndDay.HasValue && occ.EndMonth.HasValue
                        ? ToReferenceDate(new HeritageOccurrence
                        {
                            StartDay = occ.EndDay,
                            StartMonth = occ.EndMonth,
                            CalendarType = occ.CalendarType
                        }, year, targetType)
                        : start;

                    if (start.HasValue)
                    {
                        occ.StartDay = start.Value.Day;
                        occ.StartMonth = start.Value.Month;
                        occ.CalendarType = targetType;
                    }

                    if (end.HasValue)
                    {
                        occ.EndDay = end.Value.Day;
                        occ.EndMonth = end.Value.Month;
                        occ.CalendarType = targetType;
                    }
                }
            }
        }

    }
}
