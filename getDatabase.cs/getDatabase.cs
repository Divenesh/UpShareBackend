using System;
using System.Threading.Tasks;
using Npgsql;

namespace MyProject.Database
{
    public class GetDatabase
    {
        private string connectionString = "Host=localhost;Port=5433;Username=postgres;Password=123456;Database=postgres";

        private async Task<NpgsqlDataSource> ConnectDatabase()
        {
            var dataSource = NpgsqlDataSource.Create(connectionString);
            return dataSource;
        }

        public async Task TestDatabase()
        {
            try
            {
                await using var dataSource = await ConnectDatabase();

                await using var command = dataSource.CreateCommand("SELECT * FROM users");
                await using var reader = await command.ExecuteReaderAsync();
                
                while (await reader.ReadAsync())
                {
                    int id = reader.GetInt32(0);
                    string name = reader.GetString(1);
                    Console.WriteLine($"ID: {id}, Name: {name}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to connect to the database: {ex.Message}");
            }
        }
    }
}