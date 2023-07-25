using SportCalendar.Common;
using SportCalendar.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportCalendar.ServiceCommon
{
    public interface IRoleService
    {
        Task<List<Role>> GetAllAsync(Sorting sorting, BaseFiltering filtering);
    }
}
