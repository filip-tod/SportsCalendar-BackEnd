using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SportCalendar.WebApi.Models
{
    public class SportRest
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }
}