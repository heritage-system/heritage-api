using AutoMapper;
using Cultural_Heritage_System.Dtos.Request.Tag;
using Cultural_Heritage_System.Dtos.Response;
using Cultural_Heritage_System.Dtos.Response.Tag;
using Cultural_Heritage_System.Helpers;
using Cultural_Heritage_System.Middlewares;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Services.Impl
{
    public class TagService : ITagService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly TagRepository tagRepository;

        private readonly IMapper mapper;

        private readonly ILogger<TagService> logger;

        public TagService(TagRepository tagRepository, ILogger<TagService> logger, IMailService mailService,
            IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            this.tagRepository = tagRepository;
            this.logger = logger;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<CreateTagResponse> CreateTag(CreateTagRequest request)
        {
            // Validate tag name
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                logger.LogError("Invalid Tag Name");
                throw new AppException(ErrorCode.INVALID_TAG_NAME);
            }

            var existingTag = tagRepository
       .GetTagsQueryable()
       .FirstOrDefault(t => t.Name == request.Name);
            if (existingTag != null)
            {
                logger.LogError("Tag Existed");
                throw new AppException(ErrorCode.TAG_EXISTED);
            }
            Tag tag = mapper.Map<Tag>(request);
            tag.GenerateUnsignedFields();
            await tagRepository.AddAsync(tag);
            return mapper.Map<CreateTagResponse>(tag);
        }

        public async Task<DeleteTagResponse> DeleteTag(DeleteTagRequest request)
        {
            // Fetch from DB
            var tag = await tagRepository.GetTagsQueryable()
                                         .FirstOrDefaultAsync(t => t.Id == request.id);

            if (tag == null)
            {
                logger.LogError("Tag Not Existed");
                throw new AppException(ErrorCode.TAG_NOT_EXISTED);
            }

            mapper.Map(request, tag);

            await tagRepository.DeleteAsync(tag);

            return mapper.Map<DeleteTagResponse>(tag);

        }
        public async Task<UpdateTagResponse> UpdateTag(UpdateTagRequest request)
        {
            var tag = await tagRepository.GetTagsQueryable()
                              .FirstOrDefaultAsync(t => t.Id == request.id);

            if (tag == null)
            {
                logger.LogError("Tag Not Existed");
                throw new AppException(ErrorCode.TAG_NOT_EXISTED);
            }


            mapper.Map(request, tag);


            tag.GenerateUnsignedFields();

            await tagRepository.UpdateAsync(tag);

            return mapper.Map<UpdateTagResponse>(tag);
        }

        public IQueryable<Tag> GetTagsQueryable()
        {
            return tagRepository.GetTagsQueryable();
        }

        public async Task<PageResponse<TagSearchResponse>> SearchTagsAsync(TagSearchRequest request)
        {
            var query = GetTagsQueryable();

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                var searchTerm = request.Keyword.Trim().ToLower();
                var unsignedTerm = StringHelper.RemoveDiacritics(searchTerm);

                query = query.Where(h => h.Name.ToLower().Contains(searchTerm) || h.NameUnsigned.Contains(unsignedTerm));

            }

            var paged = await query
                .OrderBy(t => t.Name)
                .Select(t => mapper.Map<TagSearchResponse>(t))
                .ToPagedResponseAsync(request.Page, request.PageSize);

            return paged;
        }
    }
}
