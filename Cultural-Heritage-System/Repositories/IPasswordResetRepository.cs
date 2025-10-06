using CloudinaryDotNet.Core;
using Cultural_Heritage_System.Models;
using Cultural_Heritage_System.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cultural_Heritage_System.Repositories
{
    public interface IPasswordResetRepository : IBaseRepository<PasswordReset>
    {

        Task<PasswordReset?> GetLatestByUserIdAsync(int userId);
       
    }
}
