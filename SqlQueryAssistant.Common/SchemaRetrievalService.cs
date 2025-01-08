using Npgsql;
using Microsoft.Extensions.Configuration;

public class SchemaRetrievalService
{
    private readonly string _connectionString;
    private readonly EmbeddingService _embeddingService;
    private readonly ChatCompletionService _chatCompletionService;

    public SchemaRetrievalService(IConfiguration configuration,
                                  EmbeddingService embeddingService,
                                  ChatCompletionService chatCompletionService)
    {
        _connectionString = configuration["NeonDatabaseConnectionString"];
        _embeddingService = embeddingService;
        _chatCompletionService = chatCompletionService;
    }

    public async Task<TableSchema> GetRelevantSchemaAsync(string userQuery)
    {
        var queryEmbedding = await _embeddingService.GetEmbeddingAsync(userQuery);
        string queryEmbeddingString = string.Join(", ", queryEmbedding);
        string generatedSqlQuery = GeneratePredefinedSqlTemplate(queryEmbeddingString);

        Console.WriteLine("Generated SQL query: " + generatedSqlQuery);

        using var conn = new NpgsqlConnection(_connectionString);
        conn.Open();

        using var cmd = new NpgsqlCommand(generatedSqlQuery, conn);
        var schemaDescription = await cmd.ExecuteScalarAsync();

        return SchemaConverter.ConvertStringToSchema(schemaDescription.ToString());
    }

    public async Task<string> GenerateSqlQuery(string userQuery, TableSchema schema)
    {
        var prompt =
            "Generate an SQL query based on the following database schema and user query.\n\n" +
            "Database Schema:\n" +
            SchemaConverter.ConvertSchemaToString(schema) + "\n\n" +
            $"User Query: {userQuery}\n\n" +
            "Return only the SQL query as plain text, with no formatting, no code blocks (like ```sql), and no additional markers:";

        var generatedSqlQuery = await _chatCompletionService.GetChatCompletionAsync(prompt);
        return generatedSqlQuery;
    }

    private string GeneratePredefinedSqlTemplate(string queryEmbeddingString)
    {
        return $@"
            SELECT description
            FROM vector_data
            ORDER BY embedding <-> '[{queryEmbeddingString}]'
            LIMIT 1;";
    }

}
