using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SportCalendar.WebApi.Models
{
    public class RESTLocation
    {
        public Guid Id { get; set; }
        public bool IsActive { get; set; }
        public string Venue { get; set; }
        public string CityName { get; set; }
        public string CountyName { get; set; }
    }
}