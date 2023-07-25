using Microsoft.VisualStudio.Services.WebApi;
using SportCalendar.ServiceCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using SportCalendar.Model;
using SportCalendar.Common;
using Microsoft.Owin.Security.Provider;
using SportCalendar.WebApi.Models;


namespace SportCalendar.WebApi.Controllers
{
    public class EventSponsorController : ApiController
    {
        private IEventSponsorService eventSponsorService;

        public EventSponsorController(IEventSponsorService service)
        {
            eventSponsorService = service;
        }

        [HttpGet]
        [AllowAnonymous]
        [Authorize(Roles ="Super_admin,Organizer,User")]
        public async Task<HttpResponseMessage> EventSponsorGetAsync(Guid eventId)
        {
            try
            {
                List<EventSponsor> eventSponsors = await eventSponsorService.EventSponsorGetAsync(eventId);

                if(eventSponsors.Any())
                {
                    List<EventSponsorRest> eventSponsorRests = MapEventSponsorsToRest(eventSponsors);
                    return Request.CreateResponse(HttpStatusCode.OK, eventSponsorRests);
                }
                return Request.CreateResponse(HttpStatusCode.NotFound, "No rows found in data base");
            }
            catch (Exception ex)
            {

                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpPost]
        [Authorize(Roles ="Super_admin,Organizer")]
        public async Task<HttpResponseMessage> EventSponsorPostAsync(EventSponsor eventSponsor)
        {
            try
            {
                var resault = await eventSponsorService.EventSponsorPostAsync(eventSponsor);
                if (resault == true)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "EventSponsor row inserted");
                }
                return Request.CreateErrorResponse(HttpStatusCode.NotModified, "EventSponsor row not inserted");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpDelete]
        [Authorize(Roles ="Super_admin,Organizer")]
        public async Task<HttpResponseMessage> EventSponsorDeleteAsync(Guid id)
        {
            try
            {
                var resault = await eventSponsorService.EventSponsorDeleteAsync(id);
                if(resault == true)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "EventSponsor row deleted");
                }
                return Request.CreateResponse(HttpStatusCode.NotFound, "EventSponsor row not deleted");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        [HttpPut]
        [Authorize(Roles ="Super_admin,Organizer")]
        public async Task<HttpResponseMessage> EventSponsorPutAsync(Guid id, EventSponsor eventSponsor)
        {
            try
            {
                var resault = await eventSponsorService.EventSponsorPutAsync(id, eventSponsor);
                if(resault == true)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "EventSponsor row updated");
                }
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "EventSponsor row not updated");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        private EventSponsorRest MapToRest(EventSponsor eventSponsor)
        {
            EventSponsorRest eventSponsorRest = new EventSponsorRest();
            eventSponsorRest.Id= eventSponsor.Id;
            eventSponsorRest.EventId = eventSponsor.EventId;
            eventSponsorRest.SponsorId = eventSponsor.SponsorId;

            return eventSponsorRest;
        }

        private List<EventSponsorRest>MapEventSponsorsToRest(List<EventSponsor> eventSponsors)
        {
            List<EventSponsorRest> eventSponsorRests = new List<EventSponsorRest>();

            foreach (EventSponsor eventSponsor in eventSponsors)
            {
                eventSponsorRests.Add(MapToRest(eventSponsor));
            }
            return eventSponsorRests;
        }
    }
}
