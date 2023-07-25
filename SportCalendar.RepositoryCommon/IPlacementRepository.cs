using SportCalendar.Common;
using SportCalendar.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportCalendar.RepositoryCommon
{
    public interface IPlacementRepository
    {
        Task<PagedList<Placement>> PlacementGetFilteredAync(Paging paging, Sorting sorting, PlacementFilter placementFilter);

        Task<bool> PlacementPostAsync(Placement placement);

        Task<bool> PlacementDeleteAsync(Guid id, Placement placement);

        Task<bool> PlacementPutAsync(Guid id, Placement placement);

    }
}
