using SportCalendar.ModelCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportCalendar.Model
{
    public class Placement : IPlacement
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public int? FinishOrder { get; set; }
        public Guid? EventId { get; set; }
        public bool? IsActive { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public Guid? UpdatedByUserId { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
    }
}
