using SportCalendar.ModelCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportCalendar.Model
{
    public class EventView:IEventView
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid? LocationId { get; set; }
        public Guid? SportId { get; set; }
        public string VenueName { get; set; }
        public string CityName { get; set; }
        public string CountyName { get; set; }
        public string SportName { get; set; }
        public string SportType { get; set; }
        public int? Attendance { get; set; }
        public decimal? Rating { get; set; }
        public List<ISponsor> Sponsors { get; set; }
        public List<IPlacement> Placements { get; set; }
        public bool? IsActive { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public Guid? UpdatedByUserId { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
    }
}
