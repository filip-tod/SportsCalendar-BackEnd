using SportCalendar.Model;
using SportCalendar.RepositoryCommon;
using SportCalendar.ServiceCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportCalendar.Service
{
    public class EventSponsorService : IEventSponsorService
    {
        private IEventSponsorRepository eventSponsorRepository;

        public EventSponsorService(IEventSponsorRepository repository)
        {
            eventSponsorRepository = repository;
        }

        public async Task<List<EventSponsor>> EventSponsorGetAsync(Guid eventId)
        {
            return (await eventSponsorRepository.EventSponsorGetAsync(eventId));
        }

        public async Task<bool> EventSponsorPostAsync(EventSponsor eventSponsor)
        {
            eventSponsor.Id = Guid.NewGuid();
            eventSponsor.IsAcive = true;
            eventSponsor.UpdatedByUserId = Guid.Parse("bbb75a72-228e-4dfa-8e58-2beed90b0c92");
            eventSponsor.CreatedByUserId = Guid.Parse("bbb75a72-228e-4dfa-8e58-2beed90b0c92");
            eventSponsor.DateCreated = DateTime.UtcNow;
            eventSponsor.DateUpdated = DateTime.UtcNow;

            return (await eventSponsorRepository.EventSponsorPostAsync(eventSponsor));
        }

        public async Task<bool> EventSponsorDeleteAsync(Guid id)
        {
            EventSponsor eventSponsor = new EventSponsor();
            eventSponsor.UpdatedByUserId = Guid.Parse("bbb75a72-228e-4dfa-8e58-2beed90b0c92");
            eventSponsor.DateUpdated = DateTime.UtcNow;
            return (await eventSponsorRepository.EventSponsorDeleteAsync(id, eventSponsor));
        }

        public async Task<bool> EventSponsorPutAsync(Guid id, EventSponsor eventSponsor)
        {
            eventSponsor.UpdatedByUserId = Guid.Parse("0d3fa5c2-684c-4d88-82fd-cea2197c6e86");
            eventSponsor.DateUpdated = DateTime.UtcNow;
            return (await eventSponsorRepository.EventSponsorPutAsync(id, eventSponsor));
        }
    }
}
