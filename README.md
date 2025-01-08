# Intelligent SQL Query Assistant

This project demonstrates how to build an **Intelligent SQL Query Assistant** that enables users to interact with their **Neon** database using natural language. The assistant uses **Azure OpenAI**, **pgvector** extension in Neon, and **Retrieval-Augmented Generation (RAG)** approach to dynamically generate and execute SQL queries. Read more on how to guide blog post.

![SQL Query Assistant in Azure](/assets/neon-sql-query-azure-diagram.png)

## Features

- **Natural Language Querying**: Ask questions in plain English, and the assistant translates them into SQL queries.
- **Vector-Based Schema Matching**: Uses embeddings and Neon's `pgvector` extension to find the most relevant database schema.
- **Customizable APIs**:
    - **SchemaTraining API**: Extracts the database schema, generates embeddings, and stores them in Neon.
    - **QueryAssistant API**: Processes user queries, matches the schema, generates SQL, and executes it.
- **Azure OpenAI Integration**: Leverages GPT models like `gpt-4` and `text-embedding-ada-002`.

---

## Technologies Used

- **.NET Core**: Backend logic and API development.
- **Azure Functions**: Serverless platform for API hosting.
- **Neon**: Vector storage using the [pgvector](https://neon.tech/docs/extensions/pgvector) extension.
- **Azure OpenAI**: For generating embeddings and SQL queries.
- **Azure AI Foundry**: For deploying and managing AI models.

---

## Getting Started

### Prerequisites

Before we begin, make sure you have the following:

1. **Install Tools**
    - .[NET Core SDK](https://dotnet.microsoft.com/en-us/download/dotnet)
    - [Azure Functions Core Tools](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local?tabs=macos%2Cisolated-process%2Cnode-v4%2Cpython-v2%2Chttp-trigger%2Ccontainer-apps&pivots=programming-language-csharp) installed
    - A free [Neon account](https://console.neon.tech/signup)
    - An [Azure account](https://azure.microsoft.com/free/) with an active subscription
2. **Create Neon Project**
    - Sign up for [Neon](https://neon.tech/).
    - Create the database tables
3. **Azure OpenAI Resource**
    - Create an Azure OpenAI resource.
    - Deploy models: `gpt-4` and `text-embedding-ada-002`.

---

### Project Structure

```
SqlQueryAssistant
│   SqlQueryAssistant.sln
|
├───SqlQueryAssistant.Common
│   │   ChatCompletionService.cs
│	  │		EmbeddingService.cs
│   │   SchemaService.cs
│   │   SchemaConverter.cs
│   │   SqlExecutorService.cs
│   │   SchemaRetrievalService.cs
│   │   VectorStorageService.cs
│   └───SqlQueryAssistant.Common.csproj
│
├───SqlQueryAssistant.Data
|   |   customers.sql
|   |   schema.sql
└───SqlQueryAssistant.Functions
    │   host.json
		│   local.settings.json
		│   QueryAssistantFunction.cs
    └───SqlQueryAssistant.Functions.csproj
```

## Installation

### Clone the Repository

```bash
git clone <https://github.com/your-repo/sql-query-assistant.git>
cd sql-query-assistant
```

### Set Up Configuration

Create a `local.settings.json` file in the `SqlQueryAssistant.Functions` project:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "AzureOpenAI__Endpoint": "https://your-azure-openai-endpoint/",
    "AzureOpenAI__ApiKey": "your-api-key",
    "AzureOpenAI__EmbeddingDeploymentName": "text-embedding-ada-002",
    "NeonDatabaseConnectionString": "Host=your-neon-host;Database=your-database;Username=your-username;Password=your-password;SSL Mode=Require;Trust Server Certificate=true"
  }
}
```

### Install Dependencies

```bash
dotnet restore build
```

### Run the Project Locally

```bash
func start
```

## Usage

### **SchemaTraining API**

- **Endpoint**: `http://localhost:7071/api/schema-training`
- **Method**: `POST`
- **Description**: Trains the assistant by extracting the schema from Neon, generating embeddings, and storing them in the database.

### Test with cURL:

```bash
curl -X POST http://localhost:7071/api/schema-training
```

---

### **QueryAssistant API**

- **Endpoint**: `http://localhost:7071/api/query-assistant`
- **Method**: `POST`
- **Description**: Processes user queries, retrieves the relevant schema, generates SQL, and returns the query result.

### Test with cURL:

```bash
curl -X POST http://localhost:7071/api/query-assistant \
     -H "Content-Type: application/json" \
     -d '{"Query": "List all customers who signed up after 2022-01-01."}'
```