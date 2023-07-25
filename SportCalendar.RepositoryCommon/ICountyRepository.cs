using SportCalendar.Common;
using SportCalendar.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportCalendar.RepositoryCommon
{
    public interface ICountyRepository
    {
        Task<List<County>> GetAll(Paging paging);
        Task<List<County>> GetById(Guid id);
        Task<County> Post(County county);
        Task<County> Put(Guid id, County county);
        Task<bool> Delete(Guid id);
    }
}
