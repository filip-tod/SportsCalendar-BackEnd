using Npgsql;
using SportCalendar.Common;
using SportCalendar.Model;
using SportCalendar.ModelCommon;
using SportCalendar.RepositoryCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SportCalendar.Repository
{
    public class EventRepository : IEventRepository
    {
        private string connectionString = Environment.GetEnvironmentVariable("ConnectionString");

        public async Task<PagedList<EventView>> GetEventsAsync(Sorting sorting, Paging paging, EventFilter filtering)
        {
            List<EventView> events = new List<EventView>();
            int totalEvents = 0;
            try
            {
                NpgsqlConnection connection = new NpgsqlConnection(connectionString);
                using (connection)
                {
                    StringBuilder queryBuilder = new StringBuilder();
                    NpgsqlCommand command = new NpgsqlCommand();
                    command.Connection = connection;

                    queryBuilder.Append("SELECT e.*, l.\"Venue\" AS \"VenueName\", c.\"Name\" AS \"CityName\", co.\"Name\" AS \"CountyName\", s.\"Name\" AS \"SportName\", s.\"Type\" AS \"SportType\" ");
                    queryBuilder.Append("FROM \"Event\" e ");
                    queryBuilder.Append("LEFT JOIN \"Location\" l ON e.\"LocationId\" = l.\"Id\" ");
                    queryBuilder.Append("LEFT JOIN \"City\" c ON l.\"CityId\" = c.\"Id\" ");
                    queryBuilder.Append("LEFT JOIN \"County\" co ON l.\"CountyId\" = co.\"Id\" ");
                    queryBuilder.Append("LEFT JOIN \"Sport\" s ON e.\"SportId\" = s.\"Id\" ");
                    queryBuilder.Append("WHERE e.\"IsActive\" = true ");

                    queryBuilder.Append(FilterQuery(filtering, command));

                    if (sorting.OrderBy != null)
                    {
                        queryBuilder.Append("ORDER BY e.");
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

                    EventView eventView = new EventView();
                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();
                    if (!reader.HasRows)
                    {
                        return null;
                    }
                    while (reader.Read())
                    {
                        eventView = MapEvent(reader);
                        events.Add(eventView);
                    }
                    await reader.CloseAsync();
                    await connection.CloseAsync();
                    foreach (EventView e in events)
                    {
                        await connection.OpenAsync();
                        await GetAttendanceAsync(e.Id, connection, e);
                        if (e.Attendance != 0)
                        {
                            await GetRatingAsync(e.Id, connection, e);
                        }
                        else
                        {
                            await connection.CloseAsync();
                        }
                        queryBuilder.Clear();
                        queryBuilder.Append("SELECT s.* FROM \"Sponsor\" s ");
                        queryBuilder.Append("INNER JOIN \"EventSponsor\" es ON s.\"Id\" = es.\"SponsorId\" ");
                        queryBuilder.Append("WHERE es.\"EventId\" = @EventId");
                        await GetSponsorsAsync(connection, queryBuilder, e);

                        queryBuilder.Clear();
                        queryBuilder.Append("SELECT * FROM \"Placement\" WHERE \"EventId\" = @EventId");
                        await GetPlacementsAsync(connection, queryBuilder, e);
                    }
                    await connection.OpenAsync();
                    NpgsqlCommand commandTotal = new NpgsqlCommand();
                    StringBuilder queryTotal = new StringBuilder();
                    queryTotal.Append("SELECT COUNT(\"Id\") ");
                    queryTotal.Append("FROM \"Event\"");
                    commandTotal.CommandText = queryTotal.ToString();
                    commandTotal.Connection = connection;
                    totalEvents = Convert.ToInt32(await commandTotal.ExecuteScalarAsync());

                    PagedList<EventView> paginatedList = new PagedList<EventView>();
                    paginatedList.TotalCount = totalEvents;
                    paginatedList.Data = events;
                    paginatedList.TotalPages = (totalEvents / paging.PageSize);
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
        private StringBuilder FilterQuery(EventFilter filtering, NpgsqlCommand command)
        {
            StringBuilder queryBuilder = new StringBuilder();
            if (!string.IsNullOrEmpty(filtering.SearchQuery))
            {
                queryBuilder.Append(" AND \"Name\" = @Name ");
                command.Parameters.AddWithValue("@Name", filtering.SearchQuery);
            }
            if (!string.IsNullOrEmpty(filtering.Venue))
            {
                queryBuilder.Append(" AND \"Venue\" = @Venue ");
                command.Parameters.AddWithValue("@Venue", filtering.Venue);
            }
            if (!string.IsNullOrEmpty(filtering.Sport))
            {
                queryBuilder.Append(" AND \"Sport\" = @Sport ");
                command.Parameters.AddWithValue("@Sport", filtering.Sport);
            }
            if (!string.IsNullOrEmpty(filtering.City))
            {
                queryBuilder.Append(" AND \"City\" = @City ");
                command.Parameters.AddWithValue("@City", filtering.City);
            }
            if (!string.IsNullOrEmpty(filtering.County))
            {
                queryBuilder.Append(" AND \"County\" = @County ");
                command.Parameters.AddWithValue("@County", filtering.County);
            }
            if (filtering.Rating.HasValue)
            {
                queryBuilder.Append(" AND \"Rating\" = @Rating ");
                command.Parameters.AddWithValue("@Rating", filtering.Rating);
            }
            if (filtering.FromDate.HasValue)
            {
                queryBuilder.Append(" AND \"StartTime\" >= @FromDate ");
                command.Parameters.AddWithValue("@FromDate", filtering.FromDate);
            }
            if (filtering.ToDate.HasValue)
            {
                queryBuilder.Append(" AND \"EndTime\" <= @ToDate ");
                command.Parameters.AddWithValue("@ToDate", filtering.ToDate);
            }
            return queryBuilder;
        }

        public async Task<EventView> GetEventAsync(Guid id)
        {
            try
            {
                NpgsqlConnection connection = new NpgsqlConnection(connectionString);
                using (connection)
                {
                    StringBuilder queryBuilder = new StringBuilder();

                    queryBuilder.Append("SELECT e.*, l.\"Venue\" AS \"VenueName\", c.\"Name\" AS \"CityName\", co.\"Name\" AS \"CountyName\", s.\"Name\" AS \"SportName\", s.\"Type\" AS \"SportType\" ");
                    queryBuilder.Append("FROM \"Event\" e ");
                    queryBuilder.Append("LEFT JOIN \"Location\" l ON e.\"LocationId\" = l.\"Id\" ");
                    queryBuilder.Append("LEFT JOIN \"City\" c ON l.\"CityId\" = c.\"Id\" ");
                    queryBuilder.Append("LEFT JOIN \"County\" co ON l.\"CountyId\" = co.\"Id\" ");
                    queryBuilder.Append("LEFT JOIN \"Sport\" s ON e.\"SportId\" = s.\"Id\" ");
                    queryBuilder.Append("WHERE e.\"Id\" = @Id AND e.\"IsActive\" = true ");

                    EventView eventView = new EventView();

                    using (NpgsqlCommand command = new NpgsqlCommand(queryBuilder.ToString(), connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        await connection.OpenAsync();
                        NpgsqlDataReader reader = await command.ExecuteReaderAsync();
                        if (reader.HasRows)
                        {
                            await reader.ReadAsync();
                            eventView = MapEvent(reader);
                        }
                        await reader.CloseAsync();

                        await GetAttendanceAsync(eventView.Id, connection, eventView);
                        if(eventView.Attendance != 0)
                        {
                            await GetRatingAsync(eventView.Id, connection, eventView);
                        }

                        await connection.CloseAsync();
                    }
                    if (eventView != null)
                    {
                        queryBuilder.Clear();
                        queryBuilder.Append("SELECT s.* FROM \"Sponsor\" s ");
                        queryBuilder.Append("INNER JOIN \"EventSponsor\" es ON s.\"Id\" = es.\"SponsorId\" ");
                        queryBuilder.Append("WHERE es.\"EventId\" = @EventId");
                        await GetSponsorsAsync(connection, queryBuilder, eventView);

                        queryBuilder.Clear();
                        queryBuilder.Append("SELECT * FROM \"Placement\" WHERE \"EventId\" = @EventId");
                        await GetPlacementsAsync(connection, queryBuilder, eventView);
                        return eventView;
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

        private async Task GetSponsorsAsync(NpgsqlConnection connection, StringBuilder queryBuilder, EventView eventView)
        {
            using (NpgsqlCommand command = new NpgsqlCommand(queryBuilder.ToString(), connection))
            {
                command.Parameters.AddWithValue("@EventId", eventView.Id);
                await connection.OpenAsync();
                NpgsqlDataReader reader = await command.ExecuteReaderAsync();
                if (!reader.HasRows)
                {
                    eventView.Sponsors = null;
                }
                eventView.Sponsors = new List<ISponsor>();
                while (reader.Read())
                {
                    eventView.Sponsors.Add(MapSponsor(reader));
                }
                await reader.CloseAsync();
                await connection.CloseAsync();
            }
        }

        private async Task GetPlacementsAsync(NpgsqlConnection connection, StringBuilder queryBuilder, EventView eventView)
        {
            using (NpgsqlCommand command = new NpgsqlCommand(queryBuilder.ToString(), connection))
            {
                command.Parameters.AddWithValue("@EventId", eventView.Id);
                await connection.OpenAsync();
                NpgsqlDataReader reader = await command.ExecuteReaderAsync();
                if (!reader.HasRows)
                {
                    eventView.Placements = null;
                }
                eventView.Placements = new List<IPlacement>();
                while (reader.Read())
                {
                    eventView.Placements.Add(MapPlacement(reader));
                }
                await reader.CloseAsync();
                await connection.CloseAsync();
            }
        }

        private async Task GetAttendanceAsync(Guid? eventId, NpgsqlConnection connection, EventView eventView)
        {
            NpgsqlCommand commandAttendance = new NpgsqlCommand();
            StringBuilder queryAttendance = new StringBuilder();
            queryAttendance.Append("SELECT COUNT(*) FROM \"Review\" WHERE \"EventId\" = @EventId AND \"IsActive\" = true");
            commandAttendance.CommandText = queryAttendance.ToString();
            commandAttendance.Parameters.AddWithValue("@EventId", eventId);
            commandAttendance.Connection = connection;
            eventView.Attendance = Convert.ToInt32(await commandAttendance.ExecuteScalarAsync());
        }
        private async Task GetRatingAsync(Guid? eventId, NpgsqlConnection connection, EventView eventView)
        {
            NpgsqlCommand commandRating = new NpgsqlCommand();
            StringBuilder queryRating = new StringBuilder();
            queryRating.Append("SELECT AVG(\"Rating\") FROM \"Review\" WHERE \"EventId\" = @EventId AND \"IsActive\" = true AND \"Attended\" = true");
            commandRating.CommandText = queryRating.ToString();
            commandRating.Parameters.AddWithValue("@EventId", eventId);
            commandRating.Connection = connection;
            Decimal? averageRating = Convert.ToDecimal(await commandRating.ExecuteScalarAsync());
            if (averageRating.HasValue)
            {
                eventView.Rating = averageRating.Value;
            }
            else
            {
                eventView.Rating = 0;
            }
            //eventView.Rating = Convert.ToDecimal(await commandRating.ExecuteScalarAsync());
            await connection.CloseAsync();
        }

        public async Task<EventModel> PostEventAsync(EventModel eventModel)
        {
            try
            {
                using (NpgsqlConnection connection = new NpgsqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (NpgsqlCommand command = new NpgsqlCommand())
                    {
                        StringBuilder queryBuilder = new StringBuilder();
                        queryBuilder.Append("INSERT INTO \"Event\" (\"Id\", \"Name\", \"Description\", \"Start\", \"End\", \"LocationId\", \"SportId\", \"IsActive\", \"CreatedByUserId\", \"UpdatedByUserId\", \"DateCreated\", \"DateUpdated\") ");
                        queryBuilder.Append("VALUES (@Id, @Name, @Description, @StartTime, @EndTime, @LocationId, @SportId, @IsActive, @CreatedByUserId, @UpdatedByUserId, @DateCreated, @DateUpdated) RETURNING *");
                        command.CommandText = queryBuilder.ToString();
                        command.Connection = connection;

                        Guid id = Guid.NewGuid();
                        eventModel.Id = id;

                        command.Parameters.AddWithValue("@Id", eventModel.Id);
                        command.Parameters.AddWithValue("@Name", eventModel.Name);
                        command.Parameters.AddWithValue("@Description", eventModel.Description);
                        command.Parameters.AddWithValue("@StartTime", eventModel.StartDate);
                        command.Parameters.AddWithValue("@EndTime", eventModel.EndDate);
                        command.Parameters.AddWithValue("@LocationId", eventModel.LocationId);
                        command.Parameters.AddWithValue("@SportId", eventModel.SportId);
                        command.Parameters.AddWithValue("@IsActive", eventModel.IsActive ?? false);
                        command.Parameters.AddWithValue("@CreatedByUserId", eventModel.CreatedByUserId);
                        command.Parameters.AddWithValue("@UpdatedByUserId", eventModel.UpdatedByUserId);
                        command.Parameters.AddWithValue("@DateCreated", eventModel.DateCreated);
                        command.Parameters.AddWithValue("@DateUpdated", eventModel.DateUpdated);

                        NpgsqlDataReader reader = await command.ExecuteReaderAsync();
                        if (reader.HasRows)
                        {
                            await reader.ReadAsync();
                            return MapEventModel(reader);
                        }
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<EventModel> UpdateEventAsync(Guid id, EventModel eventModel)
        {
            try
            {
                EventModel currentEvent = await GetEventByIdAsync(id);
                if (currentEvent == null)
                {
                    return null;
                }
                NpgsqlConnection connection = new NpgsqlConnection(connectionString);
                using (connection)
                {
                    StringBuilder queryBuilder = new StringBuilder();
                    NpgsqlCommand command = new NpgsqlCommand();
                    queryBuilder.Append("UPDATE \"Event\" SET ");
                    command.Connection = connection;
                    await connection.OpenAsync();

                    if (!string.IsNullOrEmpty(eventModel.Name))
                    {
                        queryBuilder.Append("\"Name\" = @Name,");
                        command.Parameters.AddWithValue("@Name", eventModel.Name);
                    }
                    if (!string.IsNullOrEmpty(eventModel.Description))
                    {
                        queryBuilder.Append("\"Description\" = @Description,");
                        command.Parameters.AddWithValue("@Description", eventModel.Description);
                    }
                    if (eventModel.StartDate.HasValue)
                    {
                        queryBuilder.Append("\"Start\" = @StartTime,");
                        command.Parameters.AddWithValue("@StartTime", eventModel.StartDate);
                    }
                    if (eventModel.EndDate.HasValue)
                    {
                        queryBuilder.Append("\"End\" = @EndTime,");
                        command.Parameters.AddWithValue("@EndTime", eventModel.EndDate);
                    }
                    if (eventModel.LocationId.HasValue)
                    {
                        queryBuilder.Append("\"LocationId\" = @LocationId,");
                        command.Parameters.AddWithValue("@LocationId", eventModel.LocationId);
                    }
                    if (eventModel.SportId.HasValue)
                    {
                        queryBuilder.Append("\"SportId\" = @SportId,");
                        command.Parameters.AddWithValue("@SportId", eventModel.SportId);
                    }
                    if (eventModel.IsActive.HasValue)
                    {
                        queryBuilder.Append("\"IsActive\" = @IsActive,");
                        command.Parameters.AddWithValue("@IsActive", eventModel.IsActive);
                    }
                    if (eventModel.UpdatedByUserId.HasValue)
                    {
                        queryBuilder.Append("\"UpdatedByUserId\" = @UpdatedByUserId,");
                        command.Parameters.AddWithValue("@UpdatedByUserId", eventModel.UpdatedByUserId);
                    }
                    if (eventModel.DateUpdated.HasValue)
                    {
                        queryBuilder.Append("\"DateUpdated\" = @DateUpdated,");
                        command.Parameters.AddWithValue("@DateUpdated", eventModel.DateUpdated);
                    }
                    if (queryBuilder.ToString().EndsWith(","))
                    {
                        queryBuilder.Length -= 1;
                    }

                    queryBuilder.Append(" WHERE \"Id\" = @Id");
                    command.Parameters.AddWithValue("@Id", id);
                    command.CommandText = queryBuilder.ToString();
                    queryBuilder.Append(" RETURNING *");
                    command.CommandText = queryBuilder.ToString();
                    connection.CreateCommand();
                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        return MapEventModel(reader);
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

        public async Task<bool> DeleteEventAsync(Guid id)
        {
            try
            {
                NpgsqlConnection connection = new NpgsqlConnection(connectionString);
                EventModel eventModel = await GetEventByIdAsync(id);

                Guid userId = Guid.Parse("0d3fa5c2-684c-4d88-82fd-cea2197c6e86");
                eventModel.UpdatedByUserId = userId;
                eventModel.DateUpdated = DateTime.Now;

                if (eventModel == null)
                {
                    return false;
                }
                using (connection)
                {
                    eventModel.IsActive = false;
                    StringBuilder queryBuilder = new StringBuilder();
                    NpgsqlCommand command = new NpgsqlCommand();
                    command.Connection = connection;

                    queryBuilder.Append("UPDATE \"Event\" SET \"IsActive\" = @IsActive,");
                    queryBuilder.Append(" \"UpdatedByUserId\" = @UpdatedByUserId,");
                    queryBuilder.Append(" \"DateUpdated\" = @DateUpdated");
                    queryBuilder.Append(" WHERE \"Id\" = @Id");

                    command.Parameters.AddWithValue("@IsActive", eventModel.IsActive);
                    command.Parameters.AddWithValue("@UpdatedByUserId", eventModel.UpdatedByUserId);
                    command.Parameters.AddWithValue("@DateUpdated", eventModel.DateUpdated);
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
        private async Task<EventModel> GetEventByIdAsync(Guid id)
        {
            try
            {
                NpgsqlConnection connection = new NpgsqlConnection(connectionString);
                using (connection)
                {
                    StringBuilder queryBuilder = new StringBuilder();

                    queryBuilder.Append("SELECT * FROM \"Event\" WHERE \"Id\" = @Id");
                    NpgsqlCommand command = new NpgsqlCommand(queryBuilder.ToString());
                    command.Connection = connection;
                    command.Parameters.AddWithValue("@Id", id);
                    await connection.OpenAsync();
                    NpgsqlDataReader reader = await command.ExecuteReaderAsync();

                    if (reader.HasRows)
                    {
                        await reader.ReadAsync();
                        EventModel eventModel = MapEventModel(reader);
                        return eventModel;
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

        private EventModel MapEventModel(NpgsqlDataReader reader)
        {
            EventModel eventModel = new EventModel();
            eventModel.Id = (Guid)reader["Id"];
            eventModel.Name = (string)reader["Name"];
            eventModel.Description = (string)reader["Description"];
            eventModel.StartDate = (DateTime)reader["Start"];
            eventModel.EndDate = (DateTime)reader["End"];
            eventModel.LocationId = (Guid)reader["LocationId"];
            eventModel.SportId = (Guid)reader["SportId"];
            eventModel.IsActive = (bool?)reader["IsActive"];
            eventModel.CreatedByUserId = (Guid?)reader["CreatedByUserId"];
            eventModel.UpdatedByUserId = (Guid?)reader["UpdatedByUserId"];
            eventModel.DateCreated = (DateTime?)reader["DateCreated"];
            eventModel.DateUpdated = (DateTime?)reader["DateUpdated"];

            return eventModel;
        }

        private EventView MapEvent(NpgsqlDataReader reader)
        {
            EventView eventView = new EventView();

            eventView.Id = (Guid)reader["Id"];
            eventView.Name = (string)reader["Name"];
            eventView.Description = (string)reader["Description"];
            eventView.StartDate = (DateTime)reader["Start"];
            eventView.EndDate = (DateTime)reader["End"];
            eventView.LocationId = (Guid)reader["LocationId"];
            eventView.SportId = (Guid)reader["SportId"];
            eventView.VenueName = (string)reader["VenueName"];
            eventView.CityName = (string)reader["CityName"];
            eventView.CountyName = (string)reader["CountyName"];
            eventView.SportName = (string)reader["SportName"];
            eventView.SportType = (string)reader["SportType"];
            eventView.IsActive = (bool?)reader["IsActive"];
            eventView.CreatedByUserId = (Guid?)reader["CreatedByUserId"];
            eventView.UpdatedByUserId = (Guid?)reader["UpdatedByUserId"];
            eventView.DateCreated = (DateTime?)reader["DateCreated"];
            eventView.DateUpdated = (DateTime?)reader["DateUpdated"];

            return eventView;
        }
        private Sponsor MapSponsor(NpgsqlDataReader reader)
        {
            Sponsor sponsor = new Sponsor();
            sponsor.Id = (Guid)reader["Id"];
            sponsor.Name = (string)reader["Name"];
            sponsor.Website = (string)reader["Website"];
            sponsor.IsActive = (bool)reader["IsActive"];
            sponsor.CreatedByUserId = (Guid)reader["CreatedByUserId"];
            sponsor.UpdatedByUserId = (Guid)reader["UpdatedByUserId"];
            sponsor.DateCreated = (DateTime)reader["DateCreated"];
            sponsor.DateUpdated = (DateTime)reader["DateUpdated"];

            return sponsor;
        }
        private IPlacement MapPlacement(NpgsqlDataReader reader)
        {
            Placement placement = new Placement();
            placement.Id = (Guid?)reader["Id"];
            placement.Name = (string)reader["Name"];
            placement.FinishOrder = (int?)reader["FinishOrder"];
            placement.EventId = (Guid?)reader["EventId"];
            placement.IsActive = (bool?)reader["IsActive"];
            placement.CreatedByUserId = (Guid?)reader["CreatedByUserId"];
            placement.UpdatedByUserId = (Guid?)reader["UpdatedByUserId"];
            placement.DateCreated = (DateTime?)reader["DateCreated"];
            placement.DateUpdated = (DateTime?)reader["DateUpdated"];

            return placement;
        }
    }
}
