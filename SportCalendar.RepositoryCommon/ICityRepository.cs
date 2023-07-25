using SportCalendar.Common;
using SportCalendar.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportCalendar.RepositoryCommon
{
    public interface ICityRepository
    {
        Task<List<City>> GetAll(Paging paging);
        Task<List<City>> GetById(Guid id);
        Task<City> Post(City city);
        Task<City> Put(Guid id, City updatedCity);
        Task<bool> Delete(Guid id);
    }
}
