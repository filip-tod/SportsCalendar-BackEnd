using Npgsql;
using SportCalendar.Common;
using SportCalendar.Model;
using SportCalendar.RepositoryCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Results;

namespace SportCalendar.Repository
{
    public class UserRepository : IUserRepository
    {
        private static string connectionString = Environment.GetEnvironmentVariable("ConnectionString");
        public async Task<PagedList<User>> GetAllAsync(Paging paging, Sorting sorting, UserFiltering filtering)
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);

            List<User> usersList = new List<User>();

            using (connection)
            {
                connection.Open();
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = connection;

                StringBuilder selectQuery = new StringBuilder("SELECT \"User\".*, \"Role\".\"Access\" FROM public.\"User\" ");
                selectQuery.Append("JOIN public.\"Role\" ON \"User\".\"RoleId\" = \"Role\".\"Id\" ");

                StringBuilder countQuery = new StringBuilder("SELECT COUNT(\"Id\") FROM public.\"User\" ");

                StringBuilder filterQuery = new StringBuilder("WHERE 1 = 1 ");

                //adding filtering options to filterQuery                
                if (filtering != null)
                {
                    // filter by search query
                    if (filtering.SearchQuery != null)
                    {
                        filterQuery.Append($"AND (\"Username\" ILIKE @search OR \"Email\" ILIKE @search OR \"LastName\" ILIKE @search OR \"FirstName\" ILIKE @search) ");
                        command.Parameters.AddWithValue("@search", "%" + filtering.SearchQuery + "%");

                    };
                    // filtering based on date updated
                    if (filtering.FromDateUpdate != null && filtering.ToDateUpdate != null)
                    {
                        filterQuery.Append("AND \"User\".\"DateCreated\" BETWEEN @fromDate AND @toDate ");
                        command.Parameters.AddWithValue("@fromDate", filtering.FromDate);
                        command.Parameters.AddWithValue("@toDate", filtering.ToDate);
                    }
                    else
                    {
                        if (filtering.FromDateUpdate != null)
                        {
                            filterQuery.Append("AND \"User\".\"DateUpdated\" >= @fromDate ");
                            command.Parameters.AddWithValue("@fromDate", filtering.FromDate);
                        };
                        if (filtering.ToDateUpdate != default)
                        {
                            filterQuery.Append("AND \"User\".\"DateCreated\" <= @toDate ");
                            command.Parameters.AddWithValue("@toDate", filtering.ToDate);
                        };
                    };

                    // filtering based on date created

                    if (filtering.FromDate != null && filtering.ToDate != null)
                    {
                        filterQuery.Append("AND \"User\".\"DateCreated\" BETWEEN @fromDate AND @toDate ");
                        command.Parameters.AddWithValue("@fromDate", filtering.FromDate);
                        command.Parameters.AddWithValue("@toDate", filtering.ToDate);
                    }
                    else
                    {
                        if (filtering.FromDateUpdate != null)
                        {
                            filterQuery.Append("AND \"User\".\"DateUpdated\" >= @fromDate ");
                            command.Parameters.AddWithValue("@fromDate", filtering.FromDate);
                        };
                        if (filtering.ToDateUpdate != default)
                        {
                            filterQuery.Append("AND \"User\".\"DateCreated\" <= @toDate ");
                            command.Parameters.AddWithValue("@toDate", filtering.ToDate);
                        };
                    };

                    // filtering based on date created

                    if (filtering.FromDate != null && filtering.ToDate != null)
                    {
                        filterQuery.Append("AND \"User\".\"DateCreated\" BETWEEN @fromDate AND @toDate ");
                        command.Parameters.AddWithValue("@fromDate", filtering.FromDate);
                        command.Parameters.AddWithValue("@toDate", filtering.ToDate);
                    }
                    else
                    {
                        if (filtering.FromDate != null)
                        {
                            filterQuery.Append("AND \"User\".\"DateCreated\" >= @fromDate ");
                            command.Parameters.AddWithValue("@fromDate", filtering.FromDate);
                        };
                        if (filtering.ToDate != default)
                        {
                            filterQuery.Append("AND \"User\".\"DateCreated\" <= @toDate ");
                            command.Parameters.AddWithValue("@toDate", filtering.ToDate);
                        };
                    };
                }

                // getting the number of entries for submitting via PagedList
                countQuery.Append(filterQuery);
                string count = countQuery.ToString();
                command.CommandText = count;
                int entryCount = Convert.ToInt32(await command.ExecuteScalarAsync());

                //adding filtering options for selectQuery
                selectQuery.Append(filterQuery);

                //adding sorting options to selectQuery 
                if (sorting.OrderBy != null)
                {
                    selectQuery.Append($"ORDER BY \"{sorting.OrderBy}\" ");
                };
                if (sorting.SortOrder != "ASC")
                {
                    selectQuery.Append("DESC ");
                };

                // adding paging options to selectQuery
                if (paging.PageNumber != 1)
                {
                    selectQuery.Append($"OFFSET @offset ");
                    command.Parameters.AddWithValue("@offset", (paging.PageNumber - 1) * paging.PageSize);
                };
                if (paging.PageSize != 10)
                {
                    selectQuery.Append($"FETCH NEXT @next ROWS ONLY;");
                    command.Parameters.AddWithValue("@next", paging.PageSize);
                }
                else
                {
                    selectQuery.Append("FETCH NEXT @default ROWS ONLY;");
                    command.Parameters.AddWithValue("@default", paging.PageSize);
                };

                command.CommandText = selectQuery.ToString();
                NpgsqlDataReader reader = await command.ExecuteReaderAsync();


                while (await reader.ReadAsync())
                {
                    usersList.Add(

                        new User()
                        {
                            Id = (Guid)reader["Id"],
                            FirstName = (string)reader["FirstName"],
                            LastName = (string)reader["LastName"],
                            Password = (string)reader["Password"],
                            Email = (string)reader["Email"],
                            RoleId = (Guid)reader["RoleId"],
                            IsActive = (bool)reader["IsActive"],
                            UpdatedByUserId = (Guid)reader["UpdatedByUserId"],
                            DateCreated = (DateTime)reader["DateCreated"],
                            DateUpdated = (DateTime)reader["DateUpdated"],
                            Username = (string)reader["Username"]

                        });
                }
                PagedList<User> pagedUsers = new PagedList<User>()
                {
                    CurrentPage = paging.PageNumber,
                    PageSize = paging.PageSize,
                    TotalPages = (int)Math.Ceiling(entryCount / (double)paging.PageSize),
                    TotalCount = entryCount,
                    Data = usersList
                };
                return pagedUsers;
            }
            
        }

        public async Task<User> GetByUserIdAsync(Guid id)
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);

            using (connection)
            {
                connection.Open();
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = connection;

                command.CommandText = "SELECT * FROM public.\"User\" WHERE \"Id\" = @id";
                command.Parameters.AddWithValue("@id", id);

                NpgsqlDataReader reader = command.ExecuteReader();
                User user = new User();
                while (await reader.ReadAsync())
                {
                    user.Id = (Guid)reader["Id"];
                    user.FirstName = (string)reader["FirstName"];
                    user.LastName = (string)reader["LastName"];
                    user.Password = (string)reader["Password"];
                    user.Email = (string)reader["Email"];
                    user.RoleId = (Guid)reader["RoleId"];
                    user.IsActive = (bool)reader["IsActive"];
                    user.UpdatedByUserId = (Guid)reader["UpdatedByUserId"];
                    user.DateCreated = (DateTime)reader["DateCreated"];
                    user.DateUpdated = (DateTime)reader["DateUpdated"];
                    user.Username = (string)reader["Username"];


                };
                return user;
            }
        }
        public async Task<User> InsertUserAsync(User newUser)
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);

            using (connection)
            {
                connection.Open();
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = connection;

                StringBuilder insertQuery = new StringBuilder("INSERT INTO public.\"User\" VALUES ");
                insertQuery.Append("(@id, @firstname, @lastname, @password, @email, @roleid, @isactive, @updatedbyuserid, @datecreated, @dateupdated, @username)");
                command.CommandText = insertQuery.ToString();

                command.Parameters.AddWithValue("id", newUser.Id);
                command.Parameters.AddWithValue("@firstname", newUser.FirstName);
                command.Parameters.AddWithValue("@lastname", newUser.LastName);
                command.Parameters.AddWithValue("@password", newUser.Password);
                command.Parameters.AddWithValue("@email", newUser.Email);
                command.Parameters.AddWithValue("@roleid", newUser.RoleId);
                command.Parameters.AddWithValue("@isActive", newUser.IsActive);
                command.Parameters.AddWithValue("@updatedbyuserid", newUser.UpdatedByUserId);
                command.Parameters.AddWithValue("@datecreated", newUser.DateCreated);
                command.Parameters.AddWithValue("@dateupdated", newUser.DateCreated);
                command.Parameters.AddWithValue("@username", newUser.Username);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    User addedUser = new User();
                    addedUser = await GetByUserIdAsync(newUser.Id);
                    return addedUser;
                }
                return null;
            }
        }
        public async Task<User> UpdateUserAsync(Guid id, User updateUser)
        {
            User user = await GetByUserIdAsync(id);

            NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            using (connection)
            {
                connection.Open();

                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = connection;

                StringBuilder updateQuery = new StringBuilder("UPDATE public.\"User\" SET ");

                updateQuery.Append("\"UpdatedByUserId\" = @updatedbyuserid");
                command.Parameters.AddWithValue("@updatedbyuserid", updateUser.UpdatedByUserId);

                if (updateUser.FirstName != null && updateUser.FirstName != user.FirstName)
                {
                    updateQuery.Append(", \"FirstName\" = @firstname");
                    command.Parameters.AddWithValue("@firstname", updateUser.FirstName);
                }
                if (updateUser.LastName != null && updateUser.LastName != user.LastName)
                {
                    updateQuery.Append(", \"LastName\" = @lastname");
                    command.Parameters.AddWithValue("@lastname", updateUser.LastName);
                }
                if (updateUser.Password != null && updateUser.Password != user.Password)
                {
                    updateQuery.Append(", \"Password\" = @password");
                    command.Parameters.AddWithValue("@password", updateUser.Password);
                }
                if (updateUser.Email != null && updateUser.Email != user.Email)
                {
                    updateQuery.Append(", \"Email\" = @email");
                    command.Parameters.AddWithValue("@email", updateUser.Email);
                }
                if (updateUser.RoleId != Guid.Empty && updateUser.RoleId != user.RoleId)
                {
                    updateQuery.Append(", \"RoleId\" = @roleid");
                    command.Parameters.AddWithValue("@roleid", updateUser.RoleId);
                }
                if (updateUser.IsActive != false)
                {
                    updateQuery.Append(", \"IsActive\" = @isactive");
                    command.Parameters.AddWithValue("@isactive", updateUser.IsActive);
                }

                if (updateUser.Username != null && updateUser.Username != user.Username)
                {
                    updateQuery.Append(", \"Username\" = @username");
                    command.Parameters.AddWithValue("@username", updateUser.Username);
                }

                updateQuery.Append(", \"DateUpdated\" = @dateupdated ");
                command.Parameters.AddWithValue("@dateupdated", DateTime.UtcNow);

                updateQuery.Append("WHERE \"Id\" = @id");
                command.Parameters.AddWithValue("@id", id);

                string query = updateQuery.ToString();
                command.CommandText = query;

                int rowsAffected = await command.ExecuteNonQueryAsync();
                if (rowsAffected > 0)
                {
                    User updatedUser = new User();
                    updatedUser = await GetByUserIdAsync(id);
                    return updatedUser;
                }
            }

            return null;
        }

        public async Task<User> DeleteUserAsync(Guid id, User deleteUser)
        {

            NpgsqlConnection connection = new NpgsqlConnection(connectionString);

            using (connection)
            {
                connection.Open();

                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = connection;

                command.CommandText = "UPDATE public.\"User\" SET \"IsActive\" = 'false', \"UpdatedByUserId\" = @updatedby, \"DateUpdated\" = @dateupdated WHERE \"Id\" = @id";
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@updatedby", deleteUser.UpdatedByUserId);
                command.Parameters.AddWithValue("@dateupdated", deleteUser.DateUpdated);

                int rowsAffected = await command.ExecuteNonQueryAsync();
                if (rowsAffected > 0)
                {
                    User deletedUser = await GetByUserIdAsync(id);
                    return deletedUser;
                }
            }

            return null;
        }
        public async Task<bool> CheckEntryByUserIdAsync(Guid id)
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);

            try
            {
                using (connection)
                {
                    connection.Open();

                    NpgsqlCommand command = new NpgsqlCommand();
                    command.Connection = connection;

                    command.CommandText = "SELECT * FROM public.\"User\" WHERE \"Id\" = @id";
                    command.Parameters.AddWithValue("@id", id);

                    NpgsqlDataReader reader = command.ExecuteReader();

                    return reader.HasRows;
                }
            }
            catch
            { return false; }
        }
    }
}
