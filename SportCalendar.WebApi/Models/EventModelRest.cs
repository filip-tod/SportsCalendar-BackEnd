using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SportCalendar.WebApi.Models
{
    public class EventModelRest
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid? LocationId { get; set; }
        public Guid? SportId { get; set; }
        public EventModelRest()
        {

        }

        public EventModelRest(Guid? id, string name, string description, DateTime? startDate, DateTime? endDate, Guid? locationId, Guid? sportId)
        {
            Id = id;
            Name = name;
            Description = description;
            StartDate = startDate;
            EndDate = endDate;
            LocationId = locationId;
            SportId = sportId;
        }
    }
}