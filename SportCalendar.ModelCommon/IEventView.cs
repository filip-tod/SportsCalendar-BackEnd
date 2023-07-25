using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportCalendar.ModelCommon
{
    public interface IEventView
    {

        Guid? Id { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        DateTime? StartDate { get; set; }
        DateTime? EndDate { get; set; }
        Guid? LocationId { get; set; }
        Guid? SportId { get; set; }
        string VenueName { get; set; }
        string CityName { get; set; }
        string CountyName { get; set; }
        string SportName { get; set; }
        string SportType { get; set; }
        int? Attendance { get; set; }
        decimal? Rating { get; set; }
        List<ISponsor> Sponsors { get; set; }
        List<IPlacement> Placements { get; set; }
        bool? IsActive { get; set; }
        Guid? CreatedByUserId { get; set; }
        Guid? UpdatedByUserId { get; set; }
        DateTime? DateCreated { get; set; }
        DateTime? DateUpdated { get; set; }
    }
}
