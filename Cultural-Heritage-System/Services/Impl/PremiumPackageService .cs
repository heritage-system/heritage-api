using AutoMapper;
using AutoMapper.QueryableExtensions;
using Cultural_Heritage_System.Common;
using Cultural_Heritage_System.Dtos.Request.Contributor;
using Cultural_Heritage_System.Dtos.Request.PremiumPackage;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Contributor;
using Cultural_Heritage_System.Dtos.Response.PremiumPackage;
using Cultural_Heritage_System.Helpers;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Cultural_Heritage_System.Repositories.Impl;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static StackExchange.Redis.Role;

namespace Cultural_Heritage_System.Services.Impl
{
    public class PremiumPackageService : IPremiumPackageService
    {
        private readonly IPremiumPackageRepository _packageRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<PremiumPackageService> logger;

        public PremiumPackageService(IPremiumPackageRepository packageRepo, IMapper mapper, ILogger<PremiumPackageService> logger)
        {
            _packageRepo = packageRepo;
            _mapper = mapper;
            this.logger = logger;
        }

        public async Task<IEnumerable<PremiumPackageResponse>> GetAllAsync()
        {
            var data = await _packageRepo.GetAllAsync();
            return _mapper.Map<IEnumerable<PremiumPackageResponse>>(data);
        }

        public async Task<PageResponse<PremiumPackageResponse>> SearchPremiumPackagesAsync(PremiumPackageSearchRequest request)
        {
            try
            {
                var query = _packageRepo.GetQueryable();

                if (!string.IsNullOrWhiteSpace(request.Name))
                {
                    var searchTerm = request.Name.Trim().ToLower();
                    query = query.Where(h => h.Name.ToLower().Contains(searchTerm));
                }

                query = request.SortBy switch
                {
                    SortBy.IDASC => query.OrderBy(h => h.Id),
                    SortBy.IDDESC => query.OrderByDescending(h => h.Id),
                    SortBy.NAMEASC => query.OrderBy(h => h.Name),
                    SortBy.NAMEDESC => query.OrderByDescending(h => h.Name),
                    _ => query.OrderByDescending(h => h.CreatedAt)
                };

                var dtoQuery = query.ProjectTo<PremiumPackageResponse>(_mapper.ConfigurationProvider);

                var response = await dtoQuery.ToPagedResponseAsync(request.Page, request.PageSize);
                return response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error searching premium packages");
                throw;
            }
        }

        public IQueryable<PremiumPackage> GetPremiumPackagesQueryable()
        {
            return _packageRepo.GetQueryable();
        }

        public async Task<IEnumerable<PremiumPackageResponse>> GetActivePackagesAsync()
        {
            var data = await _packageRepo.GetActivePackageAsyns();
            return _mapper.Map<IEnumerable<PremiumPackageResponse>>(data);
        }

        public async Task<PremiumPackageResponse?> GetByIdAsync(int id)
        {
            var pkg = await _packageRepo.GetByIdAsync(id);
            return pkg == null ? null : _mapper.Map<PremiumPackageResponse>(pkg);
        }

        public async Task<PremiumPackageResponse> CreateAsync(PremiumPackageCreateRequest req)
        {
            var entity = _mapper.Map<PremiumPackage>(req);

            entity.PackageBenefits = (req.BenefitIds ?? new List<int>())
                .Distinct()
                .Select(bid => new PremiumPackageBenefit { BenefitId = bid })
                .ToList();

            await _packageRepo.AddAsync(entity);

            var saved = await _packageRepo.GetByIdAsync(entity.Id) ?? entity;
            return _mapper.Map<PremiumPackageResponse>(saved);
        }

        public async Task<PremiumPackageResponse> UpdateAsync(int id, PremiumPackageUpdateRequest req)
        {
            var entity = await _packageRepo.GetByIdAsync(id)
                         ?? throw new Exception("Package not found");

            _mapper.Map(req, entity);

            if (req.BenefitIds != null)
            {
                entity.PackageBenefits.Clear();
                foreach (var bid in req.BenefitIds.Distinct())
                {
                    entity.PackageBenefits.Add(new PremiumPackageBenefit { BenefitId = bid });
                }
            }

            await _packageRepo.UpdateAsync(entity);

            var refreshed = await _packageRepo.GetByIdAsync(id) ?? entity;
            return _mapper.Map<PremiumPackageResponse>(refreshed);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _packageRepo.GetByIdAsync(id);
            if (entity == null) return false;

            await _packageRepo.DeleteAsync(entity);
            await _packageRepo.SaveChangesAsync();
            return true;
        }
    }


}
