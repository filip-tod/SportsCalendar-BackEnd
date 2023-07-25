using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using SportCalendar.Common;
using SportCalendar.Model;
using SportCalendar.ModelCommon;
using SportCalendar.RepositoryCommon;

namespace SportCalendar.Repository
{
    public class SponsorRepository : ISponsorRepository
    {
        private static string connectionString = Environment.GetEnvironmentVariable("ConnectionString");

        public async Task<List<Sponsor>> SponsorGet()
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);

            try
            {
                using (connection)
                {
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    cmd.Connection = connection;
                    cmd.CommandText = $"select * from \"Sponsor\" where \"IsActive\" = true";
                    await connection.OpenAsync();
                    NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
                    List<Sponsor> sponsors = new List<Sponsor>();
                    if(reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            sponsors.Add(new Sponsor()
                            {
                                Id = (Guid)reader["Id"],
                                Name = (string)reader["Name"],
                                Website = (string)reader["Website"],
                                IsActive = (bool)reader["IsActive"],
                                UpdatedByUserId = (Guid)reader["UpdatedByUserId"],
                                CreatedByUserId = (Guid)reader["CreatedByuserId"],
                                DateCreated = (DateTime)reader["DateCreated"],
                                DateUpdated = (DateTime)reader["DateUpdated"]
                            });
                        }
                        return sponsors;
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

        public async Task<bool> SponsorPost(Sponsor sponsor)
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);

            try
            {
                using (connection)
                {
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    cmd.Connection = connection;
                    cmd.CommandText = $"insert into \"Sponsor\" (\"Id\", \"Name\", \"Website\", \"IsActive\", \"UpdatedByUserId\", \"CreatedByUserId\", \"DateCreated\", \"DateUpdated\") values (@id,@name,@website,@isactive,@updated,@created,@datecr,@dateup)";
                    await connection.OpenAsync();
                    cmd.Parameters.AddWithValue("@id", sponsor.Id);
                    cmd.Parameters.AddWithValue("@name", sponsor.Name);
                    cmd.Parameters.AddWithValue("@website", sponsor.Website);
                    cmd.Parameters.AddWithValue("@isactive", sponsor.IsActive);
                    cmd.Parameters.AddWithValue("@updated", sponsor.UpdatedByUserId);
                    cmd.Parameters.AddWithValue("@created", sponsor.CreatedByUserId);
                    cmd.Parameters.AddWithValue("@datecr", sponsor.DateCreated);
                    cmd.Parameters.AddWithValue("@dateup", sponsor.DateUpdated);
                    if (await cmd.ExecuteNonQueryAsync() > 0 )
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

        public async Task<bool> SponsorDelte(Guid id)
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);

            try
            {
                using (connection)
                {
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    cmd.Connection = connection;
                    cmd.CommandText = $"update \"Sponsor\" set \"IsActive\" = false where \"Id\"=@id";
                    await connection.OpenAsync();
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

        public async Task<bool> SponsorPut(Guid id, Sponsor sponsor)
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);

            Sponsor getSponsor = await GetSponsorById(id);

            try
            {
                using (connection)
                {
                    var queryBuilder = new StringBuilder("");
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    queryBuilder.Append($"update \"Sponsor\" set ");

                    cmd.Connection = connection;
                    await connection.OpenAsync();

                    if (String.IsNullOrEmpty(sponsor.Name))
                    {
                        cmd.Parameters.AddWithValue("@name", sponsor.Name = getSponsor.Name);
                    }
                    queryBuilder.Append($"\"Name\" = @name, ");
                    cmd.Parameters.AddWithValue("@name", sponsor.Name);
                    if (String.IsNullOrEmpty(sponsor.Website))
                    {
                        cmd.Parameters.AddWithValue("@finishOrder", getSponsor.Website);
                    }
                    queryBuilder.Append($"\"Website\" = @Website, ");
                    cmd.Parameters.AddWithValue("@Website", sponsor.Website);
                    if (sponsor.IsActive == true || false)
                    {
                        cmd.Parameters.AddWithValue("@isactive", sponsor.IsActive);
                        queryBuilder.Append($"\"IsActive\" = @isactive, ");
                    }

                    if (queryBuilder.ToString().EndsWith(", "))
                    {
                        if(queryBuilder.Length > 0)
                        {
                            queryBuilder.Remove(queryBuilder.Length - 2, 1);
                        }
                    }

                    queryBuilder.Append($" where \"Id\" =@id");
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.CommandText = queryBuilder.ToString();
                    

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

        private async Task<Sponsor> GetSponsorById(Guid id)
        {
            NpgsqlConnection connection = new NpgsqlConnection(connectionString);

            try
            {
                using (connection)
                {
                    NpgsqlCommand cmd = new NpgsqlCommand();
                    cmd.Connection = connection;
                    cmd.CommandText = $"select * from \"Sponsor\"";
                    await connection.OpenAsync();
                    NpgsqlDataReader reader = await cmd.ExecuteReaderAsync();
                    Sponsor sponsor = new Sponsor();
                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {

                            sponsor.Id = (Guid)reader["Id"];
                            sponsor.Name = (string)reader["Name"];
                            sponsor.Website = (string)reader["Website"];
                            sponsor.IsActive = (bool)reader["IsActive"];
                            sponsor.UpdatedByUserId = (Guid)reader["UpdatedByUserId"];
                            sponsor.CreatedByUserId = (Guid)reader["CreatedByuserId"];
                            sponsor.DateCreated = (DateTime)reader["DateCreated"];
                            sponsor.DateUpdated = (DateTime)reader["DateUpdated"];
                        }
                        return sponsor;
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
    }
}
