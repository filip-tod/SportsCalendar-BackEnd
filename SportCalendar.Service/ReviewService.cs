using SportCalendar.Common;
using SportCalendar.Model;
using SportCalendar.RepositoryCommon;
using SportCalendar.ServiceCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportCalendar.Service
{
    public class ReviewService : IReviewService
    {
        private IReviewRepository _reviewRepository;
        public ReviewService(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public Task<bool> DeleteReviewAsync(Guid id)
        {
            return _reviewRepository.DeleteReviewAsync(id);
        }

        public async Task<Review> GetReviewAsync(Guid id)
        {
            return await _reviewRepository.GetReviewAsync(id);
        }

        public async Task<PagedList<Review>> GetReviewsAsync(Sorting sorting, Paging paging,ReviewFilter filtering)
        {
            return await _reviewRepository.GetReviewsAsync(sorting, paging, filtering);
        }

        public async Task<Review> PostReviewAsync(Review review)
        {
            Guid userId = Guid.Parse("0d3fa5c2-684c-4d88-82fd-cea2197c6e86");
            review.CreatedByUserId = userId;
            review.UpdatedByUserId = userId;
            review.DateUpdated = DateTime.UtcNow;
            review.DateCreated = DateTime.UtcNow;
            review.IsActive = true;
            //review.EventId = Guid.Parse("5b452225-8a90-4345-8868-65e794ff0577");
            return await _reviewRepository.PostReviewAsync(review);
        }

        public async Task<Review> UpdateReviewAsync(Guid id, Review review)
        {
            Guid userId = Guid.Parse("0d3fa5c2-684c-4d88-82fd-cea2197c6e86");
            review.UpdatedByUserId = userId;
            review.DateUpdated = DateTime.UtcNow;
            return await _reviewRepository.UpdateReviewAsync(id, review);
        }
    }
}
