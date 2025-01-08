using System.ClientModel;
using Azure;
using Azure.AI.OpenAI;
using OpenAI.Embeddings;
using Microsoft.Extensions.Configuration;

public class EmbeddingService
{
    private readonly AzureOpenAIClient _client;
    private readonly string _deploymentName;

    public EmbeddingService(IConfiguration configuration)
    {
        var endpoint = new Uri(configuration["AzureOpenAIEndpoint"]);
        var apiKey = configuration["AzureOpenAIApiKey"];
        _deploymentName = configuration["AzureOpenAIEmbeddingDeploymentName"];

        _client = new AzureOpenAIClient(endpoint, new AzureKeyCredential(apiKey));
    }

    public async Task<List<float>> GetEmbeddingAsync(string input)
    {
        var embeddingClient = _client.GetEmbeddingClient(_deploymentName);

        ClientResult<OpenAIEmbedding> embeddingResult = await embeddingClient.GenerateEmbeddingAsync(input);

        if (embeddingResult.Value?.ToFloats().Length > 0)
        {
            return embeddingResult.Value.ToFloats().ToArray().ToList();
        }

        throw new InvalidOperationException("No embeddings were returned.");
    }
}
