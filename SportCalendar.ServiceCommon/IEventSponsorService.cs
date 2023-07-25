using SportCalendar.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportCalendar.ServiceCommon
{
    public interface IEventSponsorService
    {
        Task<List<EventSponsor>> EventSponsorGetAsync(Guid eventId);

        Task<bool> EventSponsorPostAsync(EventSponsor eventSponsor);

        Task<bool> EventSponsorDeleteAsync(Guid id);

        Task<bool> EventSponsorPutAsync(Guid id, EventSponsor eventSponsor);
    }
}
