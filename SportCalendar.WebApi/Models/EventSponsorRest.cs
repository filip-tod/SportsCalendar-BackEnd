using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SportCalendar.WebApi.Models
{
    public class EventSponsorRest
    {
        public Guid Id { get; set; }

        public Guid EventId { get; set; }

        public Guid SponsorId { get; set; }
    }
}