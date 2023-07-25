using SportCalendar.Common;
using SportCalendar.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportCalendar.ServiceCommon
{
    public interface IEventService
    {
        Task<PagedList<EventView>> GetEventsAsync(Sorting sorting, Paging paging, EventFilter filtering);
        Task<EventView> GetEventAsync(Guid id);
        Task<bool> DeleteEventAsync(Guid id);
        Task<EventModel> PostEventAsync(EventModel eventModel);
        Task<EventModel> UpdateEventAsync(Guid id, EventModel eventModel);
    }
}
