using SportCalendar.ModelCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportCalendar.Model
{
    public class Review : IReview
    {
        public Guid? Id { get; set; }
        public string Content { get; set; }
        public int? Rating { get; set; }
        public bool? Attended { get; set; }
        public string EventName { get; set; }
        public string UserName { get; set; }
        public Guid EventId { get; set; }
        public bool? IsActive { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public Guid? UpdatedByUserId { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
    }
}
