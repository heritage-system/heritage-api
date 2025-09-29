using AutoMapper;
using Azure.Core;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Request.Heritage;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Contribution;
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
        private readonly UserRepository _userRepository;
        private readonly LocationRepository _locationRepository;
        private readonly HeritageLocationRepository _heritageLocationRepository;
        private readonly HeritageOccurrenceRepository _heritageOccurrenceRepository;
        private readonly HeritageMediaRepository _heritageMediaRepository;
        private readonly HeritageTagRepository _heritageTagRepository;
        private readonly ICloudinaryService _cloudinaryService;

        public HeritageService(HeritageRepository heritageRepository, IHttpContextAccessor httpContextAccessor , 
            IMapper mapper, ILogger<HeritageService> logger, LocationRepository locationRepository,
            HeritageLocationRepository heritageLocationRepository, HeritageOccurrenceRepository heritageOccurrenceRepository,
            HeritageMediaRepository heritageMediaRepository, HeritageTagRepository heritageTagRepository, ICloudinaryService cloudinaryService,UserRepository userRepository)
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
                //heritage.Description = request.Description;
                heritage.CategoryId = request.CategoryId;
                heritage.IsFeatured = request.IsFeatured;

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

        public async Task<List<HeritageNameSearchResponse>> SearchListHeritageName(string keyword)
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


    }
}
