using SportCalendar.Common;
using SportCalendar.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportCalendar.ServiceCommon
{
    public interface IPlacementService
    {
        Task<PagedList<Placement>> PlacementGetFilteredAsync(Paging paging, Sorting sorting, PlacementFilter placementFilter);

        Task<bool> PlacementPostAsync(Placement placement);

        Task<bool> PlacementDeleteAsync(Guid id);

        Task<bool> PlacementPutAsync(Guid id, Placement placement);
    }
}
