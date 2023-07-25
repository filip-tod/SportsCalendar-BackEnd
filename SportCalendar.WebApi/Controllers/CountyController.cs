using SportCalendar.ServiceCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using SportCalendar.Model;
using SportCalendar.Common;
using System.Drawing.Printing;
using SportCalendar.WebApi.Models;
using SportCalendar.ModelCommon;

namespace SportCalendar.WebApi.Controllers
{
    [RoutePrefix("api/County")]
    public class CountyController : ApiController
    {
        private ICountyService CountyService;
        public CountyController(ICountyService service)
        {
            CountyService = service;
        }
        [HttpGet]
        [Route("")]
        public async Task<HttpResponseMessage> GetAll(int pageNumber = 1, int pageSize = 21)
        {
            Paging paging = new Paging();
            paging.PageNumber = pageNumber;
            paging.PageSize = pageSize;

            List<County> result = await CountyService.GetAll(paging);
            List<CountyRest> cuntyRest = result.Select(county => new CountyRest
            {

                Id = county.Id,
                Name = county.Name,
                IsActive = county.IsActive

            }).ToList();

            if (result == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "We could not find County");
            }
            return Request.CreateResponse(HttpStatusCode.OK, cuntyRest);
        }


        public async Task<HttpResponseMessage> GetById(Guid id)
        {
            List<County> result = await CountyService.GetById(id);
            if (result == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "We could not find County by Id");
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        public async Task<HttpResponseMessage> Post(County county)
        {
            var createdCounty = await CountyService.Post(county);
            if (createdCounty == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "We could not insert you'r County, check body!");
            }
            return Request.CreateResponse(HttpStatusCode.OK, createdCounty);
        }

        public async Task<HttpResponseMessage> Put(Guid id, County county)
        {
            var createdCounty = await CountyService.Put(id, county);
            if (createdCounty == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "We could not update you'r County, check body!");
            }
            return Request.CreateResponse(HttpStatusCode.OK,"We Updated you'r County with the ID:"+createdCounty.Id);
        }

        public async Task<HttpResponseMessage> Delete(Guid id)
        {
            var cuntyDeleted = await CountyService.Delete(id);
            if (cuntyDeleted == false)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "We could not Delete you'r County");
            }
            return Request.CreateResponse(HttpStatusCode.OK,"we Deleted you'r county with the ID:" );
        }

    }
}