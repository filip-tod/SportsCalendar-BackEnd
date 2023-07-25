using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using SportCalendar.Model;
using SportCalendar.Service;
using SportCalendar.ServiceCommon;
using SportCalendar.WebApi.Models;

namespace SportCalendar.WebApi.Controllers
{
    // GET api/Sponsor
    public class SponsorController : ApiController
    {
        private ISponsorService sponsorService;

        public SponsorController(ISponsorService service)
        {
            sponsorService = service;
        }

        [HttpGet]
        [AllowAnonymous]
        [Authorize(Roles = "Super_admin,Organizer,User")]
        public async Task<HttpResponseMessage> SponsorGet()
        {
            List<Sponsor> sponsors = new List<Sponsor>();
            sponsors = await sponsorService.SponsorGet();
            if (sponsors != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, MapToRest(sponsors));
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest,"No rows found");
        }

        [HttpPost]
        [Authorize(Roles = "Super_admin,Organizer")]
        public async Task<HttpResponseMessage> SponsorPost([FromBody]Sponsor sponsor)
        {
            var resaut = await sponsorService.SponsorPost(sponsor);
            if (resaut == true)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "Sponsor inserted");
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, "Rown not inserted");
        }

        [HttpDelete]
        [Authorize(Roles = "Super_admin")]
        public async Task<HttpResponseMessage> SponsorDelte(Guid id)
        {
            var resault = await sponsorService.SponsorDelte(id);
            if(resault == true)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "Sponsor delted");
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, "Error, sponsor row not deleted");
        }

        [HttpPut]
        [Authorize(Roles = "Super_admin,Organizer")]
        public async Task<HttpResponseMessage> SponsorPut(Guid id, [FromBody] Sponsor sponsor)
        {
            var resault = await sponsorService.SponsorPut(id, sponsor);
            if (resault == true)
            {
                return Request.CreateResponse(HttpStatusCode.OK, "Sponsor updated");
            }
            return Request.CreateResponse(HttpStatusCode.BadRequest, "Error, sponsor not updated");
        }

        private SponsorRest MapFromRest(Sponsor sponsor)
        {
            SponsorRest sponsorRest = new SponsorRest()
            {
                Id = sponsor.Id,
                Name = sponsor.Name
            };
            return sponsorRest;

        }

        private List<SponsorRest> MapToRest(List<Sponsor> sponsors)
        {
            List<SponsorRest> sponsorList = new List<SponsorRest>();
            if (sponsors != null)
            {
                foreach (Sponsor sponsor in sponsors)
                {
                    SponsorRest sponsorRest = new SponsorRest();
                    sponsorRest.Id = sponsor.Id;
                    sponsorRest.Name = sponsor.Name;
                    sponsorRest.Website = sponsor.Website;
                    sponsorList.Add(sponsorRest);
                }
            }
            return sponsorList;
        }

    }
}
