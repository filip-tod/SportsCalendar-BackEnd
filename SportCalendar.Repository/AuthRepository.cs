using Npgsql;
using SportCalendar.Common;
using SportCalendar.Model;
using System;
using System.Threading.Tasks;

namespace SportCalendar.Repository
{
    public class AuthRepository
    {
        private static string connectionString = Environment.GetEnvironmentVariable("ConnectionString");

        //This method is used to check and validate the user credentials
        public static async Task<AuthUser> ValidateUserAsync(string username, string password)
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);

            using (connection)
            {
                connection.Open();
                NpgsqlCommand command = new NpgsqlCommand();
                command.Connection = connection;

                string hashPassword = PasswordHasher.HashPassword(password);
                // Set the query with parameters
                command.CommandText = "SELECT public.\"User\".*, \"Role\".\"Access\" FROM \"User\" JOIN \"Role\" ON \"User\".\"RoleId\" = \"Role\".\"Id\" WHERE \"Username\" = @username AND \"Password\" = @password";
                command.Parameters.AddWithValue("@username", username);
                command.Parameters.AddWithValue("@password", hashPassword);

                // Execute the query
                NpgsqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    // Create a AuthUser object from the data reader
                    AuthUser user = new AuthUser()
                    {
                        // Map the properties from the data reader
                        Id = reader.GetGuid(reader.GetOrdinal("Id")),
                        Username = reader.GetString(reader.GetOrdinal("Username")),
                        Password = reader.GetString(reader.GetOrdinal("Password")),
                        Email = reader.GetString(reader.GetOrdinal("Email")),
                        Access = reader.GetString(reader.GetOrdinal("Access"))
                       
                    };

                    return user;
                }

            }

            return null;
        }
    }
}

