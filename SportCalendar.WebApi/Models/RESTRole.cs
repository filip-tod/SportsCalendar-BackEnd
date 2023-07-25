using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SportCalendar.WebApi.Models
{
    public class RESTRole
    {
        public Guid Id { get; set; }
        public string Access { get; set; }
        public bool IsActive { get; set; }
    }
}