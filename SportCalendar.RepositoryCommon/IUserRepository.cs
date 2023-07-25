using SportCalendar.Common;
using SportCalendar.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace SportCalendar.RepositoryCommon
{
    public interface IUserRepository
    {
        Task<PagedList<User>> GetAllAsync(Paging paging, Sorting sorting, UserFiltering filtering);
        Task<User> GetByUserIdAsync(Guid id);
        Task<User> InsertUserAsync(User newUser);
        Task<User> UpdateUserAsync(Guid id, User updateUser);
        Task<User> DeleteUserAsync(Guid id, User deleteUser);

        Task<bool> CheckEntryByUserIdAsync(Guid id);
    }
}
