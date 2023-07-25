using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportCalendar.Common;
using SportCalendar.Model;
using SportCalendar.RepositoryCommon;
using SportCalendar.ServiceCommon;

namespace SportCalendar.Service
{
    public class PlacementService : IPlacementService
    {
        private IPlacementRepository placementRepository;

        public PlacementService(IPlacementRepository repository)
        {
            placementRepository = repository;
        }

        public async Task<PagedList<Placement>> PlacementGetFilteredAsync(Paging paging, Sorting sorting, PlacementFilter placementFilter)
        {
            return (await placementRepository.PlacementGetFilteredAync(paging, sorting, placementFilter));
        } 

        public async Task<bool> PlacementPostAsync(Placement placement)
        {
            placement.Id = Guid.NewGuid();
            placement.IsActive = true;
            placement.EventId = Guid.Parse("bbb75a72-228e-4dfa-8e58-2beed90b0c92");
            placement.CreatedByUserId = Guid.Parse("0d3fa5c2-684c-4d88-82fd-cea2197c6e86");
            placement.UpdatedByUserId = Guid.Parse("0d3fa5c2-684c-4d88-82fd-cea2197c6e86");
            placement.DateCreated = DateTime.UtcNow; 
            placement.DateUpdated = DateTime.UtcNow;

            return (await placementRepository.PlacementPostAsync(placement));
        }

        public async Task<bool> PlacementDeleteAsync(Guid id)
        {
            Placement placement = new Placement();
            placement.UpdatedByUserId = Guid.Parse("0d3fa5c2-684c-4d88-82fd-cea2197c6e86");
            placement.DateUpdated= DateTime.UtcNow;
            return (await placementRepository.PlacementDeleteAsync(id, placement));
        }

        public async Task<bool> PlacementPutAsync(Guid id, Placement placement)
        {
            placement.UpdatedByUserId = Guid.Parse("0d3fa5c2-684c-4d88-82fd-cea2197c6e86");
            placement.DateUpdated = DateTime.UtcNow;
            return(await placementRepository.PlacementPutAsync(id, placement));
        }
    }
}
