using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportCalendar.ModelCommon
{
    public interface IRole
    {
        Guid Id { get; set; }
        string Access { get; set; }
        bool IsActive {  get; set; }
        Guid CreatedByUser {  get; set; }
        Guid UpdateByUser { get; set; }
        DateTime DateCreated { get; set; }
        DateTime DateUpdated { get; set; }
    }
}
