using Npgsql;
using Microsoft.Extensions.Configuration;

public class VectorStorageService
{
    private readonly string _connectionString;
    private readonly EmbeddingService _embeddingService;

    public VectorStorageService(IConfiguration configuration, EmbeddingService embeddingService)
    {
        _connectionString = configuration["NeonDatabaseConnectionString"];
        _embeddingService = embeddingService;
    }

    public async Task StoreSchemaInVectorDb(List<TableSchema> schemas)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        foreach (var schema in schemas)
        {
            var schemaString = SchemaConverter.ConvertSchemaToString(schema);
            var embedding = await _embeddingService.GetEmbeddingAsync(schemaString);

            using var cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "INSERT INTO vector_data (description, embedding) VALUES (@description, @embedding)";
            cmd.Parameters.AddWithValue("description", schemaString);
            cmd.Parameters.AddWithValue("embedding", embedding.ToArray());
            cmd.ExecuteNonQuery();
        }
    }
}
