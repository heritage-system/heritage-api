using Cultural_Heritage_System.Dtos.Request.Tag;
using Cultural_Heritage_System.Dtos.Response.Tag;
using Cultural_Heritage_System.Models;

namespace Cultural_Heritage_System.Services
{
    public interface ITagService
    {
        Task<CreateTagResponse> CreateTag(CreateTagRequest request);
        Task<UpdateTagResponse> UpdateTag(UpdateTagRequest request);
        Task<DeleteTagResponse> DeleteTag(DeleteTagRequest request);
        IQueryable<Tag> GetTagsQueryable();

    }
}
