using AutoMapper;
using Azure.Core;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Request.Heritage;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Heritage;
using Cultural_Heritage_System.Helpers;
using Cultural_Heritage_System.Middlewares;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Services.Impl
{
    public class HeritageService : IHeritageService
    {
        private readonly HeritageRepository _heritageRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly ILogger<HeritageService> _logger;
        private readonly AppDbContext _dbContext;

        private readonly LocationRepository _locationRepository;
        private readonly HeritageLocationRepository _heritageLocationRepository;
        private readonly HeritageOccurrenceRepository _heritageOccurrenceRepository;
        private readonly HeritageMediaRepository _heritageMediaRepository;
        private readonly HeritageTagRepository _heritageTagRepository;
        private readonly ICloudinaryService _cloudinaryService;

        public HeritageService(HeritageRepository heritageRepository, IHttpContextAccessor httpContextAccessor , 
            IMapper mapper, ILogger<HeritageService> logger, AppDbContext appDbContext, LocationRepository locationRepository,
            HeritageLocationRepository heritageLocationRepository, HeritageOccurrenceRepository heritageOccurrenceRepository,
            HeritageMediaRepository heritageMediaRepository, HeritageTagRepository heritageTagRepository, ICloudinaryService cloudinaryService)
        {
            _heritageRepository = heritageRepository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _logger = logger;
            _dbContext = appDbContext;
            _locationRepository = locationRepository;
            _heritageLocationRepository = heritageLocationRepository;
            _heritageOccurrenceRepository = heritageOccurrenceRepository;
            _heritageMediaRepository = heritageMediaRepository;
            _heritageTagRepository = heritageTagRepository;
            _cloudinaryService = cloudinaryService;

        }

        public async Task<PageResponse<HeritageResponse>> GetAllAsync(int page,int pageSize, string? keyword = null,int? categoryId = null,int? tagId = null)
        {
            var query = _heritageRepository.GetAllQuery();

            if (!string.IsNullOrEmpty(keyword))
            {
                var lowerKeyword = keyword.Trim().ToLower();
                var unsignedTerm = StringHelper.RemoveDiacritics(lowerKeyword);
                query = query.Where(h => h.NameUnsigned.ToLower().Contains(unsignedTerm)
                                       || h.Name.ToLower().Contains(lowerKeyword));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(h => h.CategoryId == categoryId.Value);
            }

            if (tagId.HasValue)
            {
                query = query.Where(h => h.HeritageTags.Any(ht => ht.TagId == tagId.Value));
            }

            var pagedResult = await query.ToPagedResponseAsync(page, pageSize);

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
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                //var accountIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
                //if (string.IsNullOrEmpty(accountIdClaim))
                //    throw new AppException(ErrorCode.UNAUTHORIZED);

                // 1. Tạo Heritage
                var heritage = _mapper.Map<Heritage>(request);
                heritage.CreatedBy = "accountIdClaim";
                heritage.CreatedAt = DateTime.UtcNow;

                // 2. Xử lý Media (Upload Cloudinary)
                var mediaEntities = new List<HeritageMedia>();
                if (request.Media?.Any() == true)
                {
                    foreach (var media in request.Media)
                    {
                        if (media.File == null)
                            throw new ArgumentException("File is required for media upload.");

                        using var stream = media.File.OpenReadStream();

                        if (!Enum.TryParse<MediaType>(media.Type, true, out var typeEnum))
                            throw new ArgumentException($"Invalid media type: {media.Type}");

                        var uploadedUrl = typeEnum switch
                        {
                            MediaType.IMAGE => await _cloudinaryService.UploadImageAsync(stream, media.File.FileName),
                            MediaType.VIDEO => await _cloudinaryService.UploadVideoAsync(stream, media.File.FileName),
                            MediaType.DOCUMENT => await _cloudinaryService.UploadDocumentAsync(stream, media.File.FileName),
                            _ => throw new ArgumentException("Invalid media type")
                        };

                        mediaEntities.Add(new HeritageMedia
                        {
                            Url = uploadedUrl,
                            MediaType = typeEnum
                        });
                    }

                    heritage.Media = mediaEntities;
                }

                // 3. Xử lý Tags
                if (request.TagIds?.Any() == true)
                {
                    heritage.HeritageTags = request.TagIds.Select(tagId => new HeritageTag
                    {
                        TagId = tagId
                    }).ToList();
                }

                // 4. Xử lý Locations
                if (request.Locations?.Any() == true)
                {
                    var locationEntities = request.Locations.Select(loc => new Location
                    {
                        Province = loc.Province,
                        District = loc.District,
                        Ward = loc.Ward,
                        AddressDetail = loc.AddressDetail,
                        Latitude = loc.Latitude,
                        Longitude = loc.Longitude
                    }).ToList();

                    heritage.HeritageLocations = locationEntities.Select(location => new HeritageLocation
                    {
                        Location = location
                    }).ToList();
                }

                // 5. Xử lý Occurrences
                if (request.Occurrences?.Any() == true)
                {
                    heritage.HeritageOccurrences = request.Occurrences.Select(o => new HeritageOccurrence
                    {
                        OccurrenceType = Enum.Parse<OccurrenceType>(o.OccurrenceType, true),
                        CalendarType = string.IsNullOrEmpty(o.CalendarType) ? null : Enum.Parse<CalendarType>(o.CalendarType, true),
                        StartDay = o.StartDay,
                        StartMonth = o.StartMonth,
                        EndDay = o.EndDay,
                        EndMonth = o.EndMonth,
                        Frequency = string.IsNullOrEmpty(o.Frequency) ? null : Enum.Parse<FestivalFrequency>(o.Frequency, true),
                        RecurrenceRule = o.RecurrenceRule,
                        Description = o.Description
                    }).ToList();
                }

                // Add heritage vào context
                await _dbContext.Heritages.AddAsync(heritage);

                // Lưu tất cả thay đổi 1 lần
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                // Map response
                var response = _mapper.Map<HeritageResponse>(heritage);
                return response;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<HeritageResponse> UpdateAsync(long id, HeritageUpdateRequest request)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                //var accountIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
                //if (string.IsNullOrEmpty(accountIdClaim))
                //    throw new AppException(ErrorCode.UNAUTHORIZED);

                var heritage = await _heritageRepository.GetByIdAsync(id);

                if (heritage == null)
                    return null;

                // 1. Cập nhật thông tin chính
                _mapper.Map(request, heritage);
                heritage.UpdatedBy = "accountIdClaim";

                // 2. Xử lý Media
                if (request.Media?.Any() == true)
                {
                    // Xóa media cũ
                    _dbContext.HeritageMedias.RemoveRange(heritage.Media);

                    var newMediaEntities = new List<HeritageMedia>();
                    foreach (var media in request.Media)
                    {
                        if (media.File == null)
                            throw new ArgumentException("File is required for media upload.");

                        using var stream = media.File.OpenReadStream();

                        if (!Enum.TryParse<MediaType>(media.Type, true, out var typeEnum))
                            throw new ArgumentException($"Invalid media type: {media.Type}");

                        var uploadedUrl = typeEnum switch
                        {
                            MediaType.IMAGE => await _cloudinaryService.UploadImageAsync(stream, media.File.FileName),
                            MediaType.VIDEO => await _cloudinaryService.UploadVideoAsync(stream, media.File.FileName),
                            MediaType.DOCUMENT => await _cloudinaryService.UploadDocumentAsync(stream, media.File.FileName),
                            _ => throw new ArgumentException("Invalid media type")
                        };

                        newMediaEntities.Add(new HeritageMedia
                        {
                            Url = uploadedUrl,
                            MediaType = typeEnum
                        });
                    }
                    heritage.Media = newMediaEntities;
                }

                // 3. Xử lý Tags
                _dbContext.HeritageTags.RemoveRange(heritage.HeritageTags);
                if (request.TagIds?.Any() == true)
                {
                    heritage.HeritageTags = request.TagIds.Select(tagId => new HeritageTag
                    {
                        TagId = tagId
                    }).ToList();
                }

                // 4. Xử lý Locations
                _dbContext.Locations.RemoveRange(heritage.HeritageLocations.Select(hl => hl.Location));
                heritage.HeritageLocations.Clear();

                if (request.Locations?.Any() == true)
                {
                    var locationEntities = request.Locations.Select(loc => new Location
                    {
                        Province = loc.Province,
                        District = loc.District,
                        Ward = loc.Ward,
                        AddressDetail = loc.AddressDetail,
                        Latitude = loc.Latitude,
                        Longitude = loc.Longitude
                    }).ToList();

                    heritage.HeritageLocations = locationEntities.Select(location => new HeritageLocation
                    {
                        Location = location
                    }).ToList();
                }

                // 5. Xử lý Occurrences
                _dbContext.HeritageOccurrences.RemoveRange(heritage.HeritageOccurrences);
                if (request.Occurrences?.Any() == true)
                {
                    heritage.HeritageOccurrences = request.Occurrences.Select(o => new HeritageOccurrence
                    {
                        OccurrenceType = Enum.Parse<OccurrenceType>(o.OccurrenceType, true),
                        CalendarType = string.IsNullOrEmpty(o.CalendarType) ? null : Enum.Parse<CalendarType>(o.CalendarType, true),
                        StartDay = o.StartDay,
                        StartMonth = o.StartMonth,
                        EndDay = o.EndDay,
                        EndMonth = o.EndMonth,
                        Frequency = string.IsNullOrEmpty(o.Frequency) ? null : Enum.Parse<FestivalFrequency>(o.Frequency, true),
                        RecurrenceRule = o.RecurrenceRule,
                        Description = o.Description
                    }).ToList();
                }

                await _heritageRepository.UpdateAsync(heritage);
                //await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return _mapper.Map<HeritageResponse>(heritage);
            }
            catch
            {
                await transaction.RollbackAsync();
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

    }
}
