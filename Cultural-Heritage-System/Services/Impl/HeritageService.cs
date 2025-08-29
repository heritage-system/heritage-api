using AutoMapper;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Request;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Middlewares;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.IO;

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

        public async Task<IEnumerable<HeritageResponse>> GetAllAsync()
        {
            var heritages = await _heritageRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<HeritageResponse>>(heritages);
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
                // 1. Tạo Heritage
                var heritage = _mapper.Map<Heritage>(request);
                heritage.CreatedBy = "accountIdClaim"; 
                heritage.CreatedAt = DateTime.UtcNow;
                heritage.GenerateUnsignedFields();

               

                // 2. Xử lý Media (Upload Cloudinary + Lưu DB)
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
                            //HeritageId = heritage.Id,
                            Url = uploadedUrl,
                            MediaType = typeEnum
                        });
                    }

                    //await _heritageMediaRepository.AddRangeAsync(mediaEntities);
                    heritage.Media = mediaEntities;
                }

                await _heritageRepository.AddAsync(heritage);
                await _dbContext.SaveChangesAsync(); // Cần lưu để có Heritage.Id



                // 3. Xử lý Tags
                if (request.TagIds?.Any() == true)
                {
                    var heritageTags = request.TagIds.Select(tagId => new HeritageTag
                    {
                        HeritageId = heritage.Id,
                        TagId = tagId
                    }).ToList();

                    await _heritageTagRepository.AddRangeAsync(heritageTags);
                }

                // 4. Xử lý Locations
                List<Location> newLocations = new();
                if (request.Locations?.Any() == true)
                {
                    newLocations = request.Locations.Select(loc =>
                    {
                        var location = new Location
                        {
                            Province = loc.Province,
                            District = loc.District,
                            Ward = loc.Ward,
                            AddressDetail = loc.AddressDetail,
                            Latitude = loc.Latitude,
                            Longitude = loc.Longitude
                        };
                        location.GenerateUnsignedFields();
                        return location;
                    }).ToList();

                    await _locationRepository.AddRangeAsync(newLocations);
                    await _dbContext.SaveChangesAsync(); // Lưu để có Location.Id

                    var heritageLocations = newLocations.Select(l => new HeritageLocation
                    {
                        HeritageId = heritage.Id,
                        LocationId = l.Id
                    }).ToList();

                    await _heritageLocationRepository.AddRangeAsync(heritageLocations);
                }

                // 5. Xử lý Occurrences
                if (request.Occurrences?.Any() == true)
                {
                    var occurrenceEntities = request.Occurrences.Select(o => new HeritageOccurrence
                    {
                        HeritageId = heritage.Id,
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

                    await _heritageOccurrenceRepository.AddRangeAsync(occurrenceEntities);
                }

                // 6. Save tất cả thay đổi
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                // 7. Map response
                var response = _mapper.Map<HeritageResponse>(heritage);
                response.Media = mediaEntities.Select(m => new MediaResponse
                {
                    Url = m.Url,
                    MediaType = m.MediaType.ToString()
                }).ToList();

                response.Tags = request.TagIds?.Select(t => new TagResponse { Id = t }).ToList();

                response.Locations = newLocations.Select(l => new LocationResponse
                {
                    Province = l.Province,
                    District = l.District,
                    Ward = l.Ward,
                    AddressDetail = l.AddressDetail,
                    Latitude = l.Latitude,
                    Longitude = l.Longitude
                }).ToList();

                response.Occurrences = request.Occurrences?.Select(o => new OccurrenceResponse
                {
                    OccurrenceType = o.OccurrenceType.ToString(),
                    CalendarType = o.CalendarType?.ToString(),
                    StartDay = o.StartDay,
                    StartMonth = o.StartMonth,
                    EndDay = o.EndDay,
                    EndMonth = o.EndMonth,
                    Frequency = o.Frequency?.ToString(),
                    RecurrenceRule = o.RecurrenceRule,
                    Description = o.Description
                }).ToList();

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
            var accountIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(accountIdClaim))
            {
                throw new AppException(ErrorCode.UNAUTHORIZED);
            }
            var heritage = await _heritageRepository.GetByIdAsync(id);
            if (heritage == null)
                return null;

            _mapper.Map(request, heritage);
            heritage.UpdatedBy = accountIdClaim;
            heritage.UpdatedAt = DateTime.UtcNow;

            await _heritageRepository.UpdateAsync(heritage);

            return _mapper.Map<HeritageResponse>(heritage);
        }

        //trả về id vừa xóa
        public async Task<bool> DeleteAsync(long id)
        {
            var heritage = await _heritageRepository.GetByIdAsync(id);
            if (heritage == null)
                return false;

            await _heritageRepository.DeleteAsync(heritage);
            return true;
        }
    }
}
