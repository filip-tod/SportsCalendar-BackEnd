using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Metadata.Providers;
using Npgsql;
using SportCalendar.Common;
using SportCalendar.Model;
using SportCalendar.RepositoryCommon;

namespace SportCalendar.Repository
{
    public class EventSponsorRepository : IEventSponsorRepository
    {
        static private string connectionString = Environment.GetEnvironmentVariable("ConnectionString");

        public async Task<List<EventSponsor>> EventSponsorGetAsync(Guid eventId)
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);

            try
            {

                using (connection)
                {
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    cmd.Connection= connection;
                    cmd.CommandText = $"select * from \"EventSponsor\" where \"IsActive\" = true and \"EventId\" = @eventId";
                    cmd.Parameters.AddWithValue("@eventId", eventId);

                    await connection.OpenAsync();

                    NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
                    List<EventSponsor> eventSponsors = new List<EventSponsor>();

                    if (!reader.HasRows)
                    {
                        return null;
                    }
                    while(await reader.ReadAsync())
                    {
                        eventSponsors.Add(new EventSponsor()
                        {
                            Id = (Guid)reader["Id"],
                            EventId = (Guid)reader["EventId"],
                            SponsorId = (Guid)reader["SponsorId"],
                            IsAcive = (bool)reader["IsActive"],
                            UpdatedByUserId = (Guid)reader["UpdatedByuserId"],
                            CreatedByUserId = (Guid)reader["CreatedByuserId"],
                            DateCreated = (DateTime)reader["DateCreated"],
                            DateUpdated = (DateTime)reader["DateUpdated"]
                        });
                    }
                    return eventSponsors;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<bool> EventSponsorPostAsync(EventSponsor eventSponsor)
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);

            try
            {
                using (connection)
                {
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    cmd.Connection = connection;
                    cmd.CommandText = $"insert into \"EventSponsor\" (\"Id\", \"EventId\", \"SponsorId\", \"IsActive\", \"UpdatedByUserId\", \"CreatedByUserId\", \"DateCreated\", \"DateUpdated\") values (@id,@eventId,@sponsorId,@isActive,@updated,@created,@datecr,@dateup)";
                    cmd.Parameters.AddWithValue("@id", eventSponsor.Id);
                    cmd.Parameters.AddWithValue("@eventId", eventSponsor.EventId);
                    cmd.Parameters.AddWithValue("@sponsorId", eventSponsor.SponsorId);
                    cmd.Parameters.AddWithValue("@isActive", eventSponsor.IsAcive);
                    cmd.Parameters.AddWithValue("@updated", eventSponsor.UpdatedByUserId);
                    cmd.Parameters.AddWithValue("@created", eventSponsor.CreatedByUserId);
                    cmd.Parameters.AddWithValue("@datecr", eventSponsor.DateCreated);
                    cmd.Parameters.AddWithValue("dateup", eventSponsor.DateUpdated);
                    await connection.OpenAsync();

                    if(await cmd.ExecuteNonQueryAsync() >0)
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

        public async Task<bool> EventSponsorDeleteAsync(Guid id, EventSponsor eventSponsor)
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            try
            {
                using (connection)
                {
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    cmd.CommandText = $"update \"EventSponsor\" set \"IsActive\" = false, \"UpdatedByUserId\" = @updated, \"DateUpdated\" = @dateup where \"Id\" = @id";
                    cmd.Connection = connection;
                    await connection.OpenAsync();
                    cmd.Parameters.AddWithValue("updated", eventSponsor.UpdatedByUserId);
                    cmd.Parameters.AddWithValue("dateup", eventSponsor.DateUpdated);
                    cmd.Parameters.AddWithValue("@id", id);
                    if(await cmd.ExecuteNonQueryAsync()>0)
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

        public async Task<bool> EventSponsorPutAsync(Guid id, EventSponsor eventSponsor)
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);

            EventSponsor getEventSponsor = await GetEventSponsorByIdAsync(id);

            try
            {
                using (connection)
                {
                    var queryBuilder = new StringBuilder("");
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    queryBuilder.Append($"update \"EventSponsor\" set ");

                    cmd.Connection = connection;
                    await connection.OpenAsync();

                    if(eventSponsor.EventId == null)
                    {
                        cmd.Parameters.AddWithValue("@eventId", eventSponsor.EventId = getEventSponsor.EventId);
                    }
                    queryBuilder.Append($"\"EventId\" = @eventId, ");
                    cmd.Parameters.AddWithValue("eventId", eventSponsor.EventId);
                    if(eventSponsor.SponsorId == null)
                    {
                        cmd.Parameters.AddWithValue("sponsordId", eventSponsor.SponsorId = getEventSponsor.SponsorId);
                    }
                    queryBuilder.Append($"\"SponsorId\" = @sponsorId, ");
                    cmd.Parameters.AddWithValue("sponsorId", eventSponsor.SponsorId);
                    if(eventSponsor.IsAcive == true  || false)
                    {
                        cmd.Parameters.AddWithValue("@isActive", eventSponsor.IsAcive);
                        queryBuilder.Append($"\"IsActive\" = @isActive, ");
                    }

                    if(queryBuilder.ToString().EndsWith(", "))
                    {
                        if(queryBuilder.Length > 0)
                        {
                            queryBuilder.Remove(queryBuilder.Length - 2, 1);
                        }
                    }

                    queryBuilder.Append($" where \"Id\" = @id");
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.CommandText = queryBuilder.ToString();

                    if(await cmd.ExecuteNonQueryAsync() > 0 )
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

        private async Task<EventSponsor> GetEventSponsorByIdAsync(Guid id)
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);

            try
            {
                using (connection)
                {
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    cmd.Connection = connection;
                    cmd.CommandText = $"select * from \"EventSponsor\"";
                    await connection.OpenAsync();
                    EventSponsor eventSponsor = new EventSponsor();
                    NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            eventSponsor.Id = (Guid)reader["Id"];
                            eventSponsor.EventId = (Guid)reader["EventId"];
                            eventSponsor.SponsorId = (Guid)reader["SponsorId"];
                            eventSponsor.IsAcive = (bool)reader["IsActive"];
                            eventSponsor.UpdatedByUserId = (Guid)reader["UpdatedByUserId"];
                            eventSponsor.CreatedByUserId = (Guid)reader["CreatedByUserId"];
                            eventSponsor.DateCreated = (DateTime)reader["DateCreated"];
                            eventSponsor.DateUpdated = (DateTime)reader["DateUpdated"];
                        }
                    }
                    return eventSponsor;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
