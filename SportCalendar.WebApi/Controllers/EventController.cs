using SportCalendar.Common;
using SportCalendar.Model;
using SportCalendar.ModelCommon;
using SportCalendar.ServiceCommon;
using SportCalendar.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace SportCalendar.WebApi.Controllers
{
    public class EventController : ApiController
    {
        private IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        [AllowAnonymous]
        [Authorize(Roles ="Super_admin,Organizer,User")]
        public async Task<HttpResponseMessage> Get(string venue = null, string city = null, string county = null, decimal? rating = null,string sport = null, string orderBy = "Name", string sortOrder = "DESC", int pageSize = 200, int pageNumber = 1, string searchQuery = null, DateTime? fromDate = null, DateTime? toDate = null, DateTime? fromTime = null, DateTime? toTime = null)
        {
            try
            {
                Sorting sorting = new Sorting();
                sorting.SortOrder = sortOrder;
                sorting.OrderBy = orderBy;

                Paging paging = new Paging();
                paging.PageNumber = pageNumber;
                paging.PageSize = pageSize;

                EventFilter filter = new EventFilter(venue,sport,city,county,rating,searchQuery,fromDate,toDate,fromTime,toTime);

                PagedList<EventView> pagedList = await _eventService.GetEventsAsync(sorting, paging, filter);
                if (pagedList.Data.Any())
                {
                    //List<EventRest> eventsRest = MapEventsToRest(pagedList);
                    List<EventRest> eventsRest = MapEventsViewToRest(pagedList.Data);
                    PagedList<EventRest> pagedListRest = new PagedList<EventRest>();
                    pagedListRest.Data = eventsRest;
                    pagedListRest.TotalCount = pagedList.TotalCount;
                    pagedListRest.TotalPages = pagedList.TotalPages;
                    pagedListRest.PageSize = pagedList.PageSize;
                    pagedListRest.CurrentPage = pagedList.CurrentPage;

                    return Request.CreateResponse(HttpStatusCode.OK, pagedListRest);
                }
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }
        [HttpGet]
        [AllowAnonymous]
        [Authorize(Roles = "Super_admin,Organizer,User")]
        public async Task<HttpResponseMessage> Get(Guid id)
        {
            try
            {
                EventView eventView = await _eventService.GetEventAsync(id);
                if (eventView == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Requested event not found!");
                }
                EventRest eventRest = MapToRest(eventView);
                return Request.CreateResponse(HttpStatusCode.OK, eventRest);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Super_admin,Organizer")]
        public async Task<HttpResponseMessage> PostAsync([FromBody] EventModel eventModel)
        {
            try
            {
                EventModel postedEvent = await _eventService.PostEventAsync(eventModel);
                if (postedEvent!=null)
                {
                    return Request.CreateResponse(HttpStatusCode.Created, MapToEventModelRest(postedEvent));
                }
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Event creation failed!");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
        [HttpPut]
        [Authorize(Roles = "Super_admin,Organizer")]
        public async Task<HttpResponseMessage> PutAsync(Guid id, [FromBody] EventModel eventModel)
        {
            try
            {
                EventModel updatedEvent = await _eventService.UpdateEventAsync(id, eventModel);
                if (updatedEvent!=null)
                {
                    return Request.CreateResponse(HttpStatusCode.Created, MapToEventModelRest(updatedEvent));
                }
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Event update failed!");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
        [HttpDelete]
        [Authorize(Roles = "Super_admin,Organizer")]
        public async Task<HttpResponseMessage> DeleteAsync(Guid id)
        {
            try
            {
                bool deleteStatus = await _eventService.DeleteEventAsync(id);
                if (deleteStatus)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Event deleted");
                }
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
        private EventModelRest MapToEventModelRest(EventModel eventModel)
        {
            EventModelRest eventRest = new EventModelRest(eventModel.Id,eventModel.Name,eventModel.Description,eventModel.StartDate,eventModel.EndDate,eventModel.LocationId,eventModel.SportId);
            return eventRest;
        }

        private EventRest MapToRest(EventView eventView)
        {
            EventRest eventRest = new EventRest(eventView.Id, eventView.Name, eventView.Description, eventView.StartDate, eventView.EndDate, eventView.LocationId, eventView.SportId, eventView.VenueName, eventView.CityName, eventView.CountyName, eventView.SportName, eventView.SportType, eventView.Attendance, eventView.Rating,eventView.CreatedByUserId);
            List <SponsorRest> sponsors = new List<SponsorRest>();
            List<PlacementRest> placements = new List<PlacementRest>();
            foreach(IPlacement placement in eventView.Placements)
            {
                placements.Add(new PlacementRest(placement.Id, placement.Name, placement.FinishOrder, placement.EventId));
            }
            foreach(ISponsor sponsor in eventView.Sponsors)
            {
                sponsors.Add(new SponsorRest(sponsor.Id, sponsor.Name, sponsor.Website));
            }
            eventRest.Sponsors= sponsors;
            eventRest.Placements= placements;
            return eventRest;
        }
        private List<EventRest> MapEventsToRest(PagedList<EventView> events)
        {
            List<EventRest> eventsRest = new List<EventRest>();

            foreach(EventView eventView in events.Data)
            {
                eventsRest.Add(MapToRest(eventView));
            }
            return eventsRest;
        }
        private List<EventRest> MapEventsViewToRest(List<EventView> events)
        {
            List<EventRest> eventsRest = new List<EventRest>();

            foreach (EventView eventView in events)
            {
                eventsRest.Add(MapToRest(eventView));
            }
            return eventsRest;
        }
    }
}
