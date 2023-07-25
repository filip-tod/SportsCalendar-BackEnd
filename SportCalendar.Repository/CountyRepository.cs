using Npgsql;
using SportCalendar.Common;
using SportCalendar.Model;
using SportCalendar.ModelCommon;
using SportCalendar.RepositoryCommon;
using SportCalendar.ServiceCommon;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportCalendar.Repository
{
    public class CountyRepository : ICountyRepository
    {

        //Enviroment varijabla 
        private static string connectionString = Environment.GetEnvironmentVariable("ConnectionString");
        //GET works
        public async Task<List<County>> GetAll(Paging paging)
        {
            List<County> counties = new List<County>();
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();

                var query = "SELECT * FROM \"County\" ORDER BY \"Id\" OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    int offset = (paging.PageNumber - 1) * paging.PageSize;
                    Console.WriteLine("OFFSET: " + offset);
                    command.Parameters.AddWithValue("Offset", offset);
                    command.Parameters.AddWithValue("PageSize", paging.PageSize);
                    using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            County county = new County();
                            county.Id = reader.GetGuid(reader.GetOrdinal("Id"));
                            county.Name = reader.GetString(reader.GetOrdinal("Name"));
                            county.IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"));
                            county.UpdatedByUserId = reader.GetGuid(reader.GetOrdinal("UpdatedByUserId"));
                            county.CreatedByUserId = reader.GetGuid(reader.GetOrdinal("CreatedByUserId"));
                            county.DateCreated = reader.GetDateTime(reader.GetOrdinal("DateCreated"));
                            county.DateUpdated = reader.GetDateTime(reader.GetOrdinal("DateUpdated"));

                            counties.Add(county);
                        }
                        reader.Close();
                    }
                }
            }
            return counties;
        }
        //Get by Id works - insert guid
        public async Task<List<County>> GetById(Guid id)
        {
            List<County> counties = new List<County>();
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var query = "SELECT * FROM \"County\" WHERE \"Id\" = @Id";
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            County county = new County();
                            county.Id = reader.GetGuid(reader.GetOrdinal("Id"));
                            county.Name = reader.GetString(reader.GetOrdinal("Name"));
                            county.IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"));
                            county.UpdatedByUserId = reader.GetGuid(reader.GetOrdinal("UpdatedByUserId"));
                            county.CreatedByUserId = reader.GetGuid(reader.GetOrdinal("CreatedByUserId"));
                            county.DateCreated = reader.GetDateTime(reader.GetOrdinal("DateCreated"));
                            county.DateUpdated = reader.GetDateTime(reader.GetOrdinal("DateUpdated"));
                            counties.Add(county);
                        }
                        reader.Close();
                    }

                }
            }
            return counties;
        }
        //Post Methode 
        public async Task<County> Post(County county)
        {

            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var query = "INSERT INTO \"County\" (\"Id\", \"Name\", \"IsActive\", \"UpdatedByUserId\", \"CreatedByUserId\", \"DateCreated\", \"DateUpdated\") " +
                            "VALUES (@Id, @Name, @IsActive, @UpdatedByUserId, @CreatedByUserId, @DateCreated, @DateUpdated)";
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    Guid id = Guid.NewGuid();
                    county.Id = id;
                    command.Parameters.AddWithValue("@Id", county.Id);
                    command.Parameters.AddWithValue("@Name", county.Name);
                    command.Parameters.AddWithValue("@IsActive", county.IsActive);
                    command.Parameters.AddWithValue("@UpdatedByUserId", county.UpdatedByUserId);
                    command.Parameters.AddWithValue("@CreatedByUserId", county.CreatedByUserId);
                    command.Parameters.AddWithValue("@DateCreated", county.DateCreated);
                    command.Parameters.AddWithValue("@DateUpdated", county.DateUpdated);

                    await command.ExecuteNonQueryAsync();
                }

            }

            return county;
        }
        //works, need to add Guid for insert 
        public async Task<County> Put(Guid id, County county)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var query = "UPDATE \"County\" SET \"Name\" = @Name, \"IsActive\" = @IsActive, \"UpdatedByUserId\" = @UpdatedByUserId, " +
                            "\"CreatedByUserId\" = @CreatedByUserId, \"DateCreated\" = @DateCreated, \"DateUpdated\" = @DateUpdated " +
                            "WHERE \"Id\" = @Id";
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@Name", county.Name);
                    command.Parameters.AddWithValue("@IsActive", county.IsActive);
                    command.Parameters.AddWithValue("@UpdatedByUserId", county.UpdatedByUserId);
                    command.Parameters.AddWithValue("@CreatedByUserId", county.CreatedByUserId);
                    command.Parameters.AddWithValue("@DateCreated", county.DateCreated);
                    command.Parameters.AddWithValue("@DateUpdated", county.DateUpdated);

                    await command.ExecuteNonQueryAsync();
                }
            }

            return county;
        }
        //Soft Delete - change IsActive to false
        public async Task<bool> Delete(Guid id)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var query = "UPDATE \"County\" SET \"IsActive\" = false WHERE \"Id\" = @Id";
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

