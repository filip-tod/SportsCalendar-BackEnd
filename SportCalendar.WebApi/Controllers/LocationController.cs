using SportCalendar.Model;
using SportCalendar.ServiceCommon;
using SportCalendar.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Web.Http;
using SportCalendar.Service;
using SportCalendar.Common;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace SportCalendar.WebApi.Controllers
{
    public class LocationController : ApiController
    {
        private ILocationService LocationService;
        public LocationController(ILocationService service)
        {
            LocationService = service;

        }
        //Rest Works
        public async Task<HttpResponseMessage> GetAllREST(int pageNumber = 1, int pageSize = 10, string sortOrder = "ASC", string orderBy = "Venue")
        {
            Paging paging = new Paging
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            Sorting sorting = new Sorting
            {
                SortOrder = sortOrder,
                OrderBy = orderBy
            };

            try
            {
                List<Location> locations = await LocationService.GetAllREST(paging, sorting);
                List<RESTLocation> restLocations = locations.Select(location => new RESTLocation
                {
                    Id = location.Id,
                    IsActive = location.IsActive,
                    Venue = location.Venue,
                    CountyName = location.CountyName,
                    CityName = location.CityName
                }).ToList();

                if (restLocations.Count > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, restLocations);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "No locations found.");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        public async Task<HttpResponseMessage> GetById(Guid id)
        {
            try
            {
                Location location = await LocationService.GetById(id);

                if (location != null)
                {
                    RESTLocation restLocation = new RESTLocation
                    {
                        Id = location.Id,
                        IsActive = location.IsActive,
                        Venue = location.Venue,
                        CityName = location.CityName,
                        CountyName = location.CountyName
                    };

                    return Request.CreateResponse(HttpStatusCode.OK, restLocation);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Location not found.");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        public async Task<HttpResponseMessage> Create(Location location)
        {
            try
            {
                var createdLocation = await LocationService.Create(location);
                return Request.CreateResponse(HttpStatusCode.OK, createdLocation);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "can't add a new location.");
            }
        }
        public async Task<HttpResponseMessage> Put(Location location, Guid id)
        {
            try
            {
                var update = await LocationService.Put(location , id);
                return Request.CreateResponse(HttpStatusCode.OK, update);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "can't update you'r Location");
            }
        }

        public async Task<HttpResponseMessage> Delete(Guid id)
        {
            var locationdelete = await LocationService.Delete(id);
            if (locationdelete == false)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "We could not Delete you'r Location");
            }
            return Request.CreateResponse(HttpStatusCode.OK, "we Deleted you'r Location with the ID:");
        }

    }
}