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

       public async Task<List<Dictionary<string, object>>> getItemsListed()
        {
            var items = new List<Dictionary<string, object>>();
            try
            {
                    await using var dataSource = await ConnectDatabase();
 
                    await using var command = dataSource.CreateCommand("SELECT * FROM items");
                    await using var reader = await command.ExecuteReaderAsync();
                    
                    

                    while (await reader.ReadAsync())
                    {
                        var item = new Dictionary<string, object>
                        {
                            ["stringId"] = reader.GetGuid(0),
                            ["name"] = reader.GetString(1),
                            ["sellerId"] = reader.GetGuid(2),
                            ["dateAdded"] = reader.GetDateTime(3),
                            ["price"] = reader.GetDouble(4),
                            ["imageLocation"] = reader.GetString(5),
                            ["category"] = reader.GetString(6),
                            ["stock"] = reader.GetInt32(7)
                        };
						
                        items.Add(item);
                    }
			

                    return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to connect to the database: {ex.Message}");
                return items;
            }
        }
    }
}