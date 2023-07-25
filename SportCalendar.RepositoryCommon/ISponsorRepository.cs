using SportCalendar.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportCalendar.RepositoryCommon
{
    public interface ISponsorRepository
    {
        Task<List<Sponsor>> SponsorGet();

        Task<bool> SponsorPost(Sponsor sponsor);

        Task<bool> SponsorDelte(Guid id);

        Task<bool> SponsorPut(Guid id, Sponsor sponsor);
    }
}
