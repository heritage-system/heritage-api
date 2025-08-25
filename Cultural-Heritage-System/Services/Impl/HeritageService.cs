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

        private readonly HeritageCoordinateRepository _heritageCoordinateRepository;
        private readonly HeritageLocationRepository _heritageLocationRepository;
        private readonly HeritageOccurrenceRepository _heritageOccurrenceRepository;
        private readonly HeritageMediaRepository _heritageMediaRepository;
        private readonly HeritageTagRepository _heritageTagRepository;
        private readonly ICloudinaryService _cloudinaryService;

        public HeritageService(HeritageRepository heritageRepository, IHttpContextAccessor httpContextAccessor , 
            IMapper mapper, ILogger<HeritageService> logger, AppDbContext appDbContext, HeritageCoordinateRepository heritageCoordinateRepository,
            HeritageLocationRepository heritageLocationRepository, HeritageOccurrenceRepository heritageOccurrenceRepository,
            HeritageMediaRepository heritageMediaRepository, HeritageTagRepository heritageTagRepository, ICloudinaryService cloudinaryService)
        {
            _heritageRepository = heritageRepository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _logger = logger;
            _dbContext = appDbContext;
            _heritageCoordinateRepository = heritageCoordinateRepository;
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
            var accountIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(accountIdClaim))
            {
                throw new AppException(ErrorCode.UNAUTHORIZED);
            }

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var heritage = _mapper.Map<Heritage>(request);
                heritage.CreatedBy = accountIdClaim;
                heritage.CreatedAt = DateTime.UtcNow;
                heritage.GenerateUnsignedFields();

                await _heritageRepository.AddAsync(heritage);
                await _dbContext.SaveChangesAsync();

                // Media
                if (request.Media != null && request.Media.Any())
                {
                    var mediaEntities = new List<HeritageMedia>();

                    foreach (var media in request.Media)
                    {
                        using var stream = media.File.OpenReadStream();
                        var typeEnum = Enum.Parse<MediaType>(media.Type, true);

                        string uploadedUrl = typeEnum switch
                        {
                            MediaType.IMAGE => await _cloudinaryService.UploadImageAsync(stream, media.File.FileName),
                            MediaType.VIDEO => await _cloudinaryService.UploadVideoAsync(stream, media.File.FileName),
                            MediaType.DOCUMENT => await _cloudinaryService.UploadDocumentAsync(stream, media.File.FileName),
                            _ => throw new ArgumentException("Invalid media type")
                        };


                        mediaEntities.Add(new HeritageMedia
                        {
                            HeritageId = heritage.Id,
                            Url = uploadedUrl,
                            MediaType = typeEnum 
                        });
                    }

                    await _heritageMediaRepository.AddRangeAsync(mediaEntities);
                }




                // Tags
                //if (request.TagIds != null && request.TagIds.Any())
                //{
                //    var heritageTags = request.TagIds.Select(tagId => new HeritageTag
                //    {
                //        HeritageId = heritage.Id,
                //        TagId = tagId
                //    }).ToList();

                //    await _heritageTagRepository.AddRangeAsync(heritageTags);
                //}


                // Locations
                if (request.Locations != null && request.Locations.Any())
                {
                    var newLocations = new List<Location>();
                    foreach (var loc in request.Locations)
                    {
                        var location = new Location
                        {
                            Name = loc.Name,
                            Code = loc.Code
                        };
                        newLocations.Add(location);
                    }

                    //await _heritageLocationRepository.AddAsync(newLocations);

                    var heritageLocations = newLocations.Select(l => new HeritageLocation
                    {
                        HeritageId = heritage.Id,
                        LocationId = l.Id
                    }).ToList();

                    await _heritageLocationRepository.AddRangeAsync(heritageLocations);
                }


                // Occurrences
                //if (request.Occurrences != null && request.Occurrences.Any())
                //{
                //    var occurrenceEntities = request.Occurrences.Select(o => new HeritageOccurrence
                //    {
                //        HeritageId = heritage.Id,
                //        Date = o.Date,
                //        Description = o.Description
                //    }).ToList();

                //    await _heritageOccurrenceRepository.AddRangeAsync(occurrenceEntities);
                //}

                // Coordinates
                //if (request.Coordinates != null && request.Coordinates.Any())
                //{
                //    var coordinateEntities = request.Coordinates.Select(c => new HeritageCoordinate
                //    {
                //        HeritageId = heritage.Id,
                //        Latitude = c.Latitude,
                //        Longitude = c.Longitude
                //    }).ToList();

                //    await _heritageCoordinateRepository.AddRangeAsync(coordinateEntities);
                //}

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                var response = _mapper.Map<HeritageResponse>(heritage);
                response.Media = request.Media?.Select(m => new MediaResponse { MediaType = m.Type }).ToList();
                //response.Tags = request.TagIds?.Select(t => new TagResponse { Id = t.Id }).ToList();
                response.Locations = request.Locations?.Select(l => new LocationResponse { Name = l.Name }).ToList();
                //response.Occurrences = request.Occurrences?.Select(o => new OccurrenceResponse { Date = o.Date, Description = o.Description }).ToList();
                //response.Coordinates = request.Coordinates?.Select(c => new CoordinateResponse { Latitude = c.Latitude, Longitude = c.Longitude }).ToList();

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
