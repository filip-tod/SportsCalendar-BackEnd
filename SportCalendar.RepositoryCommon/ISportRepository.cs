using SportCalendar.Common;
using SportCalendar.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportCalendar.RepositoryCommon
{
    public interface ISportRepository
    {
        Task<PagedList<Sport>> GetSportsAsync(Sorting sorting, Paging paging, SportFilter filtering);
        Task<Sport> GetSportAsync(Guid id);
        Task<bool> DeleteSportAsync(Guid id);
        Task<Sport> PostSportAsync(Sport sport);
        Task<Sport> UpdateSportAsync(Guid id, Sport sport);
        //Task<bool> SetUpdatedAsync(Guid userId);
        //Task<bool> CreatedAsync(Guid userId);


    }
}
