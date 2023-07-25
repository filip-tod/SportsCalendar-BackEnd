using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SportCalendar.WebApi.Models
{
    public class SponsorRest
    {
        public SponsorRest(Guid? id, string name, string website)
        {
            Id = id;
            Name = name;
            Website = website;
        }
        public SponsorRest()
        {

        }
        public Guid? Id { get; set; }

        public string Name { get; set; }

        public string Website { get; set; }
    }
}