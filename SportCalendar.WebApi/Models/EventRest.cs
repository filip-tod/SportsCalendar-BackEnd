using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SportCalendar.WebApi.Models
{
    public class EventRest
    {
        public EventRest(Guid? id, string name, string description, DateTime? startDate, DateTime? endDate, Guid? locationId, Guid? sportId, string venueName, string cityName, string countyName, string sportName, string sportType, int? attendance, decimal? rating,Guid? createdByUserId)
        {
            Id = id;
            Name = name;
            Description = description;
            StartDate = startDate;
            EndDate = endDate;
            LocationId = locationId;
            SportId = sportId;
            VenueName = venueName;
            CityName = cityName;
            CountyName = countyName;
            SportName = sportName;
            SportType = sportType;
            Attendance = attendance;
            Rating = rating;
            CreatedByUserId = createdByUserId;
        }
        public EventRest()
        {

        }

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
        public List<SponsorRest> Sponsors { get; set; }
        public List<PlacementRest> Placements { get; set; }
        public Guid? CreatedByUserId    { get; set; }
    }
}