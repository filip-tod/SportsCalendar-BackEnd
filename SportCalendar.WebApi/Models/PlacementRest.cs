using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SportCalendar.WebApi.Models
{
    public class PlacementRest
    {
        public PlacementRest(Guid? id, string name, int? finishOrder, Guid? eventId)
        {
            Id = id;
            Name = name;
            FinishOrder = finishOrder;
            EventId = eventId;
        }
        public PlacementRest()
        {

        }

        public Guid? Id { get; set; }
        public string Name { get; set; }
        public int? FinishOrder { get; set; }
        public Guid? EventId { get; set; }
    }
}