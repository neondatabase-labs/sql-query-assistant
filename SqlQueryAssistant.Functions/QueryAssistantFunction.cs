using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

public class QueryAssistantFunction
{
    private readonly ILogger _logger;
    private readonly SchemaRetrievalService _schemaRetrievalService;
    private readonly SqlExecutorService _sqlExecutorService;
    private readonly SchemaService _schemaService;
    private readonly VectorStorageService _vectorStorageService;

    public QueryAssistantFunction(
        ILoggerFactory loggerFactory,
        SchemaRetrievalService schemaRetrievalService,
        SqlExecutorService sqlExecutorService,
        SchemaService schemaService,
        VectorStorageService vectorStorageService)
    {
        _logger = loggerFactory.CreateLogger<QueryAssistantFunction>();
        _schemaRetrievalService = schemaRetrievalService;
        _sqlExecutorService = sqlExecutorService;
        _schemaService = schemaService;
        _vectorStorageService = vectorStorageService;
    }

    [Function("QueryAssistant")]
    public async Task<HttpResponseData> QueryAsync(
    [HttpTrigger(AuthorizationLevel.Function, "post", Route = "query-assistant")] HttpRequestData req)
    {
        _logger.LogInformation("Received a request to Query Assistant.");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var userQuery = JsonConvert.DeserializeObject<UserQuery>(requestBody);

        var schema = await _schemaRetrievalService.GetRelevantSchemaAsync(userQuery.Query);
        var generatedSqlQuery = await _schemaRetrievalService.GenerateSqlQuery(userQuery.Query, schema);
        var result = await _sqlExecutorService.ExecuteQueryAsync(generatedSqlQuery);

        var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        await response.WriteStringAsync(JsonConvert.SerializeObject(result));
        return response;
    }

    [Function("SchemaTraining")]
    public async Task<HttpResponseData> TrainSchemaAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "schema-training")] HttpRequestData req)
    {
        _logger.LogInformation("Received a request to train schema.");

        var schemas = await _schemaService.GetDatabaseSchemaAsync();
        await _vectorStorageService.StoreSchemaInVectorDb(schemas);

        var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
        await response.WriteStringAsync("Schema training completed successfully.");
        return response;
    }
}

public class UserQuery
{
    public string Query { get; set; }
}