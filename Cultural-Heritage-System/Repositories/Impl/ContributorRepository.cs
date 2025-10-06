using Cultural_Heritage_System.DataAccessObjects;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Cultural_Heritage_System.Repositories.Impl
{
    public class ContributorRepository : BaseRepository< Contributor>, IContributorRepository
    {
        private readonly  ContributorDAO _entityDAO;

        public  ContributorRepository(ContributorDAO dao) : base(dao)
        {
            _entityDAO = dao;
        }

        public async Task<Contributor?> GetContributorById(int id)
        {
            return await _entityDAO.GetContributorById(id);
        }

        public IQueryable<Contributor> GetContributorsQueryable()
        {
            return _entityDAO.GetContributorsQueryable();
        }

        public async Task<Contributor?> GetContributorByUserId(int id)
        {
            return await _entityDAO.GetContributorByUserId(id);
        }
    }
}