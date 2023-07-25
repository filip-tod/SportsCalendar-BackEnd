using SportCalendar.Common;
using SportCalendar.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportCalendar.ServiceCommon
{
    public interface ILocationService
    {
        Task<List<Location>> GetAllREST(Paging paging, Sorting sorting);
        Task<Location> GetById(Guid id);
        Task<Location> Create(Location location);
        Task<Location> Put(Location location, Guid id);
        Task<bool> Delete(Guid id);
    }
}
