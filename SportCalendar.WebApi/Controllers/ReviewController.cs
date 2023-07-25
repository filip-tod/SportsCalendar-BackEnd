using SportCalendar.Common;
using SportCalendar.Model;
using SportCalendar.ServiceCommon;
using SportCalendar.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace SportCalendar.WebApi.Controllers
{
    public class ReviewController : ApiController
    {
        private IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet]
        [AllowAnonymous]
        [Authorize(Roles = "Super_admin,Organizer,User")]
        public async Task<HttpResponseMessage> Get(string orderBy = "Rating", string sortOrder = "DESC", int pageSize = 10, int pageNumber = 1, Guid? userId = null, Guid? eventId = null)
        {
            try
            {
                Sorting sorting = new Sorting();
                sorting.SortOrder = sortOrder;
                sorting.OrderBy = orderBy;

                Paging paging = new Paging();
                paging.PageNumber = pageNumber;
                paging.PageSize = pageSize;

                ReviewFilter filter = new ReviewFilter();
                filter.EventId = eventId;
                filter.UserId = userId;

                PagedList<Review> pagedList = await _reviewService.GetReviewsAsync(sorting, paging, filter);

                if (pagedList != null)
                {
                    if (pagedList.Data.Any())
                    {
                        List<ReviewRest> reviewsRest = MapReviewsToRest(pagedList);
                        PagedList<ReviewRest> pagedListRest = new PagedList<ReviewRest>();
                        pagedListRest.Data = reviewsRest;
                        pagedListRest.TotalCount = pagedList.TotalCount;
                        pagedListRest.TotalPages = pagedList.TotalPages;
                        pagedListRest.PageSize = pagedList.PageSize;
                        pagedListRest.CurrentPage = pagedList.CurrentPage;

                        return Request.CreateResponse(HttpStatusCode.OK, pagedListRest);
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.NoContent);
                    }
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Super_admin,Organizer,User")]
        public async Task<HttpResponseMessage> GetAsync(Guid id)
        {
            try
            {
                Review review = await _reviewService.GetReviewAsync(id);
                if (review == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Requested review not found!");
                }
                return Request.CreateResponse(HttpStatusCode.OK, MapToRest(review));
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Super_admin,Organizer,User")]
        public async Task<HttpResponseMessage> PostAsync([FromBody] Review review)
        {
            try
            {
                Review postedReview = await _reviewService.PostReviewAsync(review);
                if (postedReview!=null)
                {
                    return Request.CreateResponse(HttpStatusCode.Created, MapToRestPosted(postedReview));
                }
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Review creation failed!");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
        [HttpPut]
        [Authorize(Roles = "Super_admin,Organizer,User")]
        public async Task<HttpResponseMessage> PutAsync(Guid id, [FromBody] Review review)
        {
            try
            {
                Review updatedReview = await _reviewService.UpdateReviewAsync(id, review);
                if (updatedReview!=null)
                {
                    return Request.CreateResponse(HttpStatusCode.Created, MapToRestPosted(updatedReview));
                }
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "review update failed!");
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
        [HttpDelete]
        [Authorize(Roles = "Super_admin,Organizer,User")]
        public async Task<HttpResponseMessage> DeleteAsync(Guid id)
        {
            try
            {
                bool deleteStatus = await _reviewService.DeleteReviewAsync(id);
                if (deleteStatus)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, "Review deleted");
                }
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }
        private ReviewRest MapToRest(Review review)
        {
            ReviewRest reviewRest = new ReviewRest();

            reviewRest.Id = review.Id;
            reviewRest.Rating = review.Rating;
            reviewRest.Content = review.Content;
            reviewRest.Attended = review.Attended;
            reviewRest.EventName = review.EventName;
            reviewRest.EventId = review.EventId;
            reviewRest.UserName = review.UserName;

            return reviewRest;
        }
        private ReviewRestPost MapToRestPosted(Review review)
        {
            ReviewRestPost reviewRest = new ReviewRestPost();

            reviewRest.Id = review.Id;
            reviewRest.Rating = review.Rating;
            reviewRest.Content = review.Content;
            reviewRest.Attended = review.Attended;
            reviewRest.EventId = review.EventId;
            reviewRest.UserName = review.UserName;

            return reviewRest;
        }

        private List<ReviewRest> MapReviewsToRest(PagedList<Review> reviews)
        {
            List<ReviewRest> reviewsRest = new List<ReviewRest>();

            foreach (Review review in reviews.Data)
            {
                reviewsRest.Add(MapToRest(review));
            }
            return reviewsRest;
        }
    }
}

