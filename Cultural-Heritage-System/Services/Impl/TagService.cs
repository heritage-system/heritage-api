using AutoMapper;
using Cultural_Heritage_System.Dtos.Request.Tag;
using Cultural_Heritage_System.Dtos.Response.Tag;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;

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

            Tag tag = mapper.Map<Tag>(request);
            await tagRepository.AddAsync(tag);
            return mapper.Map<CreateTagResponse>(tag);
        }

        public async Task<DeleteTagResponse> DeleteTag(DeleteTagRequest request)
        {
            Tag tag = mapper.Map<Tag>(request);

            await tagRepository.DeleteAsync(tag);

            return mapper.Map<DeleteTagResponse>(tag);

        }
        public async Task<UpdateTagResponse> UpdateTag(UpdateTagRequest request)
        {
            Tag tag = mapper.Map<Tag>(request);

            await tagRepository.UpdateAsync(tag);

            return mapper.Map<UpdateTagResponse>(tag);
        }
        public IQueryable<Tag> GetTagsQueryable()
        {
            return tagRepository.GetTagsQueryable();
        }


    }
}
