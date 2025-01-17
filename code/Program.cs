using LangChain.Databases;
using LangChain.Databases.Postgres;
using LangChain.DocumentLoaders;
using LangChain.Extensions;
using LangChain.Providers;
using LangChain.Providers.OpenAI;
using LangChain.Retrievers;
using System.Text;

//init llm and embedding model
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
ArgumentException.ThrowIfNullOrWhiteSpace(apiKey);

var llm = new OpenAiChatModel(apiKey, "gpt-4o-mini");
var embeddingModel = new OpenAiEmbeddingModel(apiKey, "text-embedding-3-small");

//Load recipe embeddings into vector store
var vectorCollection = await LoadRecipesToVectorStore(embeddingModel);
var vectorRetriever = vectorCollection.AsRetriever(embeddingModel);


while (true)
{
    Console.Write("You:");
    var userInput = Console.ReadLine();
    ArgumentException.ThrowIfNullOrEmpty(userInput);

    var generatedMealPlan = await GenerateMealPlan(userInput, llm, vectorRetriever);
    Console.Write($"AI: {generatedMealPlan}");

    Console.WriteLine();
    Console.WriteLine();
}

static async Task<IVectorCollection> LoadRecipesToVectorStore(OpenAiEmbeddingModel embeddingModel)
{
    //Loads the recipes from the CSV file into Documents,
    //The embedding model converts these documents into embeddings.
    //The embeddings are then saved into the Vector Store.

    var filePath = "./data/sample_recipes_v2.csv";
    var conStr = Environment.GetEnvironmentVariable("PGVECTOR_CONNECTION_STRING");
    ArgumentException.ThrowIfNullOrWhiteSpace(conStr);

    var vectorStore = new PostgresVectorDatabase(conStr);

    var vectorCollection = await vectorStore.AddDocumentsFromAsync<RecipeFileLoader>(
        embeddingModel,
        dimensions: 1536,
        dataSource: DataSource.FromPath(filePath),
        collectionName: "recipesDotNet",
    textSplitter: null,
    loaderSettings: null,
    embeddingSettings: null,
    behavior: AddDocumentsToDatabaseBehavior.OverwriteExistingCollection);
    
    return vectorCollection;
}
static async Task<string> GenerateMealPlan(string userInput, OpenAiChatModel llm, VectorStoreRetriever vectorStoreRetriever)
{
    var promptBuilder = new StringBuilder(
                        """
                        You are a a helpful cheery meal planner.Your job is to suggest meals based on the user's request and 
                        the master recipe list(also called master list), which is provided in the
                        context below.

                        Follow these steps before recommending meals:
                        1.If the user requests a meal type you're not familiar with, obtain information 
                        about the meal type before suggesting meals.
                        2.Obtain recipes from the master list that meet the user's request
                        3.If no recipes are found or the number of recipes found doesn't satisfy the user's request, look for new recipes
                        on the Internet that match the user's request. DO NOT ask the user whether they want recipes from the Internet, just complete the task.
                        4.Return a list of recipes selected in JSON with the following information for each recipe:
        
                        "recipe_name": <recipe name>
                        "description": <one sentence describing the recipe>
                        "source": <'Master list' or 'Internet' depending on where the recipe came from>
                        "ingredients": <list of ingredients>
                        "directions": <list of steps needed to make recipe>
                        "link": < Link to recipe's webpage. Do not make up a website for new recipes, include the link to the website you obtained the recipe from.>
        
                    If no recipes were found, then reply 'No matching recipes were found'. 
                    If the user's request is not relevant to a meal planner say 'Sorry, I don't have the information requested.'
                    
                    Context:
                    """);

    var relevantRecipes = await vectorStoreRetriever.GetRelevantDocumentsAsync(userInput);

    promptBuilder.AppendLine(relevantRecipes.AsString());

    var chatRequest = new ChatRequest
    {
        Messages =
        [
            new Message(promptBuilder.ToString(), MessageRole.System),
            new Message(userInput, MessageRole.Human)
        ]
    };

    var chatResponse = await llm.GenerateAsync(chatRequest);

    return chatResponse.ToString();
}