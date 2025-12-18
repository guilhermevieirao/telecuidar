using Microsoft.Data.Sqlite;

if (args.Length < 2)
{
    Console.WriteLine("Usage: dotnet run <database-path> <user-id>");
    return 1;
}

var dbPath = args[0];
var userId = args[1];

try
{
    using var connection = new SqliteConnection($"Data Source={dbPath}");
    connection.Open();
    
    var command = connection.CreateCommand();
    command.CommandText = "UPDATE Users SET Role = 1 WHERE Id = @userId";
    command.Parameters.AddWithValue("@userId", userId);
    
    var rows = command.ExecuteNonQuery();
    
    if (rows > 0)
    {
        Console.WriteLine($"Successfully updated user {userId} to ADMIN role");
        return 0;
    }
    else
    {
        Console.WriteLine($"User {userId} not found");
        return 1;
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    return 1;
}
