using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportCalendar.ServiceCommon;
using SportCalendar.Repository;
using SportCalendar.Common;
using SportCalendar.Model;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using SportCalendar.RepositoryCommon;

namespace SportCalendar.Service
{
    public class SponsorService : ISponsorService
    {
        private ISponsorRepository sponsorRepository;

        public SponsorService(ISponsorRepository repository)
        {
            sponsorRepository = repository;
        }

        public async Task<List<Sponsor>> SponsorGet()
        {
            List<Sponsor> sponsors = await sponsorRepository.SponsorGet();
            return sponsors;
        }

        public async Task<bool> SponsorPost(Sponsor sponsor)
        {
            sponsor.Id = Guid.NewGuid();
            sponsor.IsActive = true;
            sponsor.UpdatedByUserId = Guid.Parse("0d3fa5c2-684c-4d88-82fd-cea2197c6e86");
            sponsor.CreatedByUserId = Guid.Parse("0d3fa5c2-684c-4d88-82fd-cea2197c6e86");
            sponsor.DateUpdated = DateTime.Now;
            sponsor.DateCreated = DateTime.Now;
            var resault = await sponsorRepository.SponsorPost(sponsor);
            if (resault == true)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> SponsorDelte(Guid id)
        {
            return (await sponsorRepository.SponsorDelte(id));
        }

        public async Task<bool> SponsorPut(Guid id, Sponsor sponsor)
        {
            sponsor.IsActive = true;
            sponsor.UpdatedByUserId = Guid.Parse("0d3fa5c2-684c-4d88-82fd-cea2197c6e86");
            sponsor.DateUpdated= DateTime.Now;
            return (await sponsorRepository.SponsorPut(id, sponsor));
        }
    }
}
