using Npgsql;
using SportCalendar.Common;
using SportCalendar.Model;
using SportCalendar.RepositoryCommon;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace SportCalendar.Repository
{
    public class LocationRepository : ILocationRepository
    {

        //Enviroment varijabla 
        private static string connectionString = Environment.GetEnvironmentVariable("ConnectionString");
        public async Task<List<Location>> GetAllREST(Paging paging, Sorting sorting)
        {
            List<Location> locations = new List<Location>();
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = @"SELECT l.""Id"", l.""Venue"", l.""IsActive"", co.""Name"" AS CountyName, ci.""Name"" AS CityName 
                FROM public.""Location"" l 
                JOIN public.""County"" co ON l.""CountyId"" = co.""Id"" 
                JOIN public.""City"" ci ON l.""CityId"" = ci.""Id""";

                if (sorting != null)
                {
                    var orderByMapped = "";
                    switch (sorting.OrderBy.ToLower())
                    {
                        case "id":
                            orderByMapped = "l.\"Id\"";
                            break;
                        case "venue":
                            orderByMapped = "l.\"Venue\"";
                            break;
                        case "isactive":
                            orderByMapped = "l.\"IsActive\"";
                            break;
                        case "countyname":
                            orderByMapped = "co.\"Name\"";
                            break;
                        case "cityname":
                            orderByMapped = "ci.\"Name\"";
                            break;
                        default:
                            orderByMapped = "l.\"Venue\""; // Default to Venue if invalid
                            break;
                    }

                    string sortOrder = (sorting.SortOrder.ToUpper() == "ASC" || sorting.SortOrder.ToUpper() == "DESC")
                                ? sorting.SortOrder
                                : "ASC"; // Default to ascending if invalid
                    query += $" ORDER BY {orderByMapped} {sortOrder}";
                }

                if (paging != null)
                {
                    int offset = (paging.PageNumber - 1) * paging.PageSize;
                    int limit = paging.PageSize;
                    query += " LIMIT " + limit + " OFFSET " + offset;
                }

                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            Location location = new Location
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                                Venue = reader.GetString(reader.GetOrdinal("Venue")),
                                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                                CityName = reader.IsDBNull(reader.GetOrdinal("CityName")) ? null : reader.GetString(reader.GetOrdinal("CityName")),
                                CountyName = reader.IsDBNull(reader.GetOrdinal("CountyName")) ? null : reader.GetString(reader.GetOrdinal("CountyName"))
                            };

                            locations.Add(location);
                        }
                    }
                }
            }
            return locations;
        }
        //GetById rest Works
        public async Task<Location> GetById(Guid id)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                string query = "SELECT l.\"Id\", l.\"Venue\", l.\"IsActive\", co.\"Name\" AS CountyName, ci.\"Name\" AS CityName " +
                   "FROM public.\"Location\" l " +
                   "JOIN public.\"County\" co ON l.\"CountyId\" = co.\"Id\" " +
                   "JOIN public.\"City\" ci ON l.\"CityId\" = ci.\"Id\" " +
                   "WHERE l.\"Id\" = @Id";


                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            Location location = new Location
                            {
                                Id = (Guid)reader["Id"],
                                Venue = (string)reader["Venue"],
                                IsActive = (bool)reader["IsActive"],
                                CityName = (string)reader["CityName"],
                                CountyName = (string)reader["CountyName"]
                            };

                            return location;
                        }
                    }
                }
            }

            return null; 
        }
        // Create works, need to add JSON body on request with working CountyId and CityId
        public async Task<Location> Create(Location location)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var insertQuery = @"INSERT INTO public.""Location"" 
                    (""Id"", ""Venue"", ""IsActive"", ""UpdatedByUserId"", ""CreatedByUserId"", 
                    ""DateCreated"", ""DateUpdated"", ""CountyId"", ""CityId"")
                    VALUES
                    (@Id, @Venue, @IsActive, @UpdatedByUserId, @CreatedByUserId,
                    @DateCreated, @DateUpdated, @CountyId, @CityId)";

                        using (NpgsqlCommand command = new NpgsqlCommand(insertQuery, connection))
                        {
                            command.Parameters.AddWithValue("@Id", location.Id);
                            command.Parameters.AddWithValue("@Venue", location.Venue);
                            command.Parameters.AddWithValue("@IsActive", location.IsActive);
                            command.Parameters.AddWithValue("@UpdatedByUserId", location.UpdatedByUserId);
                            command.Parameters.AddWithValue("@CreatedByUserId", location.CreatedByUserId);
                            command.Parameters.AddWithValue("@DateCreated", location.DateCreated);
                            command.Parameters.AddWithValue("@DateUpdated", location.DateUpdated);
                            command.Parameters.AddWithValue("@CountyId", location.CountyId);
                            command.Parameters.AddWithValue("@CityId", location.CityId);

                            await Task.Run(() => command.ExecuteNonQuery());
                        }

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }

            return location;
        }
        // PUT works need to add JSON body on request with working CountyId and CityId
        public async Task<Location> Put(Location location, Guid id)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var updateQuery = @"UPDATE public.""Location"" SET
                    ""Venue"" = @Venue,
                    ""IsActive"" = @IsActive,
                    ""UpdatedByUserId"" = @UpdatedByUserId,
                    ""CreatedByUserId"" = @CreatedByUserId,
                    ""DateCreated"" = @DateCreated,
                    ""DateUpdated"" = @DateUpdated,
                    ""CountyId"" = @CountyId,
                    ""CityId"" = @CityId
                    WHERE ""Id"" = @Id";

                        using (NpgsqlCommand command = new NpgsqlCommand(updateQuery, connection))
                        {
                            command.Parameters.AddWithValue("@Id", id);
                            command.Parameters.AddWithValue("@Venue", location.Venue);
                            command.Parameters.AddWithValue("@IsActive", location.IsActive);
                            command.Parameters.AddWithValue("@UpdatedByUserId", location.UpdatedByUserId);
                            command.Parameters.AddWithValue("@CreatedByUserId", location.CreatedByUserId);
                            command.Parameters.AddWithValue("@DateCreated", location.DateCreated);
                            command.Parameters.AddWithValue("@DateUpdated", location.DateUpdated);
                            command.Parameters.AddWithValue("@CountyId", location.CountyId);
                            command.Parameters.AddWithValue("@CityId", location.CityId);

                            await command.ExecuteNonQueryAsync();
                        }

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }

            return location;
        }
        public async Task<bool> Delete(Guid id)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var query = "UPDATE \"Location\" SET \"IsActive\" = false WHERE \"Id\" = @Id";
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);

                    int rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }


    }
}

