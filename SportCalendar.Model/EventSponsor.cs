using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportCalendar.Model
{
    public class EventSponsor
    {
        public Guid Id { get; set; }

        public Guid EventId { get; set; }

        public Guid SponsorId { get; set; }

        public bool IsAcive { get; set; }

        public Guid UpdatedByUserId { get; set; }

        public Guid CreatedByUserId { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }
    }
}
