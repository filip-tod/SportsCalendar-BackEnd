using SportCalendar.Common;
using SportCalendar.Model;
using SportCalendar.RepositoryCommon;
using SportCalendar.ServiceCommon;
using System;
using System.Threading.Tasks;

namespace SportCalendar.Service
{
    public class SportService:ISportService
    {
        private ISportRepository _sportRepository;
        public SportService(ISportRepository sportRepository)
        {
            _sportRepository = sportRepository;
        }

        public async Task<bool> DeleteSportAsync(Guid id)
        {
            return await _sportRepository.DeleteSportAsync(id);
        }

        public async Task<PagedList<Sport>> GetSportsAsync(Sorting sorting, Paging paging, SportFilter filtering)
        {
            return await _sportRepository.GetSportsAsync(sorting, paging, filtering);
        }
        public async Task<Sport> GetSportAsync(Guid id)
        {
            return await _sportRepository.GetSportAsync(id);
        }

        public Task<Sport> PostSportAsync(Sport sport)
        {
            Guid userId = Guid.Parse("0d3fa5c2-684c-4d88-82fd-cea2197c6e86");
            sport.CreatedByUserId = userId;
            sport.UpdatedByUserId = userId;
            sport.DateUpdated = DateTime.Now;
            sport.DateCreated = DateTime.Now;
            sport.IsActive = true;
            return _sportRepository.PostSportAsync(sport);
        }

        public async Task<Sport> UpdateSportAsync(Guid id, Sport sport)
        {
            Guid userId = Guid.Parse("0d3fa5c2-684c-4d88-82fd-cea2197c6e86");
            sport.UpdatedByUserId = userId;
            sport.DateUpdated = DateTime.Now;
            return await _sportRepository.UpdateSportAsync(id, sport);
        }
        //before calling repository function set sport attributes like userId and dates and then send the sport object to repository function
        //superAdmin 0d3fa5c2-684c-4d88-82fd-cea2197c6e86
    }
}
