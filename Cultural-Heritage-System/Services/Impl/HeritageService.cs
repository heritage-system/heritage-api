using AutoMapper;
using AutoMapper.QueryableExtensions;
using Azure.Core;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Models;
using Cultural_Heritage_System.Dtos.Request.Heritage;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Contribution;
using Cultural_Heritage_System.Dtos.Response.Heritage;
using Cultural_Heritage_System.Helpers;
using Cultural_Heritage_System.Middlewares;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static StackExchange.Redis.Role;

namespace Cultural_Heritage_System.Services.Impl
{
    public class HeritageService : IHeritageService
    {
        private readonly IHeritageRepository _heritageRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly ILogger<HeritageService> _logger;
        private readonly IUserRepository _userRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IHeritageLocationRepository _heritageLocationRepository;
        private readonly IHeritageOccurrenceRepository _heritageOccurrenceRepository;
        private readonly IHeritageMediaRepository _heritageMediaRepository;
        private readonly IHeritageTagRepository _heritageTagRepository;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IFavoriteRepository _favoriteRepository;
        public HeritageService(IHeritageRepository heritageRepository, IHttpContextAccessor httpContextAccessor , 
            IMapper mapper, ILogger<HeritageService> logger, ILocationRepository locationRepository,
            IHeritageLocationRepository heritageLocationRepository, IHeritageOccurrenceRepository heritageOccurrenceRepository,
            IHeritageMediaRepository heritageMediaRepository, IHeritageTagRepository heritageTagRepository, 
            ICloudinaryService cloudinaryService,IUserRepository userRepository, IFavoriteRepository favoriteRepository)
        {
            _heritageRepository = heritageRepository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _logger = logger;        
            _locationRepository = locationRepository;
            _heritageLocationRepository = heritageLocationRepository;
            _heritageOccurrenceRepository = heritageOccurrenceRepository;
            _heritageMediaRepository = heritageMediaRepository;
            _heritageTagRepository = heritageTagRepository;
            _cloudinaryService = cloudinaryService;
            _userRepository = userRepository;
            _favoriteRepository = favoriteRepository;
        }

        public async Task<PageResponse<HeritageResponse>> GetAllAsync(HeritageOverviewSearchRequest request)
        {
            var query = _heritageRepository.GetAllQuery();

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                var lowerKeyword = request.Keyword.Trim().ToLower();
                var unsignedTerm = StringHelper.RemoveDiacritics(lowerKeyword);
                query = query.Where(h => h.NameUnsigned.ToLower().Contains(unsignedTerm)
                                       || h.Name.ToLower().Contains(lowerKeyword));
            }

            if (request.CategoryId.HasValue)
                query = query.Where(h => h.CategoryId == request.CategoryId.Value);

            if (request.TagIds != null && request.TagIds.Any())
            {
                query = query.Where(h => request.TagIds.All(tagId =>
                    h.HeritageTags.Any(ht => ht.TagId == tagId)));
            }

            if (request.SortBy.HasValue)
            {
                query = request.SortBy.Value switch
                {
                    SortBy.NAMEASC => query.OrderBy(h => h.Name),
                    SortBy.NAMEDESC => query.OrderByDescending(h => h.Name),
                    SortBy.IDASC => query.OrderBy(h => h.Id),
                    SortBy.IDDESC => query.OrderByDescending(h => h.Id),
                    SortBy.DATEASC => query.OrderBy(h => h.CreatedAt),
                    SortBy.DATEDESC => query.OrderByDescending(h => h.CreatedAt),
                    _ => query.OrderByDescending(h => h.CreatedAt)
                };
            }
            else
            {
                query = query.OrderByDescending(h => h.CreatedAt);
            }

            var pagedResult = await query.ToPagedResponseAsync(request.Page, request.PageSize);
            return _mapper.Map<PageResponse<HeritageResponse>>(pagedResult);
        }

        public async Task<HeritageResponse> GetByIdAsync(long id)
        {
            var heritage = await _heritageRepository.GetByIdAsync(id);
            if (heritage == null)
                return null;

            return _mapper.Map<HeritageResponse>(heritage);
        }
        
        public async Task<HeritageResponse> CreateAsync(HeritageCreateRequest request)
        {
            //using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var accountIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(accountIdClaim))
                    throw new AppException(ErrorCode.UNAUTHORIZED);

                var staff = await _userRepository.FindUserById(int.Parse(accountIdClaim));
                // 1. Tạo Heritage
                var heritage = _mapper.Map<Heritage>(request);
                heritage.CreatedBy = accountIdClaim;


                // 3. Xử lý Tags
                if (request.TagIds?.Any() == true)
                {
                    heritage.HeritageTags = request.TagIds.Select(tagId => new HeritageTag
                    {
                        TagId = tagId
                    }).ToList();
                }

                // --- Locations ---
                if(request.Locations != null)
                {
                    foreach (var locReq in request.Locations)
                    {
                        var location = _mapper.Map<Location>(locReq);
                        location.GenerateUnsignedFields();

                        heritage.HeritageLocations.Add(new HeritageLocation
                        {
                            Heritage = heritage,
                            Location = location
                        });
                    }
                }
                

                // --- Occurrences ---
                foreach (var occReq in request.Occurrences)
                {
                    var occurrence = _mapper.Map<HeritageOccurrence>(occReq);
                    occurrence.Heritage = heritage;
                    heritage.HeritageOccurrences.Add(occurrence);
                }

                // Add heritage vào context
                await _heritageRepository.AddAsync(heritage);          

                // Map response
                var response = _mapper.Map<HeritageResponse>(heritage);
                return response;
            }
            catch
            {              
                throw;
            }
        }
        public async Task<HeritageResponse> UpdateAsync(HeritageUpdateRequest request)
        {           
            try
            {
                var accountIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(accountIdClaim))
                    throw new AppException(ErrorCode.UNAUTHORIZED);

                var heritage = await _heritageRepository.GetByIdAsync(request.Id);
                if (heritage == null)
                    throw new Exception("Heritage not found");

                // --- Update basic fields ---
                heritage.Name = request.Name;
                heritage.Description = request.Description;
                heritage.CategoryId = request.CategoryId;
                if (request.Content != null)
                {
                    heritage.Content = JsonSerializer.Serialize(request.Content);
                }

                // --- Update Media ---
                heritage.Media.Clear();
                foreach (var mediaReq in request.Media)
                {
                    var media = _mapper.Map<HeritageMedia>(mediaReq);
                    media.HeritageId = heritage.Id;
                    heritage.Media.Add(media);
                }

                // --- Update Tags ---
                heritage.HeritageTags.Clear();
                foreach (var tagId in request.TagIds)
                {
                    heritage.HeritageTags.Add(new HeritageTag
                    {
                        HeritageId = heritage.Id,
                        TagId = tagId
                    });
                }

                // --- Update Locations ---
                heritage.HeritageLocations.Clear();
                foreach (var locReq in request.Locations)
                {
                    var location = _mapper.Map<Location>(locReq);                

                    heritage.HeritageLocations.Add(new HeritageLocation
                    {
                        HeritageId = heritage.Id,
                        Location = location
                    });
                }

                // --- Update Occurrences ---
                heritage.HeritageOccurrences.Clear();
                foreach (var occReq in request.Occurrences)
                {
                    var occurrence = _mapper.Map<HeritageOccurrence>(occReq);
                    occurrence.HeritageId = heritage.Id;
                    heritage.HeritageOccurrences.Add(occurrence);
                }

                await _heritageRepository.UpdateAsync(heritage);
                //await _dbContext.SaveChangesAsync();            

                return _mapper.Map<HeritageResponse>(heritage);
            }
            catch
            {               
                throw;
            }
        }

        public async Task<long?> DeleteAsync(long id)
        {
            var heritage = await _heritageRepository.GetByIdAsync(id);
            if (heritage == null)
                return null;

            await _heritageRepository.DeleteAsync(heritage);
            return id;
        }

        public async Task<List<HeritageNameSearchResponse>> SearchListHeritageName(string? keyword)
        {
            var query = _heritageRepository.GetAllQuery();

            if (!string.IsNullOrEmpty(keyword))
            {
                var lowerKeyword = keyword.Trim().ToLower();
                var unsignedTerm = StringHelper.RemoveDiacritics(lowerKeyword);

                query = query.Where(h => h.NameUnsigned.ToLower().Contains(unsignedTerm)
                                       || h.Name.ToLower().Contains(lowerKeyword));
            }

            var list = await query.ToListAsync();

            return _mapper.Map<List<HeritageNameSearchResponse>>(list);
        }

        public async Task<List<HeritageRelatedResponse>> GetHeritageRelated(HeritageRelatedRequest request)
        {
            try
            {
                var initQuery = _heritageRepository.GetHeritagesQueryable();

                // Loại bỏ heritage hiện tại
                var baseQuery = initQuery.Where(c => c.Id != request.HeritageId);

                List<HeritageRelatedResponse> related = new();

                // 1. Ưu tiên Category
                if (request.CategoryIds > 0)
                {
                    var categoryQuery = baseQuery
                        .Where(c => c.CategoryId == request.CategoryIds)
                        .ProjectTo<HeritageRelatedResponse>(_mapper.ConfigurationProvider)
                        .OrderBy(x => Guid.NewGuid());
               
                    related = await categoryQuery.Take(request.Quantity).ToListAsync();

                    if (related.Count >= request.Quantity)
                        return related;
                }

                // 2. Nếu chưa đủ thì check TagIds
                if ((request.TagIds != null && request.TagIds.Any()) && related.Count < request.Quantity)
                {
                    var tagQuery = baseQuery
                        .Where(c => c.ContributionHeritageTags.Any(tag => request.TagIds.Contains((int)tag.HeritageId)))
                        .ProjectTo<HeritageRelatedResponse>(_mapper.ConfigurationProvider)
                        .OrderBy(x => Guid.NewGuid());
                
                    var tagRelated = await tagQuery.Take(request.Quantity - related.Count).ToListAsync();
                    related.AddRange(tagRelated);

                    if (related.Count >= request.Quantity)
                        return related;
                }

                // 3. Nếu chưa đủ thì check keyword
                if (!string.IsNullOrWhiteSpace(request.Keyword) && related.Count < request.Quantity)
                {
                    var keyword = request.Keyword.Trim().ToLower();
                    var unsignedKeyword = StringHelper.RemoveDiacritics(keyword);

                    var keywordQuery = baseQuery
                        .Where(c =>
                            c.Name.ToLower().Contains(keyword) ||
                            c.NameUnsigned.Contains(unsignedKeyword))
                        .ProjectTo<HeritageRelatedResponse>(_mapper.ConfigurationProvider)
                        .OrderBy(x => Guid.NewGuid());

                    var keywordRelated = await keywordQuery.Take(request.Quantity - related.Count).ToListAsync();
                    related.AddRange(keywordRelated);

                    if (related.Count >= request.Quantity)
                        return related;
                }

                // 4. Nếu vẫn chưa đủ thì fallback random
                if (related.Count < request.Quantity)
                {
                    var excludeIds = related.Select(r => r.Id).ToList();                   
                    excludeIds.Add((int)request.HeritageId);

                    var fallbackQuery = baseQuery
                        .Where(c => !excludeIds.Contains((int)c.Id))
                        .ProjectTo<HeritageRelatedResponse>(_mapper.ConfigurationProvider)
                        .OrderBy(x => Guid.NewGuid());

                    var fallback = await fallbackQuery.Take(request.Quantity - related.Count).ToListAsync();
                    related.AddRange(fallback);
                }

                return related;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting related contributions");
                throw;
            }
        }


        public async Task<PageResponse<HeritageSearchResponse>> SearchHeritagesAsync1(HeritageSearchRequest request)
        {
            try
            {
                var query = _heritageRepository.GetHeritagesQueryable();

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

                // ---- XỬ LÝ DATE FILTER ----
                int targetYear = DateTime.Now.Year;
                bool hasDateFilter = (request.StartDay.HasValue && request.StartMonth.HasValue) ||
                                     (request.EndDay.HasValue && request.EndMonth.HasValue);

                if (hasDateFilter)
                {
                    // Convert Occurrences & filter trước khi phân trang
                    query = query.AsEnumerable()
                        .Select(h =>
                        {
                            ConvertOccurrencesToRequestCalendarType(h.HeritageOccurrences, request.CalendarType, targetYear);
                            return h;
                        })
                        .Where(h =>
                        {
                            var filterStart = request.StartDay.HasValue && request.StartMonth.HasValue
                                ? new DateTime(targetYear, request.StartMonth.Value, request.StartDay.Value)
                                : new DateTime(targetYear, 1, 1);

                            var filterEnd = request.EndDay.HasValue && request.EndMonth.HasValue
                                ? new DateTime(targetYear, request.EndMonth.Value, request.EndDay.Value)
                                : new DateTime(targetYear, 12, 31);

                            return h.HeritageOccurrences.Any(o =>
                            {
                                var occStart = o.StartDay.HasValue && o.StartMonth.HasValue
                                    ? new DateTime(targetYear, o.StartMonth.Value, o.StartDay.Value)
                                    : new DateTime(targetYear, 1, 1);

                                var occEnd = o.EndDay.HasValue && o.EndMonth.HasValue
                                    ? new DateTime(targetYear, o.EndMonth.Value, o.EndDay.Value)
                                    : occStart;

                                return IsInRange(occStart, filterStart, filterEnd);
                            });
                        })
                        .AsQueryable();
                }

                // ---- SORT ----
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

                // ---- MAP + PHÂN TRANG ----
                var dtoQuery = query.ProjectTo<HeritageSearchResponse>(_mapper.ConfigurationProvider);
                var response = dtoQuery.ToPagedResponse(request.Page, request.PageSize);


                if (!hasDateFilter)
                {
                    foreach (var item in response.Items)
                    {
                        ConvertOccurrencesDtoToRequestCalendarType(item.HeritageOccurrences, request.CalendarType, targetYear);
                    }
                }

                // ---- Gán IsSave cho từng heritage ----
                var accountIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
                if (!string.IsNullOrEmpty(accountIdClaim) && response.Items != null)
                {
                    var userId = int.Parse(accountIdClaim);

                    foreach (var item in response.Items)
                    {
                        item.IsSave = await _favoriteRepository.IsFavoriteExistsAsync(userId, item.Id);
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching heritages");
                throw;
            }
        }

        public async Task<PageResponse<HeritageSearchResponse>> SearchHeritagesAsync(HeritageSearchRequest request)
        {
            try
            {
                var baseQuery = _heritageRepository.GetHeritagesQueryable();

                // ===== 1) SQL-first filters (dịch được sang SQL) =====
                if (!string.IsNullOrWhiteSpace(request.Keyword))
                {
                    var searchTerm = request.Keyword.Trim().ToLower();
                    var unsignedTerm = StringHelper.RemoveDiacritics(searchTerm).ToLower();

                    baseQuery = baseQuery.Where(h =>
                        h.Name.ToLower().Contains(searchTerm) ||
                        h.Description.ToLower().Contains(searchTerm) ||
                        h.NameUnsigned.Contains(unsignedTerm) ||
                        h.DescriptionUnsigned.Contains(unsignedTerm));
                }

                if (request.Locations != null && request.Locations.Any())
                {
                    var unsignedLocations = request.Locations
                        .Select(loc => StringHelper.RemoveDiacritics(loc))
                        .Select(loc =>
                        {
                            loc = loc.ToLower().Trim();                         
                            loc = loc
                                .Replace("thanh pho ", "")  
                                .Replace("tp. ", "")       
                                .Replace("tp ", "")      
                                .Replace("tinh ", "");  

                            return loc.Trim();
                        })
                        .ToList();

                    baseQuery = baseQuery.Where(h =>
                        h.HeritageLocations.Any(hl =>
                            unsignedLocations.Contains(hl.Location.ProvinceUnsigned.ToLower())));
                }


                if (request.CategoryIds != null && request.CategoryIds.Any())
                {
                    baseQuery = baseQuery.Where(h => request.CategoryIds.Contains(h.CategoryId));
                }

                if (request.TagIds != null && request.TagIds.Any())
                {
                    baseQuery = baseQuery.Where(h => h.HeritageTags.Any(t => request.TagIds.Contains(t.TagId)));
                }

                // ===== 2) Date filter logic =====
                int targetYear = DateTime.Now.Year;
                bool hasDateFilter =
                    (request.StartDay.HasValue && request.StartMonth.HasValue) ||
                    (request.EndDay.HasValue && request.EndMonth.HasValue);

                // Tập id sau khi áp các filter SQL ở trên
                var candidateIds = await baseQuery.Select(h => h.Id).ToListAsync();

                HashSet<long> eligibleIds;

                if (hasDateFilter)
                {
                    // 2A. Tính khoảng lọc ngày
                    var filterStart = request.StartDay.HasValue && request.StartMonth.HasValue
                        ? new DateTime(targetYear, request.StartMonth.Value, request.StartDay.Value)
                        : new DateTime(targetYear, 1, 1);

                    var filterEnd = request.EndDay.HasValue && request.EndMonth.HasValue
                        ? new DateTime(targetYear, request.EndMonth.Value, request.EndDay.Value)
                        : new DateTime(targetYear, 12, 31);

                    // 2B. Duyệt theo lô để không kéo quá nhiều dữ liệu vào RAM
                    const int CHUNK = 1000;
                    eligibleIds = new HashSet<long>();

                    for (int i = 0; i < candidateIds.Count; i += CHUNK)
                    {
                        var chunk = candidateIds.Skip(i).Take(CHUNK).ToList();

                        // Chỉ nạp Occurrences cần thiết cho lô id
                        var occByHeritage = await _heritageOccurrenceRepository.QueryOccurrences()
                            .AsNoTracking()
                            .Where(o => chunk.Contains(o.HeritageId))
                            .Select(o => new
                            {
                                o.HeritageId,
                                o.StartDay,
                                o.StartMonth,
                                o.EndDay,
                                o.EndMonth,
                                o.CalendarType
                            })
                            .ToListAsync();

                        // Nhóm theo HeritageId
                        var group = occByHeritage.GroupBy(o => o.HeritageId);

                        foreach (var g in group)
                        {
                            // convert âm/dương theo request trước khi so khớp
                            bool match = false;
                            foreach (var o in g)
                            {
                                var occModel = new HeritageOccurrence
                                {
                                    StartDay = o.StartDay,
                                    StartMonth = o.StartMonth,
                                    EndDay = o.EndDay,
                                    EndMonth = o.EndMonth,
                                    CalendarType = o.CalendarType
                                };

                                // chuyển về hệ lịch người dùng yêu cầu
                                ConvertOccurrencesToRequestCalendarType(
                                    new[] { occModel }, request.CalendarType, targetYear);

                                if (occModel.StartDay.HasValue && occModel.StartMonth.HasValue)
                                {
                                    var occStart = new DateTime(targetYear, occModel.StartMonth.Value, occModel.StartDay.Value);
                                    var occEnd = (occModel.EndDay.HasValue && occModel.EndMonth.HasValue)
                                        ? new DateTime(targetYear, occModel.EndMonth.Value, occModel.EndDay.Value)
                                        : occStart;

                                    if (IsInRange(occStart, filterStart, filterEnd))
                                    {
                                        match = true;
                                        break;
                                    }
                                }
                            }

                            if (match) eligibleIds.Add(g.Key);
                        }
                    }
                }
                else
                {
                    // Không có date filter: tất cả id đang là ứng viên hợp lệ
                    eligibleIds = new HashSet<long>(candidateIds);
                }

                // Nếu rỗng → trả trang rỗng sớm
                if (eligibleIds.Count == 0)
                {
                    return new PageResponse<HeritageSearchResponse>
                    {
                        Items = new List<HeritageSearchResponse>(),
                        TotalElements = 0,
                        CurrentPages = request.Page,
                        PageSizes = request.PageSize
                    };
                }

                // ===== 3) Áp eligibleIds -> Sort + Paginate trong SQL =====
                var finalQuery = _heritageRepository.GetHeritagesQueryable().AsNoTracking()
                    .Where(h => eligibleIds.Contains(h.Id));

                switch (request.SortBy)
                {
                    case SortBy.IDASC: finalQuery = finalQuery.OrderBy(h => h.Id); break;
                    case SortBy.IDDESC: finalQuery = finalQuery.OrderByDescending(h => h.Id); break;
                    case SortBy.NAMEASC: finalQuery = finalQuery.OrderBy(h => h.Name); break;
                    case SortBy.NAMEDESC: finalQuery = finalQuery.OrderByDescending(h => h.Name); break;
                    default: finalQuery = finalQuery.OrderBy(h => h.Name); break;
                }

                var dtoQuery = finalQuery.ProjectTo<HeritageSearchResponse>(_mapper.ConfigurationProvider);
                var response = dtoQuery.ToPagedResponse(request.Page, request.PageSize);


                foreach (var item in response.Items)
                {
                    ConvertOccurrencesDtoToRequestCalendarType(item.HeritageOccurrences, request.CalendarType, targetYear);
                }
           
                // ===== 4) IsSave cho từng heritage =====
                var accountIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
                if (!string.IsNullOrEmpty(accountIdClaim))
                {
                    var userId = int.Parse(accountIdClaim);
                    foreach (var item in response.Items)
                    {
                        item.IsSave = await _favoriteRepository.IsFavoriteExistsAsync(userId, item.Id);
                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching heritages");
                throw;
            }
        }

        public async Task<HeritageDetailResponse> GetHeritageDetail(long id)
        {

            var existingHeritage = await _heritageRepository.GetHeritageById(id);

            if (existingHeritage == null)
            {
                throw new AppException(ErrorCode.HERITAGE_NOT_FOUND);
            }

            var response = _mapper.Map<HeritageDetailResponse>(existingHeritage);

            var accountIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
            if (accountIdClaim != null)
            {
                response.IsSave = await _favoriteRepository.IsFavoriteExistsAsync(int.Parse(accountIdClaim), id);
            }

            return response;
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

            try
            {
                // Khi cùng loại lịch
                if (occ.CalendarType == requestType)
                {
                    if (occ.CalendarType == CalendarType.LUNAR)
                    {
                        // ⚙️ Xử lý tháng nhuận trước khi convert
                        int leapMonth = lunarCal.GetLeapMonth(targetYear);
                        if (leapMonth > 0 && month >= leapMonth / 2)
                            month++;

                        return lunarCal.ToDateTime(targetYear, month, day, 0, 0, 0, 0);
                    }
                    else
                    {
                        return new DateTime(targetYear, month, day);
                    }
                }

                // 🌙 ÂM → DƯƠNG
                if (occ.CalendarType == CalendarType.LUNAR && requestType == CalendarType.SOLAR)
                {
                    int leapMonth = lunarCal.GetLeapMonth(targetYear);
                    if (leapMonth > 0 && month >= leapMonth / 2)
                        month++;

                    return lunarCal.ToDateTime(targetYear, month, day, 0, 0, 0, 0);
                }

                // ☀️ DƯƠNG → ÂM
                if (occ.CalendarType == CalendarType.SOLAR && requestType == CalendarType.LUNAR)
                {
                    var solarDate = new DateTime(targetYear, month, day);

                    int lunarYear = lunarCal.GetYear(solarDate);
                    int lunarMonth = lunarCal.GetMonth(solarDate);
                    int lunarDay = lunarCal.GetDayOfMonth(solarDate);

                    // ⚙️ Xử lý lại tháng nhuận ngược
                    int leapMonth = lunarCal.GetLeapMonth(lunarYear);
                    if (leapMonth > 0 && lunarMonth > leapMonth / 2)
                        lunarMonth--;

                    return lunarCal.ToDateTime(lunarYear, lunarMonth, lunarDay, 0, 0, 0, 0);
                }
            }
            catch
            {
                return null;
            }

            return null;
        }


        private DateTime? ToReferenceDateDto(HeritageOccurrenceDto occ, int targetYear, CalendarType requestType)
        {
            if (!occ.StartDay.HasValue || !occ.StartMonth.HasValue)
                return null;

            int month = occ.StartMonth.Value;
            int day = occ.StartDay.Value;
            var lunarCal = new ChineseLunisolarCalendar();

            try
            {
                // Khi cùng loại lịch
                if (occ.CalendarTypeName == requestType.ToString())
                {
                    if (occ.CalendarTypeName == CalendarType.LUNAR.ToString())
                    {
                        // ⚙️ Xử lý tháng nhuận trước khi convert
                        int leapMonth = lunarCal.GetLeapMonth(targetYear);
                        if (leapMonth > 0 && month >= leapMonth / 2)
                            month++;

                        return lunarCal.ToDateTime(targetYear, month, day, 0, 0, 0, 0);
                    }
                    else
                    {
                        return new DateTime(targetYear, month, day);
                    }
                }

                // 🌙 ÂM → DƯƠNG
                if (occ.CalendarTypeName == CalendarType.LUNAR.ToString() && requestType == CalendarType.SOLAR)
                {
                    int leapMonth = lunarCal.GetLeapMonth(targetYear);
                    if (leapMonth > 0 && month >= leapMonth / 2)
                        month++;

                    return lunarCal.ToDateTime(targetYear, month, day, 0, 0, 0, 0);
                }

                // ☀️ DƯƠNG → ÂM
                if (occ.CalendarTypeName == CalendarType.SOLAR.ToString() && requestType == CalendarType.LUNAR)
                {
                    var solarDate = new DateTime(targetYear, month, day);

                    int lunarYear = lunarCal.GetYear(solarDate);
                    int lunarMonth = lunarCal.GetMonth(solarDate);
                    int lunarDay = lunarCal.GetDayOfMonth(solarDate);

                    // ⚙️ Xử lý lại tháng nhuận ngược
                    int leapMonth = lunarCal.GetLeapMonth(lunarYear);
                    if (leapMonth > 0 && lunarMonth > leapMonth / 2)
                        lunarMonth--;

                    return lunarCal.ToDateTime(lunarYear, lunarMonth, lunarDay, 0, 0, 0, 0);
                }
            }
            catch
            {
                return null;
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

        private void ConvertOccurrencesDtoToRequestCalendarType(IEnumerable<HeritageOccurrenceDto> occurrences, CalendarType targetType, int year)
        {
            if (occurrences == null) return;

            foreach (var occ in occurrences)
            {
                if (occ.CalendarTypeName != targetType.ToString())
                {
                    var start = ToReferenceDateDto(occ, year, targetType);
                    var end = occ.EndDay.HasValue && occ.EndMonth.HasValue
                        ? ToReferenceDateDto(new HeritageOccurrenceDto
                        {
                            StartDay = occ.EndDay,
                            StartMonth = occ.EndMonth,
                            CalendarTypeName = occ.CalendarTypeName
                        }, year, targetType)
                        : start;

                    if (start.HasValue)
                    {
                        occ.StartDay = start.Value.Day;
                        occ.StartMonth = start.Value.Month;
                        occ.CalendarTypeName = targetType.ToString();
                    }

                    if (end.HasValue)
                    {
                        occ.EndDay = end.Value.Day;
                        occ.EndMonth = end.Value.Month;
                        occ.CalendarTypeName = targetType.ToString();
                    }
                }
            }
        }

        public async Task<byte[]> ExportHeritagesToCsvAsync(HeritageOverviewSearchRequest request)
        {
            try
            {
                if(request.Page == 0)
                {
                    // Get all data without pagination for export
                    request.Page = 1;
                    request.PageSize = int.MaxValue;                 
                }
 
                var heritages = await GetAllAsync(request);

                // Convert to CSV
                var csv = GenerateHeritageCsv(heritages.Items);

                // Add BOM for Excel UTF-8 support
                var preamble = System.Text.Encoding.UTF8.GetPreamble();
                var bytes = System.Text.Encoding.UTF8.GetBytes(csv);

                var result = new byte[preamble.Length + bytes.Length];
                Buffer.BlockCopy(preamble, 0, result, 0, preamble.Length);
                Buffer.BlockCopy(bytes, 0, result, preamble.Length, bytes.Length);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting heritages to CSV");
                throw;
            }
        }

        private string GenerateHeritageCsv(IEnumerable<HeritageResponse> heritages)
        {
            var sb = new System.Text.StringBuilder();

            // ================================
            // HEADER CẤP 1 (TIẾNG VIỆT)
            // ================================
            sb.AppendLine(string.Join(",",
                "Mã ID",
                "Tên di sản",
                "Mô tả",
                "Nội dung – Lịch sử",
                "Nội dung – Nghi lễ",
                "Nội dung – Giá trị",
                "Nội dung – Bảo tồn",             
                "Danh mục",                      
                "Danh sách thẻ",
                "Ngày tạo",
                "Ngày cập nhật",

                // Location (không gộp)
                "Địa điểm – Tỉnh/Thành",
                "Địa điểm – Quận/Huyện",
                "Địa điểm – Phường/Xã",
                "Địa điểm – Chi tiết",

                

                // Occurrence
                "Loại sự kiện",
                "Lịch (Âm/Dương)",
                "Ngày bắt đầu",
                "Tháng bắt đầu",
                "Ngày kết thúc",
                "Tháng kết thúc",
                "Tần suất"
            ));

            // ================================
            // HEADER CẤP 2 (THUỘC TÍNH CON)
            // ================================
            sb.AppendLine(string.Join(",",
                "Id",
                "Name",
                "Description",
                "History",
                "Rituals",
                "Values",
                "Preservation",
              
                "CategoryName",
                "TagNames",
                "CreatedAt",
                "UpdatedAt",

                "Province",
                "District",
                "Ward",
                "AddressDetail",                      

                "OccurrenceType",
                "CalendarType",
                "StartDay",
                "StartMonth",
                "EndDay",
                "EndMonth",
                "Frequency"
            ));

            // ================================
            // XỬ LÝ TỪNG HERITAGE
            // ================================
            foreach (var h in heritages)
            {
                var loc = h.Locations?.FirstOrDefault();
                var occ = h.Occurrences?.FirstOrDefault();

                var tagIds = string.Join(";", h.Tags.Select(t => t.Id));
                var tagNames = string.Join(";", h.Tags.Select(t => t.Name));

                // Content JSON → tách thành 4 phần
                var (history, rituals, values, preservation) = ExtractContentSections(h.Content);

                sb.AppendLine(string.Join(",",
                    Quote(h.Id),
                    Quote(h.Name),
                    Quote(h.Description),

                    Quote(history),
                    Quote(rituals),
                    Quote(values),
                    Quote(preservation),

                  
                    Quote(h.CategoryName),
                    Quote(tagNames),

                    Quote(h.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")),
                    Quote(h.UpdatedAt?.ToString("yyyy-MM-dd HH:mm:ss") ?? ""),

                    Quote(loc?.Province),
                    Quote(loc?.District),
                    Quote(loc?.Ward),
                    Quote(loc?.AddressDetail),
                 

                    Quote(occ?.OccurrenceType),
                    Quote(occ?.CalendarType),
                    Quote(occ?.StartDay),
                    Quote(occ?.StartMonth),
                    Quote(occ?.EndDay),
                    Quote(occ?.EndMonth),
                    Quote(occ?.Frequency)
                ));
            }

            return sb.ToString();
        }

        private string Quote(object value)
        {
            if (value == null) return "\"\"";
            return $"\"{value.ToString().Replace("\"", "'")}\"";
        }

        // ================================
        // TÁCH CONTENT JSON → 4 TRƯỜNG
        // ================================
        private (string History, string Rituals, string Values, string Preservation)
            ExtractContentSections(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return ("", "", "", "");

            try
            {
                var content = System.Text.Json.JsonSerializer.Deserialize<HeritageContent>(json);

                return (
                    BlocksToString(content?.History),
                    BlocksToString(content?.Rituals),
                    BlocksToString(content?.Values),
                    BlocksToString(content?.Preservation)
                );
            }
            catch
            {
                return ("", "", "", "");
            }
        }

        private string BlocksToString(List<HeritageDescriptionBlock>? blocks)
        {
            if (blocks == null) return "";

            return string.Join("\n\n", blocks.Select(b =>
                b.Type == "paragraph"
                    ? b.Content
                    : string.Join("\n", b.Items.Select(i => $"• {i}"))
            ));
        }


        private string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
                return "";

            return field.Replace("\"", "\"\"")
                        .Replace("\n", " ")
                        .Replace("\r", "");
        }
    }
}
