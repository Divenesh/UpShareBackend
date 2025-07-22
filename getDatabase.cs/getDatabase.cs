using Npgsql;

namespace MyProject.Database
{
    public class GetDatabase
    {
        private string connectionString =
            "Host=localhost;Port=5433;Username=postgres;Password=123456;Database=postgres";

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
                        ["stock"] = reader.GetInt32(7),
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
                        ["contactnum"] = reader.GetString(5),
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

        public async Task<Dictionary<string, object>?> getUser(string userId)
        {
            try
            {
                await using var dataSource = ConnectDatabase();

                await using var command = dataSource.CreateCommand(
                    $"SELECT * FROM users WHERE id = '{userId}'"
                );
                await using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    return new Dictionary<string, object>
                    {
                        ["id"] = reader.GetGuid(0),
                        ["firstname"] = reader.GetString(1),
                        ["lastname"] = reader.GetString(2),
                        ["email"] = reader.GetString(3),
                        ["profilePicture"] = reader.GetString(4),
                        ["dateJoined"] = reader.GetDateTime(5),
                        ["address"] = reader.GetString(6),
                        ["city"] = reader.GetString(7),
                        ["state"] = reader.GetString(8),
                        ["country"] = reader.GetString(9),
                        ["phoneNumber"] = reader.GetString(10),
                    };
                }
                Console.WriteLine($"User with ID {userId} not found.");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to connect to the database: {ex.Message}");
                return null;
            }
        }

        public async Task<Dictionary<string, object>?> createUser(object user)
        {
            try
            {
                await using var dataSource = ConnectDatabase();

                var userDict = user as Dictionary<string, object>;
                if (userDict == null)
                {
                    Console.WriteLine("Invalid user data format.");
                    return null;
                }

                var commandText =
                    @"
                    INSERT INTO users (id , firstname, lastname, email, profilePictureurl, dateJoined, address, city, state, country, phoneNumber)
                    VALUES (@id, @firstname, @lastname, @email, @profilePicture, @dateJoined, @address, @city, @state, @country, @phoneNumber)
                    RETURNING id";

                await using var command = dataSource.CreateCommand(commandText);
                // With this code:
                Guid userId;
                if (Guid.TryParse(userDict["id"].ToString(), out userId))
                {
                    command.Parameters.AddWithValue("id", userId);
                }
                else
                {
                    Console.WriteLine($"Invalid UUID format. Generated new ID: {userId}");
                }
                command.Parameters.AddWithValue(
                    "firstname",
                    ConvertToString(userDict["firstname"])
                );
                command.Parameters.AddWithValue("lastname", ConvertToString(userDict["lastname"]));
                command.Parameters.AddWithValue("email", ConvertToString(userDict["email"]));
                command.Parameters.AddWithValue(
                    "profilePicture",
                    ConvertToString(userDict["profilePicture"])
                );
                command.Parameters.AddWithValue("dateJoined", DateTime.UtcNow);
                command.Parameters.AddWithValue("address", ConvertToString(userDict["address"]));
                command.Parameters.AddWithValue("city", ConvertToString(userDict["city"]));
                command.Parameters.AddWithValue("state", ConvertToString(userDict["state"]));
                command.Parameters.AddWithValue("country", ConvertToString(userDict["country"]));
                command.Parameters.AddWithValue(
                    "phoneNumber",
                    ConvertToString(userDict["phoneNumber"])
                );

                var newUserId = await command.ExecuteScalarAsync();
                Console.WriteLine($"User created with ID: {newUserId}");
                return new Dictionary<string, object> { ["id"] = newUserId };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create user: {ex.Message}");
                return null;
            }
        }

        public async Task<Dictionary<string, object>?> updateUser(object user)
        {
            try
            {
                await using var dataSource = ConnectDatabase();

                var userDict = user as Dictionary<string, object>;
                if (userDict == null)
                {
                    Console.WriteLine("Invalid user data format.");
                    return null;
                }

                var commandText =
                    @"  UPDATE users 
                        SET firstname = @firstname, 
                            lastname = @lastname, 
                            email = @email, 
                            profilePictureurl = @profilePicture, 
                            dateJoined = @dateJoined, 
                            address = @address, 
                            city = @city, 
                            state = @state, 
                            country = @country, 
                            phoneNumber = @phoneNumber
                            WHERE id = @id";

                await using var command = dataSource.CreateCommand(commandText);
                Guid userId;
                if (Guid.TryParse(userDict["id"].ToString(), out userId))
                {
                    command.Parameters.AddWithValue("id", userId);
                }
                else
                {
                    Console.WriteLine($"Invalid UUID format. Generated new ID: {userId}");
                }

                command.Parameters.AddWithValue(
                    "firstname",
                    ConvertToString(userDict["firstname"])
                );
                command.Parameters.AddWithValue("lastname", ConvertToString(userDict["lastname"]));
                command.Parameters.AddWithValue("email", ConvertToString(userDict["email"]));
                command.Parameters.AddWithValue(
                    "profilePicture",
                    ConvertToString(userDict["profilePicture"])
                );
                command.Parameters.AddWithValue("dateJoined", DateTime.UtcNow);
                command.Parameters.AddWithValue("address", ConvertToString(userDict["address"]));
                command.Parameters.AddWithValue("city", ConvertToString(userDict["city"]));
                command.Parameters.AddWithValue("state", ConvertToString(userDict["state"]));
                command.Parameters.AddWithValue("country", ConvertToString(userDict["country"]));
                command.Parameters.AddWithValue(
                    "phoneNumber",
                    ConvertToString(userDict["phoneNumber"])
                );

                var newUserId = await command.ExecuteScalarAsync();
                return new Dictionary<string, object> { ["id"] = newUserId };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to update user: {ex.Message}");
                return null;
            }
        }

        private string ConvertToString(object value)
        {
            if (value is System.Text.Json.JsonElement jsonElement)
            {
                return jsonElement.GetString() ?? jsonElement.ToString();
            }
            return value?.ToString() ?? string.Empty;
        }
    }
}
