using SportCalendar.Common;
using SportCalendar.Model;
using SportCalendar.ModelCommon;
using SportCalendar.Repository;
using SportCalendar.RepositoryCommon;
using SportCalendar.ServiceCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportCalendar.Service
{
    public class LocationService : ILocationService
    {
        private ILocationRepository LocationRepository;

        public LocationService(ILocationRepository repository)
        {
            LocationRepository = repository;
        }

        public async Task<List<Location>> GetAllREST(Paging paging, Sorting sorting)
        {
            try
            {
                var result = await LocationRepository.GetAllREST(paging, sorting);
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<Location> GetById(Guid id)
        {
            try
            {
                var result = await LocationRepository.GetById(id);
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Location> Create(Location location)
        {
            try
            {
                var result = await LocationRepository.Create(location);
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<Location> Put(Location location, Guid id) 
        {
            try
            {
                var result = await LocationRepository.Put(location, id);
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<bool> Delete(Guid id)
        {
            try
            {
                var result = await LocationRepository.Delete(id);
                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
