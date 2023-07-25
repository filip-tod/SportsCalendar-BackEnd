using SportCalendar.Common;
using SportCalendar.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportCalendar.RepositoryCommon
{
    public interface IRoleRepository
    {
        Task<List<Role>> GetAllAsync(Sorting ordering, BaseFiltering filtering);
    }
}
