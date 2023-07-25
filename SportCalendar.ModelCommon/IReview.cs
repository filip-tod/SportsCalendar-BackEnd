using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportCalendar.ModelCommon
{
    public interface IReview
    {
        Guid? Id { get; set; }
        string Content { get; set; }
        int? Rating { get; set; }
        bool? Attended { get; set; }
        string EventName { get; set; }
        string UserName { get; set; }
        Guid EventId { get; set; }
        bool? IsActive { get; set; }
        Guid? CreatedByUserId { get; set; }
        Guid? UpdatedByUserId { get; set; }
        DateTime? DateCreated { get; set; }
        DateTime? DateUpdated { get; set; }
    }
}
