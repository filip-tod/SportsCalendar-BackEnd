using SportCalendar.Common;
using SportCalendar.Model;
using SportCalendar.ModelCommon;
using SportCalendar.RepositoryCommon;
using SportCalendar.ServiceCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace SportCalendar.Service
{
    public class CountyService : ICountyService
    {
        private ICountyRepository CountyRepository;

        public CountyService(ICountyRepository repository)
        {
            CountyRepository = repository;
        }

        public async Task<List<County>> GetAll(Paging paging)
        {
            try
            {
                var result = await CountyRepository.GetAll(paging);
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<List<County>> GetById(Guid id)
        {
            try
            {
                var result = await CountyRepository.GetById(id);
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<County> Post(County county)
        {
            try
            {
                var result = await CountyRepository.Post(county);
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<County> Put(Guid id, County county)
        {
            try
            {
                var result = await CountyRepository.Put(id, county);
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
                var result = await CountyRepository.Delete(id);
                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
