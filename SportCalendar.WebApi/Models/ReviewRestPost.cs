using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SportCalendar.WebApi.Models
{
    public class ReviewRestPost
    {
        public Guid? Id { get; set; }
        public string Content { get; set; }
        public int? Rating { get; set; }
        public bool? Attended { get; set; }
        public string UserName { get; set; }
        public Guid EventId { get; set; }
    }
}