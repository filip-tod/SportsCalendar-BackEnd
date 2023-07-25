using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SportCalendar.WebApi.Models
{
    public class CityRest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public bool IsActive { get; set; }
        
    }
}