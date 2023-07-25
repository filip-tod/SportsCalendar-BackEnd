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
    public class RoleRepository : IRoleRepository
    {
        private static string connectionString = Environment.GetEnvironmentVariable("ConnectionString");
        public async Task<List<Role>> GetAllAsync(Sorting sorting, BaseFiltering filtering)
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);
            List<Role> rolesList = new List<Role>();

            using (connection)
            {
                connection.Open();
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = connection;

                StringBuilder selectQuery = new StringBuilder("SELECT * FROM public.\"Role\" ");

                //adding filtering options to selectQuery                
                if (filtering != null)
                {
                    // filter by search query
                    if (filtering.SearchQuery != null)
                    {
                        selectQuery.Append($"WHERE (\"Access\" ILIKE @search) ");
                        command.Parameters.AddWithValue("@search", "%" + filtering.SearchQuery + "%");
                    };
                }

                //adding sorting options to selectQuery 
                if (sorting.OrderBy != null)
                {
                    selectQuery.Append($"ORDER BY \"{sorting.OrderBy}\" ");
                };
                if (sorting.SortOrder != "ASC")
                {
                    selectQuery.Append("DESC ");
                };

                command.CommandText = selectQuery.ToString();

                NpgsqlDataReader reader = await command.ExecuteReaderAsync();

                while (reader.Read())
                {
                    rolesList.Add(
                        new Role()
                        {
                            Id = (Guid)reader["Id"],
                            Access = (string)reader["Access"],
                            IsActive = (bool)reader["IsActive"],
                            CreatedByUser = (Guid)reader["CreatedByUserID"],
                            UpdatedByUser = (Guid)reader["UpdatedByUserID"],
                            DateCreated = (DateTime)reader["DateCreated"],
                            DateUpdated = (DateTime)reader["DateUpdated"]
                        }
                        );                    
                }
                return rolesList;
            }

        }
    }
}
