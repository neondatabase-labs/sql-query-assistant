using Npgsql;
using Microsoft.Extensions.Configuration;

public class SqlExecutorService
{
    private readonly string _connectionString;

    public SqlExecutorService(IConfiguration configuration)
    {
        _connectionString = configuration["NeonDatabaseConnectionString"];
    }

    public async Task<List<Dictionary<string, object>>> ExecuteQueryAsync(string sqlQuery)
    {
        var result = new List<Dictionary<string, object>>();

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            using var command = new NpgsqlCommand(sqlQuery, connection);
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row[reader.GetName(i)] = reader.GetValue(i);
                }
                result.Add(row);
            }
        }

        return result;
    }
}
