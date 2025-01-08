using Azure;
using Azure.AI.OpenAI;
using OpenAI.Chat;
using Microsoft.Extensions.Configuration;

public class ChatCompletionService
{
    private readonly AzureOpenAIClient _azureOpenAIclient;
    private readonly string _deploymentName;

    public ChatCompletionService(IConfiguration configuration)
    {
        var endpoint = new Uri(configuration["AzureOpenAIEndpoint"]);
        var apiKey = configuration["AzureOpenAIApiKey"];
        _deploymentName = configuration["AzureOpenAIChatCompletionDeploymentName"];

        _azureOpenAIclient = new AzureOpenAIClient(endpoint, new AzureKeyCredential(apiKey));
    }


    public async Task<string> GetChatCompletionAsync(string prompt)
    {
        var chatClient = _azureOpenAIclient.GetChatClient(_deploymentName);

        var completionResult = await chatClient.CompleteChatAsync(
        [
            new SystemChatMessage("You are a helpful assistant that generates SQL query."),
            new UserChatMessage(prompt),
        ]);

        string completionText = completionResult.Value.Content.First().Text.Trim();
        return completionText;
    }
}