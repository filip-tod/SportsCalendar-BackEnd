using Npgsql;
using SportCalendar.Common;
using SportCalendar.Model;
using SportCalendar.RepositoryCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportCalendar.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        private string connectionString = Environment.GetEnvironmentVariable("ConnectionString");

        public async Task<PagedList<Review>> GetReviewsAsync(Sorting sorting, Paging paging, ReviewFilter filtering)
        {
            List<Review> reviews = new List<Review>();
            int totalReviews = 0;
            try
            {
                NpgsqlConnection connection = new NpgsqlConnection(connectionString);
                using (connection)
                {
                    StringBuilder queryBuilder = new StringBuilder();
                    NpgsqlCommand command = new NpgsqlCommand();
                    command.Connection = connection;

                    queryBuilder.Append("SELECT r.*, u.\"Username\" AS \"CreatedByUsername\", e.\"Name\" AS \"EventName\" ");
                    queryBuilder.Append("FROM \"Review\" r ");
                    queryBuilder.Append("JOIN \"User\" u ON r.\"CreatedByUserId\" = u.\"Id\" ");
                    queryBuilder.Append("JOIN \"Event\" e ON r.\"EventId\" = e.\"Id\" ");
                    queryBuilder.Append("Where r.\"IsActive\" = true ");

                    if (filtering.UserId != null)
                    {
                        queryBuilder.Append("AND r.\"CreatedByUserId\" = @UserId ");
                        command.Parameters.AddWithValue("@UserId", filtering.UserId);
                    }
                    if (filtering.EventId != null)
                    {
                        queryBuilder.Append("AND r.\"EventId\" = @EventId ");
                        command.Parameters.AddWithValue("@EventId", filtering.EventId);
                    }

                    if (sorting.OrderBy != null)
                    {
                        queryBuilder.Append("ORDER BY r.");
                        queryBuilder.Append("\"" + sorting.OrderBy + "\" ");
                        command.Parameters.AddWithValue("@OrderBy", sorting.OrderBy);
                    }
                    if (sorting.SortOrder != null)
                    {
                        queryBuilder.Append(sorting.SortOrder + " ");
                    }
                    queryBuilder.Append(" OFFSET @Offset LIMIT @Limit");

                    command.Parameters.AddWithValue("@Offset", paging.PageSize * (paging.PageNumber - 1));
                    command.Parameters.AddWithValue("@Limit", paging.PageSize);

                    command.CommandText = queryBuilder.ToString();
                    connection.CreateCommand();
                    await connection.OpenAsync();

                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();
                    if (!reader.HasRows)
                    {
                        return null;
                    }
                    while (reader.Read())
                    {
                        reviews.Add(MapReview(reader));
                    }
                    await reader.CloseAsync();

                    NpgsqlCommand commandTotal = new NpgsqlCommand();
                    StringBuilder queryTotal = new StringBuilder();
                    queryTotal.Append("SELECT COUNT(r.\"Id\") ");
                    queryTotal.Append("FROM \"Review\" r ");
                    queryTotal.Append("JOIN \"Event\" e ON r.\"EventId\" = e.\"Id\" ");
                    commandTotal.CommandText = queryTotal.ToString();
                    commandTotal.Connection = connection;
                    totalReviews = Convert.ToInt32(await commandTotal.ExecuteScalarAsync());

                    PagedList<Review> paginatedList = new PagedList<Review>();
                    paginatedList.TotalCount = totalReviews;
                    paginatedList.Data = reviews;
                    paginatedList.TotalPages = totalReviews / paging.PageSize;
                    paginatedList.PageSize = paging.PageSize;
                    paginatedList.CurrentPage = paging.PageNumber;
                    return paginatedList;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }


        public async Task<Review> GetReviewAsync(Guid id)
        {
            try
            {
                Review review = await GetReviewByIdAsync(id);
                if (review == null)
                {
                    return null;
                }
                return review;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<bool> DeleteReviewAsync(Guid id)
        {
            try
            {
                NpgsqlConnection connection = new NpgsqlConnection(connectionString);
                Review review = await GetReviewByIdAsync(id);

                Guid userId = Guid.Parse("0d3fa5c2-684c-4d88-82fd-cea2197c6e86");
                review.UpdatedByUserId = userId;
                review.DateUpdated = DateTime.Now;

                if (review == null)
                {
                    return false;
                }
                using (connection)
                {
                    review.IsActive = false;
                    StringBuilder queryBuilder = new StringBuilder();
                    NpgsqlCommand command = new NpgsqlCommand();
                    command.Connection = connection;

                    queryBuilder.Append("UPDATE \"Review\" SET \"IsActive\" = @IsActive,");
                    queryBuilder.Append(" \"UpdatedByUserId\" = @UpdatedByUserId,");
                    queryBuilder.Append(" \"DateUpdated\" = @DateUpdated");
                    queryBuilder.Append(" WHERE \"Id\" = @Id");

                    command.Parameters.AddWithValue("@IsActive", review.IsActive);
                    command.Parameters.AddWithValue("@UpdatedByUserId", review.UpdatedByUserId);
                    command.Parameters.AddWithValue("@DateUpdated", review.DateUpdated);
                    command.Parameters.AddWithValue("@Id", id);

                    command.CommandText = queryBuilder.ToString();
                    connection.CreateCommand();
                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();

                    return true;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<Review> PostReviewAsync(Review review)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (NpgsqlCommand command = new NpgsqlCommand())
                    {
                        command.CommandText = "INSERT INTO \"Review\" (\"Id\", \"Content\", \"Rating\", \"Attended\", \"EventId\", \"UserId\", \"IsActive\", \"CreatedByUserId\", \"UpdatedByUserId\", \"DateCreated\", \"DateUpdated\") " +
                      "VALUES (@Id, @Content, @Rating, @Attended, @EventId, @UserId, @IsActive, @CreatedByUserId, @UpdatedByUserId, @DateCreated, @DateUpdated) RETURNING * ";
                        command.Connection = connection;

                        Guid id = Guid.NewGuid();
                        review.Id = id;

                        command.Parameters.AddWithValue("@Id", review.Id);
                        command.Parameters.AddWithValue("@Content", review.Content);
                        command.Parameters.AddWithValue("@Rating", review.Rating);
                        command.Parameters.AddWithValue("@Attended", review.Attended);
                        command.Parameters.AddWithValue("@EventId", review.EventId);
                        command.Parameters.AddWithValue("@IsActive", review.IsActive);
                        command.Parameters.AddWithValue("@CreatedByUserId", review.CreatedByUserId);
                        command.Parameters.AddWithValue("@UserId", review.CreatedByUserId);
                        command.Parameters.AddWithValue("@UpdatedByUserId", review.UpdatedByUserId);
                        command.Parameters.AddWithValue("@DateCreated", review.DateCreated);
                        command.Parameters.AddWithValue("@DateUpdated", review.DateUpdated);

                        NpgsqlDataReader reader = await command.ExecuteReaderAsync();
                        if (reader.HasRows)
                        {
                            await reader.ReadAsync();
                            return MapReviewModel(reader);
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }


        public async Task<Review> UpdateReviewAsync(Guid id, Review review)
        {
            try
            {
                Review currentReview = await GetReviewByIdAsync(id);
                if (currentReview == null)
                {
                    return null;
                }
                NpgsqlConnection connection = new NpgsqlConnection(connectionString);
                using (connection)
                {

                    StringBuilder queryBuilder = new StringBuilder();
                    NpgsqlCommand command = new NpgsqlCommand();
                    queryBuilder.Append("UPDATE \"Review\" SET ");
                    command.Connection = connection;
                    await connection.OpenAsync();

                    if (review.Rating != null)
                    {
                        queryBuilder.Append("\"Rating\" = @Rating,");
                        command.Parameters.AddWithValue("@Rating", review.Rating);
                    }
                    if (review.Attended != null)
                    {
                        queryBuilder.Append("\"Attended\" = @Attended,");
                        command.Parameters.AddWithValue("@Attended", review.Attended);
                    }
                    if (!string.IsNullOrEmpty(review.Content))
                    {
                        queryBuilder.Append("\"Content\" = @Content,");
                        command.Parameters.AddWithValue("@Content", review.Content);
                    }
                    if (review.IsActive.HasValue)
                    {
                        queryBuilder.Append("\"IsActive\" = @IsActive,");
                        command.Parameters.AddWithValue("@IsActive", review.IsActive);
                    }
                    if (review.UpdatedByUserId.HasValue)
                    {
                        queryBuilder.Append("\"UpdatedByUserId\" = @UpdatedByUserId,");
                        command.Parameters.AddWithValue("@UpdatedByUserId", review.UpdatedByUserId);
                    }
                    if (review.DateUpdated.HasValue)
                    {
                        queryBuilder.Append("\"DateUpdated\" = @DateUpdated,");
                        command.Parameters.AddWithValue("@DateUpdated", review.DateUpdated);
                    }
                    if (queryBuilder.ToString().EndsWith(","))
                    {
                        queryBuilder.Length -= 1;
                    }

                    queryBuilder.Append(" WHERE \"Id\" = @Id");
                    command.Parameters.AddWithValue("@Id", id);
                    queryBuilder.Append(" RETURNING *");
                    command.CommandText = queryBuilder.ToString();
                    connection.CreateCommand();
                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        Review updatedReview = MapReviewModel(reader);
                        updatedReview.UserName = currentReview.UserName;
                        updatedReview.EventName = currentReview.EventName;
                        return updatedReview;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        private async Task<Review> GetReviewByIdAsync(Guid id)
        {
            try
            {
                NpgsqlConnection connection = new NpgsqlConnection(connectionString);
                using (connection)
                {
                    StringBuilder queryBuilder = new StringBuilder();

                    queryBuilder.Append("SELECT r.*, u.\"Username\" AS \"CreatedByUsername\", e.\"Name\" AS \"EventName\" ");
                    queryBuilder.Append("FROM \"Review\" r ");
                    queryBuilder.Append("JOIN \"Event\" e ON r.\"EventId\" = e.\"Id\" ");
                    queryBuilder.Append("JOIN \"User\" u ON r.\"CreatedByUserId\" = u.\"Id\" ");
                    queryBuilder.Append("WHERE r.\"Id\" = @Id");

                    NpgsqlCommand command = new NpgsqlCommand(queryBuilder.ToString());
                    command.Connection = connection;
                    command.Parameters.AddWithValue("@Id", id);
                    await connection.OpenAsync();
                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        Review review = MapReview(reader);

                        return review;
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        private Review MapReview(NpgsqlDataReader reader)
        {
            Review review = new Review();

            review.Id = (Guid)reader["Id"];
            review.Content = (string)reader["Content"];
            review.Rating = (int)reader["Rating"];
            review.Attended = (bool)reader["Attended"];
            review.EventName = (string)reader["EventName"];
            review.UserName = (string)reader["CreatedByUsername"];
            review.EventId = (Guid)reader["EventId"];
            review.IsActive = (bool?)reader["IsActive"];
            review.CreatedByUserId = (Guid?)reader["CreatedByUserId"];
            review.UpdatedByUserId = (Guid?)reader["UpdatedByUserId"];
            review.DateCreated = (DateTime?)reader["DateCreated"];
            review.DateUpdated = (DateTime?)reader["DateUpdated"];

            return review;
        }
        private Review MapReviewModel(NpgsqlDataReader reader)
        {
            Review review = new Review();

            review.Id = (Guid)reader["Id"];
            review.Content = (string)reader["Content"];
            review.Rating = (int)reader["Rating"];
            review.Attended = (bool)reader["Attended"];
            review.EventId = (Guid)reader["EventId"];
            review.IsActive = (bool?)reader["IsActive"];
            review.CreatedByUserId = (Guid?)reader["CreatedByUserId"];
            review.UpdatedByUserId = (Guid?)reader["UpdatedByUserId"];
            review.DateCreated = (DateTime?)reader["DateCreated"];
            review.DateUpdated = (DateTime?)reader["DateUpdated"];

            return review;
        }

    }
}
