using SportCalendar.Model;
using SportCalendar.Service;
using SportCalendar.ServiceCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using SportCalendar.Common;
using System.Drawing.Printing;
using SportCalendar.ModelCommon;
using SportCalendar.WebApi.Models;

namespace SportCalendar.WebApi.Controllers
{
    public class CityController : ApiController
    {
        private ICityService CityService;
        public CityController(ICityService service)
        {
            CityService = service;
            
        }

        public async Task<HttpResponseMessage> GetAll(int pageNumber = 2, int pageSize = 10)
        {
            Paging paging = new Paging();
            paging.PageNumber = pageNumber;
            paging.PageSize = pageSize;

            List<City> result = await CityService.GetAll(paging);
            List<CityRest> restCity = result.Select(city => new CityRest
            {
                Id = city.Id,
                Name = city.Name,
                IsActive = city.IsActive

            }).ToList();

            if (result == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "We could not find your cities.");
            }
            return Request.CreateResponse(HttpStatusCode.OK, restCity);
        }


        public async Task<HttpResponseMessage> GetById(Guid id)
        {
            List<City> result = await CityService.GetById(id);
            if (result == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "We could not find you'r specific City");
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        public async Task<HttpResponseMessage> Post(City city)
        {
            var createdCounty = await CityService.Post(city);
            if (createdCounty == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "We could not insert you'r City, check body!");
            }
            return Request.CreateResponse(HttpStatusCode.OK, createdCounty);
        }

        public async Task<HttpResponseMessage> Put(Guid id, City updatedCity)
        {
            var CityCreated = await CityService.Put(id, updatedCity);
            if (CityCreated == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "We could not insert you'r new City, check body!");
            }
            return Request.CreateResponse(HttpStatusCode.OK, CityCreated);
        }
        public async Task<HttpResponseMessage>Delete(Guid id)
        {
            var cityDelete = await CityService.Delete(id);
            if (cityDelete == false)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "We could not Delete you'r City");
            }
            return Request.CreateResponse(HttpStatusCode.OK, "we Deleted you'r City");
        }
    }
}