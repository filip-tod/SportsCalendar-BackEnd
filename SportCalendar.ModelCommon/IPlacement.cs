using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportCalendar.ModelCommon
{
    public interface IPlacement
    {
        Guid? Id { get; set; }
        string Name { get; set; }
        int? FinishOrder { get; set; }
        Guid? EventId { get; set; }
        bool? IsActive { get; set; }
        Guid? CreatedByUserId { get; set; }
        Guid? UpdatedByUserId { get; set; }
        DateTime? DateCreated { get; set; }
        DateTime? DateUpdated { get; set; }
    }
}
