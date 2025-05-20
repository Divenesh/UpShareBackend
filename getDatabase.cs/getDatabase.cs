using Npgsql;

namespace MyProject.Database
{
    public class GetDatabase
    {
        private string connectionString = "Host=localhost;Port=5433;Username=postgres;Password=123456;Database=postgres";

        private NpgsqlDataSource ConnectDatabase()
        {
            var dataSource = NpgsqlDataSource.Create(connectionString);
            return dataSource;
        }

        public async Task<List<Dictionary<string, object>>> getItemsListed()
        {
            var items = new List<Dictionary<string, object>>();
            try
            {
                await using var dataSource = ConnectDatabase();

                await using var command = dataSource.CreateCommand("SELECT * FROM items");
                await using var reader = await command.ExecuteReaderAsync();



                while (await reader.ReadAsync())
                {
                    var item = new Dictionary<string, object>
                    {
                        ["id"] = reader.GetGuid(0),
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
        public async Task<List<Dictionary<string, object>>> getSellers()
        {
            var seller = new List<Dictionary<string, object>>();
            try
            {
                await using var dataSource = ConnectDatabase();

                await using var command = dataSource.CreateCommand("SELECT * FROM seller");
                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var item = new Dictionary<string, object>
                    {
                        ["id"] = reader.GetGuid(0),
                        ["sellername"] = reader.GetString(1),
                        ["regnum"] = reader.GetString(2),
                        ["address"] = reader.GetString(3),
                        ["contactnum"] = reader.GetInt64(5),
                    };

                    seller.Add(item);
                }
                return seller;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to connect to the database: {ex.Message}");
                return seller;
            }
        }
        public async Task<List<Dictionary<string, object>>> getRatings()
        {
            var ratings = new List<Dictionary<string, object>>();
            try
            {
                await using var dataSource = ConnectDatabase();

                await using var command = dataSource.CreateCommand("SELECT * FROM rating");
                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var item = new Dictionary<string, object>
                    {
                        ["id"] = reader.GetGuid(0),
                        ["itemid"] = reader.GetGuid(1),
                        ["description"] = reader.GetString(2),
                    };

                    ratings.Add(item);
                }
                return ratings;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to connect to the database: {ex.Message}");
                return ratings;
            }
        }
        public async Task<List<Dictionary<string, object>>> getSpecification()
        {
            var specifications = new List<Dictionary<string, object>>();
            try
            {
                await using var dataSource = ConnectDatabase();

                await using var command = dataSource.CreateCommand("SELECT * FROM specification");
                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var item = new Dictionary<string, object>
                    {
                        ["id"] = reader.GetGuid(0),
                        ["itemid"] = reader.GetGuid(1),
                        ["details"] = reader.GetString(2),
                    };

                    specifications.Add(item);
                }
                return specifications;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to connect to the database: {ex.Message}");
                return specifications;
            }


    }
    }
}