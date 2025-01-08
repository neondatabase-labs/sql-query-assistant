using Npgsql;
using Microsoft.Extensions.Configuration;

public class SchemaService
{
    private readonly string _connectionString;

    public SchemaService(IConfiguration configuration)
    {
        _connectionString = configuration["NeonDatabaseConnectionString"];
    }

    public async Task<List<TableSchema>> GetDatabaseSchemaAsync()
    {
        var tables = new List<TableSchema>();

        await using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var getTablesQuery = @"
                SELECT table_schema, table_name
                FROM information_schema.tables
                WHERE table_schema NOT IN ('pg_catalog', 'information_schema');";

            await using var command = new NpgsqlCommand(getTablesQuery, connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var schemaName = reader.GetString(0);
                var tableName = reader.GetString(1);

                // Open a new connection for retrieving columns to avoid concurrent usage of the same connection
                var columns = await GetTableColumnsAsync(schemaName, tableName);
                tables.Add(new TableSchema { SchemaName = schemaName, TableName = tableName, Columns = columns });
            }
        }

        return tables;
    }

    private async Task<List<ColumnSchema>> GetTableColumnsAsync(string schemaName, string tableName)
    {
        var columns = new List<ColumnSchema>();

        // Create a new connection for each query
        await using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            var columnQuery = @"
                SELECT column_name, data_type
                FROM information_schema.columns
                WHERE table_schema = @schemaName AND table_name = @tableName";

            await using var command = new NpgsqlCommand(columnQuery, connection);
            command.Parameters.AddWithValue("@schemaName", schemaName);
            command.Parameters.AddWithValue("@tableName", tableName);

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                columns.Add(new ColumnSchema
                {
                    ColumnName = reader.GetString(0),
                    DataType = reader.GetString(1)
                });
            }
        }

        return columns;
    }
}

public class TableSchema
{
    public string SchemaName { get; set; }
    public string TableName { get; set; }
    public List<ColumnSchema> Columns { get; set; }
}

public class ColumnSchema
{
    public string ColumnName { get; set; }
    public string DataType { get; set; }
}
