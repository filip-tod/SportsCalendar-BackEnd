using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportCalendar.ModelCommon
{
    public interface ISponsor
    {
        Guid? Id { get; set; }

        string Name { get; set; }

        string Website { get; set; }

        bool? IsActive { get; set; }

        Guid? UpdatedByUserId { get; set; }

        Guid? CreatedByUserId { get; set; }

        DateTime? DateCreated { get; set; }

        DateTime? DateUpdated { get; set; }
    }
}
