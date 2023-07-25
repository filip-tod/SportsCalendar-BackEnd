using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Metadata.Providers;
using Npgsql;
using SportCalendar.Common;
using SportCalendar.Model;
using SportCalendar.ModelCommon;
using SportCalendar.RepositoryCommon;

namespace SportCalendar.Repository
{
    public class PlacementRepository : IPlacementRepository
    {
        private string connectionString = Environment.GetEnvironmentVariable("ConnectionString");

        public async Task<PagedList<Placement>> PlacementGetFilteredAync(Paging paging, Sorting sorting, PlacementFilter placementFilter)
        {
            List<Placement> placments = new List<Placement>();
            int totalPlacements = 0;
            try
            {
                NpgsqlConnection connection = new NpgsqlConnection(connectionString);
                using (connection)
                {
                    StringBuilder queryBuilder = new StringBuilder();
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    cmd.Connection = connection;

                    queryBuilder.Append($"select * from \"Placement\"");
                    queryBuilder.Append($"where \"IsActive\" = true");

                    if  (placementFilter.EventId != null)
                    {
                        queryBuilder.Append($" and \"EventId\" =@EventId");
                        cmd.Parameters.AddWithValue("@EventId", placementFilter.EventId);
                    }
                    if (sorting != null)
                    {
                        queryBuilder.Append(SortQuerry(sorting, cmd));
                    }
                    
                    queryBuilder.Append($" offset @offset limit @limit");

                    cmd.Parameters.AddWithValue("@offset", paging.PageSize * (paging.PageNumber - 1));
                    cmd.Parameters.AddWithValue("@limit", paging.PageSize);

                    cmd.CommandText = queryBuilder.ToString();
                    connection.CreateCommand();
                    await connection.OpenAsync();

                    NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
                    if(!reader.HasRows)
                    {
                        return null;
                    }
                    while (reader.Read())
                    {
                        placments.Add(new Placement
                        {
                            Id = (Guid)reader["Id"],
                            Name = (string)reader["Name"],
                            FinishOrder = (int?)reader["Finishorder"],
                            EventId = (Guid)reader["EventId"],
                            IsActive = (bool)reader["IsActive"],
                            CreatedByUserId = (Guid)reader["CreatedByUserId"],
                            UpdatedByUserId = (Guid)reader["UpdatedByUserId"],
                            DateCreated = (DateTime)reader["DateCreated"],
                            DateUpdated = (DateTime)reader["DateUpdated"]
                        });
                    }
                    await reader.CloseAsync();

                    NpgsqlCommand countCmd = new NpgsqlCommand();
                    countCmd.Connection = connection;
                    countCmd.CommandText = "SELECT COUNT(*) FROM \"Placement\" WHERE \"IsActive\" = true";
                    totalPlacements = Convert.ToInt32(await countCmd.ExecuteScalarAsync());

                    PagedList<Placement> pagedList = new PagedList<Placement>();
                    pagedList.TotalPages=totalPlacements / paging.PageSize;
                    pagedList.PageSize = paging.PageSize;
                    pagedList.TotalCount = totalPlacements;
                    pagedList.Data = placments;
                    return pagedList;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }

        public async Task<bool> PlacementPostAsync(Placement placement)
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            try
            {
                using (connection)
                {
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    cmd.Connection = connection;
                    cmd.CommandText = $"insert into \"Placement\" (\"Id\", \"Name\", \"FinishOrder\", \"EventId\", \"IsActive\", \"CreatedByUserId\", \"UpdatedByUserId\", \"DateCreated\", \"DateUpdated\") values (@id,@name,@finishOrder,@eventId,@IsActive,@created,@updated,@datecr,@dateup)";
                    cmd.Parameters.AddWithValue("@id", placement.Id);
                    cmd.Parameters.AddWithValue("@name", placement.Name);
                    cmd.Parameters.AddWithValue("@finishOrder", placement.FinishOrder);
                    cmd.Parameters.AddWithValue("@eventId", placement.EventId);
                    cmd.Parameters.AddWithValue("@IsActive", placement.IsActive);
                    cmd.Parameters.AddWithValue("@created", placement.CreatedByUserId);
                    cmd.Parameters.AddWithValue("@updated", placement.UpdatedByUserId);
                    cmd.Parameters.AddWithValue("datecr", placement.DateUpdated);
                    cmd.Parameters.AddWithValue("dateup", placement.DateUpdated);
                    await connection.OpenAsync();

                    if(await cmd.ExecuteNonQueryAsync() > 0)
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> PlacementDeleteAsync(Guid id, Placement placement)
        {
            try
            {
                NpgsqlConnection connection = new NpgsqlConnection(connectionString);

                using (connection)
                {
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    cmd.CommandText = $"update \"Placement\" set \"IsActive\" = false, \"UpdatedByUserId\" = @updated, \"DateUpdated\" = @dateup where \"Id\"=@id";
                    cmd.Connection = connection;
                    await connection.OpenAsync();
                    cmd.Parameters.AddWithValue("@updated", placement.UpdatedByUserId);
                    cmd.Parameters.AddWithValue("@dateup", placement.DateUpdated);
                    cmd.Parameters.AddWithValue("@id", id);
                    if (await cmd.ExecuteNonQueryAsync() > 0)
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<bool> PlacementPutAsync(Guid id, Placement placement)
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);

            Placement getPlacement = await GetPlacementByIdAsync(id);

            try
            {
                using (connection)
                {
                    var queryBuilder = new StringBuilder("");
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    queryBuilder.Append($"update \"Placement\" set");

                    cmd.Connection = connection;
                    await connection.OpenAsync();

                    if (String.IsNullOrEmpty(placement.Name))
                    {
                        cmd.Parameters.AddWithValue("@name", placement.Name = getPlacement.Name);
                    }
                    queryBuilder.Append($"\"Name\" = @name, ");
                    cmd.Parameters.AddWithValue("@name", placement.Name);
                    if(placement.FinishOrder == getPlacement.FinishOrder)
                    {
                        cmd.Parameters.AddWithValue("@finishOrder", getPlacement.FinishOrder);
                    }
                    queryBuilder.Append($"\"FinishOrder\" = @finishOrder, ");
                    cmd.Parameters.AddWithValue("@finishOrder", placement.FinishOrder);
                    if(placement.IsActive == true || false)
                    {
                        cmd.Parameters.AddWithValue("@isactive", placement.IsActive);
                        queryBuilder.Append($"\"IsActive\" = @isactive, ");
                    }

                    if (queryBuilder.ToString().EndsWith(", "))
                    {
                        if (queryBuilder.Length > 0)
                        {
                            queryBuilder.Remove(queryBuilder.Length - 2, 1);
                        }
                    }

                    queryBuilder.Append($" where \"Id\" =@id");
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.CommandText = queryBuilder.ToString();

                    if (await cmd.ExecuteNonQueryAsync() > 0)
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
                return false;
            }
        }

        private async Task<Placement> GetPlacementByIdAsync(Guid id)
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);

            try
            {
                using (connection)
                {
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    cmd.Connection = connection;
                    cmd.CommandText = $"select * from \"Placement\"";
                    await connection.OpenAsync();
                    Placement placement = new Placement();
                    NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            placement.Id = (Guid)reader["Id"];
                            placement.Name = (string)reader["Name"];
                            placement.FinishOrder = (int)reader["FinishOrder"];
                            placement.EventId = (Guid)reader["EventId"];
                            placement.IsActive = (bool)reader["IsActive"];
                            placement.UpdatedByUserId = (Guid)reader["UpdatedByUserId"];
                            placement.CreatedByUserId = (Guid)reader["CreatedByUserId"];
                            placement.DateCreated = (DateTime)reader["DateCreated"];
                            placement.DateUpdated = (DateTime)reader["DateUpdated"];
                        }
                    }
                    return placement;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private StringBuilder SortQuerry(Sorting sorting, NpgsqlCommand cmd)
        {
            StringBuilder queryBuilder = new StringBuilder();
            if(sorting.OrderBy == "Name")
            {
                queryBuilder.Append(" order by \"Name\" @sortOrder");
                cmd.Parameters.AddWithValue("@sortOrder", sorting.SortOrder);
            }
            if(sorting.OrderBy == "FinishOrder")
            {
                queryBuilder.Append(" order by \"FinishOrder\"");
                cmd.Parameters.AddWithValue("@sortOrder", sorting.SortOrder);
            }
            return queryBuilder;
        }
    }
}
