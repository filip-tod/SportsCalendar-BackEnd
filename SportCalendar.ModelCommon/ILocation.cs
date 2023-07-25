using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportCalendar.ModelCommon
{
    public interface ILocation
    {
         Guid Id { get; set; }
         string Venue { get; set; }
         bool IsActive { get; set; }
         Guid UpdatedByUserId { get; set; }
         Guid CreatedByUserId { get; set; }
         DateTime DateCreated { get; set; }
         DateTime DateUpdated { get; set; }

         Guid CountyId { get; set; }

         Guid CityId { get; set; }
    }
}
