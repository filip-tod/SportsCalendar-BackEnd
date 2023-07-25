using SportCalendar.RepositoryCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Npgsql;
using System.Web;
using SportCalendar.Model;
using System.Web.Http;
using SportCalendar.Common;

namespace SportCalendar.Repository
{
    
    public class SportRepository : ISportRepository
    {
        private string connectionString = Environment.GetEnvironmentVariable("ConnectionString");

        public async Task<PagedList<Sport>> GetSportsAsync(Sorting sorting, Paging paging, SportFilter filtering)
        {
            List<Sport> sports = new List<Sport>();
            int totalSports = 0;
            try
            {
                NpgsqlConnection connection = new NpgsqlConnection(connectionString);
                using (connection)
                {
                    var queryBuilder = new StringBuilder();
                    NpgsqlCommand command = new NpgsqlCommand();
                    command.Connection = connection;

                    queryBuilder.Append("SELECT * FROM \"Sport\" ");
                    queryBuilder.Append("WHERE \"IsActive\" = true ");

                    queryBuilder.Append(FilterQuery(filtering, command));
                    if (sorting.OrderBy != null)
                    {
                        queryBuilder.Append("ORDER BY \"Sport\".");
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
                        sports.Add(MapSport(reader));
                    }
                    await reader.CloseAsync();

                    NpgsqlCommand commandTotal = new NpgsqlCommand();
                    StringBuilder queryTotal = new StringBuilder();
                    queryTotal.Append("SELECT COUNT(\"Name\") FROM \"Sport\" WHERE \"IsActive\" = true ");
                    queryTotal.Append(FilterQuery(filtering, commandTotal));
                    commandTotal.CommandText = queryTotal.ToString();
                    commandTotal.Connection = connection;
                    totalSports = Convert.ToInt32(await commandTotal.ExecuteScalarAsync());

                    PagedList<Sport> paginatedList = new PagedList<Sport>();
                    paginatedList.TotalCount = totalSports;
                    paginatedList.Data = sports;
                    paginatedList.TotalPages = totalSports / paging.PageSize;
                    paginatedList.PageSize = paging.PageSize;
                    paginatedList.CurrentPage = paging.PageNumber;
                    return paginatedList;
                }
            }catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        private StringBuilder FilterQuery(SportFilter filtering, NpgsqlCommand command)
        {
            StringBuilder queryBuilder = new StringBuilder();
            if (filtering.SearchQuery != null)
            {
                queryBuilder.Append("AND \"Name\" = @Name ");
                command.Parameters.AddWithValue("@Name", filtering.SearchQuery);
            }
            if (filtering.Type != null)
            {
                queryBuilder.Append("AND \"Type\" = @Type ");
                command.Parameters.AddWithValue("@Type", filtering.Type);
            }
            return queryBuilder;
        }
        
        public async Task<Sport> GetSportAsync(Guid id)
        {
            try
            {
                Sport sport = await GetSportByIdAsync(id);
                if(sport == null)
                {
                    return null;
                }
                return sport;
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public async Task<bool> DeleteSportAsync(Guid id)
        {

            try
            {
                NpgsqlConnection connection = new NpgsqlConnection(connectionString);
                Sport sport = await GetSportByIdAsync(id);

                Guid userId = Guid.Parse("0d3fa5c2-684c-4d88-82fd-cea2197c6e86");
                sport.UpdatedByUserId = userId;
                sport.DateUpdated = DateTime.Now;

                if (sport == null)
                {
                    return false;
                }
                using (connection)
                {
                    sport.IsActive = false;
                    var queryBuilder = new StringBuilder();
                    NpgsqlCommand command = new NpgsqlCommand();
                    command.Connection = connection;

                    queryBuilder.Append("UPDATE \"Sport\" SET \"IsActive\" = @IsActive,");
                    queryBuilder.Append(" \"UpdatedByUserId\" = @UpdatedByUserId,");
                    queryBuilder.Append(" \"DateUpdated\" = @DateUpdated");
                    queryBuilder.Append(" WHERE \"Id\" = @Id");

                    command.Parameters.AddWithValue("@IsActive",sport.IsActive);
                    command.Parameters.AddWithValue("@UpdatedByUserId", sport.UpdatedByUserId);
                    command.Parameters.AddWithValue("@DateUpdated", sport.DateUpdated);
                    command.Parameters.AddWithValue("@Id", id);

                    command.CommandText = queryBuilder.ToString();
                    connection.CreateCommand();
                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                    //need to wait for auth so I can set updated Id and Date
                    return true;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<Sport> PostSportAsync(Sport sport)
        {
            try
            {
                NpgsqlConnection connection = new NpgsqlConnection(connectionString);
                using (connection)
                {
                    NpgsqlCommand command = new NpgsqlCommand();
                    command.CommandText = "INSERT INTO \"Sport\" values(@Id,@Name,@Type,@IsActive,@CreatedByUserId,@UpdatedByUserId,@DateCreated,@DateUpdated) RETURNING *";
                    command.Connection = connection;
                    connection.CreateCommand();
                    await connection.OpenAsync();

                    Guid id = Guid.NewGuid();
                    sport.Id = id;
                    command.Parameters.AddWithValue("@Id", sport.Id);
                    command.Parameters.AddWithValue("@Name", sport.Name);
                    command.Parameters.AddWithValue("@Type", sport.Type);
                    command.Parameters.AddWithValue("@IsActive", sport.IsActive);
                    command.Parameters.AddWithValue("@CreatedByUserId",sport.CreatedByUserId);
                    command.Parameters.AddWithValue("@UpdatedByUserId", sport.UpdatedByUserId);
                    command.Parameters.AddWithValue("@DateCreated", sport.DateCreated);
                    command.Parameters.AddWithValue("@DateUpdated", sport.DateUpdated);

                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        var savedSport = new Sport();

                        sport.Id = (Guid)reader["Id"];
                        sport.Name = (string)reader["Name"];
                        sport.Type = (string)reader["Type"];
                        sport.IsActive = (bool)reader["IsActive"];
                        sport.CreatedByUserId = (Guid)reader["CreatedByUserId"];
                        sport.UpdatedByUserId = (Guid)reader["UpdatedByUserId"];
                        sport.DateCreated = (DateTime)reader["DateCreated"];
                        sport.DateUpdated = (DateTime)reader["DateUpdated"];

                        return savedSport;
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
        public async Task<Sport> UpdateSportAsync(Guid id,Sport sport)
        {
            try
            {
                Sport currentSport = await GetSportByIdAsync(id);
                if (currentSport == null)
                {
                    return null;
                }
                NpgsqlConnection connection = new NpgsqlConnection(connectionString);
                using (connection)
                {
                    var queryBuilder = new StringBuilder();
                    NpgsqlCommand command = new NpgsqlCommand();
                    queryBuilder.Append("UPDATE \"Sport\" SET ");
                    command.Connection = connection;
                    await connection.OpenAsync();

                    if(sport.Name!=null)
                    {
                        queryBuilder.Append(" \"Name\" = @Name,");
                        command.Parameters.AddWithValue("@Name", sport.Name);
                    }
                    if (sport.Type != null)
                    {
                        queryBuilder.Append(" \"Type\" = @Type,");
                        command.Parameters.AddWithValue("@Type", sport.Type);
                    }
                    if(sport.UpdatedByUserId!= null)
                    {
                        queryBuilder.Append(" \"UpdatedByUserId\" = @UpdatedByUserId,");
                        command.Parameters.AddWithValue("@UpdatedByUserId",sport.UpdatedByUserId);
                    }
                    if (sport.DateUpdated != null)
                    {
                        queryBuilder.Append(" \"DateUpdated\" = @DateUpdated,");
                        command.Parameters.AddWithValue("@DateUpdated", sport.DateUpdated);
                    }
                    if (sport.IsActive != null)
                    {
                        queryBuilder.Append(" \"IsActive\" = @IsActive,");
                        command.Parameters.AddWithValue("@IsActive", sport.IsActive);
                    }
                    if (queryBuilder.ToString().EndsWith(","))
                    {
                        if (queryBuilder.Length > 0)
                        {
                            queryBuilder.Remove(queryBuilder.Length - 1, 1);
                        }
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

                        Sport updatedSport = new Sport();

                        updatedSport.Id = id;
                        updatedSport.Name = (string)reader["Name"];
                        updatedSport.Type = (string)reader["Type"];
                        updatedSport.IsActive = (bool)reader["IsActive"];
                        updatedSport.CreatedByUserId = (Guid)reader["CreatedByUserId"];
                        updatedSport.UpdatedByUserId = (Guid)reader["UpdatedByUserId"];
                        updatedSport.DateCreated = (DateTime)reader["DateCreated"];
                        updatedSport.DateUpdated = (DateTime)reader["DateUpdated"];

                        return updatedSport;
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
        private async Task<Sport> GetSportByIdAsync(Guid id)
        {
            try
            {
                NpgsqlConnection connection = new NpgsqlConnection(connectionString);
                using (connection)
                {
                    NpgsqlCommand command = new NpgsqlCommand();
                    command.CommandText = "SELECT * FROM \"Sport\" WHERE \"Id\"=@Id";
                    command.Connection = connection;
                    command.Parameters.AddWithValue("@Id",id);
                    await connection.OpenAsync();
                    
                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        reader.Read();

                        Sport sport = MapSport(reader);
                        return sport;
                    }
                    return null;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        private Sport MapSport(NpgsqlDataReader reader)
        {
            Sport sport = new Sport();

            sport.Id = (Guid)reader["Id"];
            sport.Name = (string)reader["Name"];
            sport.Type = (string)reader["Type"];
            sport.IsActive = (bool)reader["IsActive"];
            sport.CreatedByUserId = (Guid)reader["CreatedByUserId"];
            sport.UpdatedByUserId = (Guid)reader["UpdatedByUserId"];
            sport.DateCreated = (DateTime)reader["DateCreated"];
            sport.DateUpdated = (DateTime)reader["DateUpdated"];

            return sport;
        }

        
    }
}
