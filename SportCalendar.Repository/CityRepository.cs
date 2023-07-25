using Npgsql;
using SportCalendar.Common;
using SportCalendar.Model;
using SportCalendar.ModelCommon;
using SportCalendar.RepositoryCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportCalendar.Repository
{
    public class CityRepository : ICityRepository
    {
        //Enviroment varijabla 
        private static string connectionString = Environment.GetEnvironmentVariable("ConnectionString");

        public async Task<List<City>> GetAll(Paging paging)
        {
            List<City> city = new List<City>();
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var query = "SELECT * FROM \"City\"";
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
                            City cityatributes = new City();
                            cityatributes.Id = reader.GetGuid(reader.GetOrdinal("Id"));
                            cityatributes.Name = reader.GetString(reader.GetOrdinal("Name"));
                            cityatributes.IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"));
                            cityatributes.UpdatedByUserId = reader.GetGuid(reader.GetOrdinal("UpdatedByUserId"));
                            cityatributes.CreatedByUserId = reader.GetGuid(reader.GetOrdinal("CreatedByUserId"));
                            cityatributes.DateCreated = reader.GetDateTime(reader.GetOrdinal("DateCreated"));
                            cityatributes.DateUpdated = reader.GetDateTime(reader.GetOrdinal("DateUpdated"));

                            city.Add(cityatributes);
                        }
                        reader.Close();
                    }
                }
            }
            return city;
        }
        //Get by Id 
        public async Task<List<City>> GetById(Guid id)
        {
            List<City> city = new List<City>();
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var query = "SELECT * FROM \"City\" WHERE \"Id\" = @Id";
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (NpgsqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            City cityatributes = new City();
                            cityatributes.Id = reader.GetGuid(reader.GetOrdinal("Id"));
                            cityatributes.Name = reader.GetString(reader.GetOrdinal("Name"));
                            cityatributes.IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"));
                            cityatributes.UpdatedByUserId = reader.GetGuid(reader.GetOrdinal("UpdatedByUserId"));
                            cityatributes.CreatedByUserId = reader.GetGuid(reader.GetOrdinal("CreatedByUserId"));
                            cityatributes.DateCreated = reader.GetDateTime(reader.GetOrdinal("DateCreated"));
                            cityatributes.DateUpdated = reader.GetDateTime(reader.GetOrdinal("DateUpdated"));

                            city.Add(cityatributes);
                        }
                        reader.Close();
                    }

                }
            }
            return city;
        }
        public async Task<City> Post(City city)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var query = "INSERT INTO \"City\" (\"Id\", \"Name\", \"IsActive\", \"UpdatedByUserId\", \"CreatedByUserId\", \"DateCreated\", \"DateUpdated\") " +
                            "VALUES (@Id, @Name, @IsActive, @UpdatedByUserId, @CreatedByUserId, @DateCreated, @DateUpdated)";
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    Guid id = Guid.NewGuid();
                    city.Id = id;
                    command.Parameters.AddWithValue("@Id", city.Id);
                    command.Parameters.AddWithValue("@Name", city.Name);
                    command.Parameters.AddWithValue("@IsActive", city.IsActive);
                    command.Parameters.AddWithValue("@UpdatedByUserId", city.UpdatedByUserId);
                    command.Parameters.AddWithValue("@CreatedByUserId", city.CreatedByUserId);
                    command.Parameters.AddWithValue("@DateCreated", city.DateCreated);
                    command.Parameters.AddWithValue("@DateUpdated", city.DateUpdated);

                    await command.ExecuteNonQueryAsync();
                }
            }

            return city;
        }
        public async Task<City> Put(Guid id, City updatedCity)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var query = "UPDATE \"City\" SET \"Name\" = @Name, \"IsActive\" = @IsActive, \"UpdatedByUserId\" = @UpdatedByUserId, " +
                            "\"DateUpdated\" = @DateUpdated WHERE \"Id\" = @Id";
                using (NpgsqlCommand command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.Parameters.AddWithValue("@Name", updatedCity.Name);
                    command.Parameters.AddWithValue("@IsActive", updatedCity.IsActive);
                    command.Parameters.AddWithValue("@UpdatedByUserId", updatedCity.UpdatedByUserId);
                    command.Parameters.AddWithValue("@DateUpdated", updatedCity.DateUpdated);

                    await command.ExecuteNonQueryAsync();
                }
            }

            return updatedCity;
        }
        public async Task<bool> Delete(Guid id)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var query = "UPDATE \"City\" SET \"IsActive\" = false WHERE \"Id\" = @Id";
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
