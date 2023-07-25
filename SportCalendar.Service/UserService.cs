using SportCalendar.Common;
using SportCalendar.Model;
using SportCalendar.RepositoryCommon;
using SportCalendar.ServiceCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace SportCalendar.Service
{
    public class UserService : IUserService
    {
        public UserService(IUserRepository userRepository)
        {
            UserRepository = userRepository;
        }
        protected IUserRepository UserRepository { get; set; }
        public async Task<PagedList<User>> GetAllAsync(Paging paging, Sorting sorting, UserFiltering filtering)
        {
            string[] orderBy = { "FirstName", "LastName", "DateCreated", "DateUpdated", "Username", "UpdatedByUserId" };

            if (!orderBy.Contains(sorting.OrderBy))
            {
                throw new Exception();
            }

            PagedList<User> usersList = await UserRepository.GetAllAsync(paging, sorting, filtering);
            if (usersList != null)
            {
                return usersList;
            }
            return null;
        }
        public async Task<User> GetByUserIdAsync(Guid id)
        {
            bool isUser = await UserRepository.CheckEntryByUserIdAsync(id);

            if (isUser)
            {
                User result = await UserRepository.GetByUserIdAsync(id);

                if (result != null)
                {
                    return result;
                };
            }
            return null;
        }
        public async Task<User> InsertUserAsync(User newUser)
        {
            bool isUser = await UserRepository.CheckEntryByUserIdAsync(newUser.Id);

            if (!isUser)
            {
                Guid newGuid = Guid.NewGuid();

                string hashPassword = PasswordHasher.HashPassword(newUser.Password);

                newUser.Id = newGuid;
                newUser.Password = hashPassword;
                newUser.RoleId = Guid.Parse("f81e3cdf-5c78-49b9-a72a-7c12a7e5b814");
                newUser.IsActive = true;
                newUser.UpdatedByUserId = newGuid;
                newUser.DateCreated = DateTime.UtcNow;
                newUser.DateUpdated = DateTime.UtcNow;                                             

                User result = await UserRepository.InsertUserAsync(newUser);

                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }
        public async Task<User> UpdateUserAsync(Guid id, User updateUser)
        {
            bool isUser = await UserRepository.CheckEntryByUserIdAsync(id);

            if (isUser)
            {
                ClaimsIdentity identity = HttpContext.Current.User.Identity as ClaimsIdentity;
                string userId = identity.FindFirst("Id")?.Value;

                updateUser.UpdatedByUserId = Guid.Parse(userId);                
                updateUser.DateUpdated = DateTime.UtcNow;
                if (updateUser.Password != null)
                {
                    string hashPassword = PasswordHasher.HashPassword(updateUser.Password);
                    updateUser.Password = hashPassword;
                }

                User result = await UserRepository.UpdateUserAsync(id, updateUser);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }
        public async Task<User> DeleteUserAsync(Guid id)
        {
            User deleteUser = await UserRepository.GetByUserIdAsync(id);

            if (deleteUser != null)
            {
                ClaimsIdentity identity = HttpContext.Current.User.Identity as ClaimsIdentity;
                string userId = identity.FindFirst("Id")?.Value;

                deleteUser.UpdatedByUserId = Guid.Parse(userId);
                deleteUser.DateUpdated = DateTime.UtcNow;

                User result = await UserRepository.DeleteUserAsync(id, deleteUser);
                return result;
            }
            return null;
        }
    }
}
