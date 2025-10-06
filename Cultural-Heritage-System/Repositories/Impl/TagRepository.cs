using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class TagRepository : BaseRepository<Tag>, ITagRepository
    {
        private readonly TagDAO _entityDAO;

        public TagRepository(TagDAO dao) : base(dao)
        {
            _entityDAO = dao;
        }
        // Tạm thời 
        public IQueryable<Tag> GetTagsQueryable()
        {
            return _entityDAO.GetTagsQueryable();
        }
    }
}
