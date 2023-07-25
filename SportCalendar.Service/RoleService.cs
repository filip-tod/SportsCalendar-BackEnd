using SportCalendar.Common;
using SportCalendar.Model;
using SportCalendar.Repository;
using SportCalendar.RepositoryCommon;
using SportCalendar.ServiceCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace SportCalendar.Service
{
    public class RoleService : IRoleService
    {
        public RoleService(IRoleRepository roleRepository) 
        {
            RoleRepository = roleRepository;
        }
        protected IRoleRepository RoleRepository { get; set; }   

        public async Task<List<Role>> GetAllAsync(Sorting sorting, BaseFiltering filtering)
        {
            string[] orderBy = { "Id", "Access", "IsActive", "DateUpdated" };

            if (sorting.OrderBy != null && !orderBy.Contains(sorting.OrderBy))
            {
                throw new Exception();
            }
            List<Role> rolesList = await RoleRepository.GetAllAsync(sorting, filtering);
            if (rolesList != null)
            {
                return rolesList;
            }
            return null;
        }
    }
}
