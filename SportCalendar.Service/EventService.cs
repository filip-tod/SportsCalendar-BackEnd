using SportCalendar.Common;
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
    public class EventService : IEventService
    {
        private IEventRepository _eventRepository;

        public EventService(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }
        public async Task<PagedList<EventView>> GetEventsAsync(Sorting sorting, Paging paging, EventFilter filtering)
        {
            return await _eventRepository.GetEventsAsync(sorting, paging, filtering);
        }
        public async Task<EventView> GetEventAsync(Guid id)
        {
            return await _eventRepository.GetEventAsync(id);
        }
        public async Task<bool> DeleteEventAsync(Guid id)
        {
            return await _eventRepository.DeleteEventAsync(id);
        }

        public async Task<EventModel> PostEventAsync(EventModel eventModel)
        {
            Guid userId = Guid.Parse("0d3fa5c2-684c-4d88-82fd-cea2197c6e86");
            eventModel.CreatedByUserId = userId;
            eventModel.UpdatedByUserId = userId;
            eventModel.DateUpdated = DateTime.UtcNow;
            eventModel.DateCreated = DateTime.UtcNow;
            eventModel.IsActive = true;
            //eventModel.LocationId = Guid.Parse("58c9e9bb-1c9d-4994-bed8-8395c8d56712");//frontend
            //eventModel.SportId = Guid.Parse("8cff62ec-0572-482d-bb66-7ffbfde4f271");//frontend
            return await _eventRepository.PostEventAsync(eventModel);
        }

        public async Task<EventModel> UpdateEventAsync(Guid id, EventModel eventModel)
        {
            Guid userId = Guid.Parse("0d3fa5c2-684c-4d88-82fd-cea2197c6e86");
            eventModel.UpdatedByUserId = userId;
            eventModel.DateUpdated = DateTime.UtcNow;
            return await _eventRepository.UpdateEventAsync(id, eventModel);
        }
    }
}
