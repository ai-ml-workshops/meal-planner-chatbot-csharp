using CsvHelper;
using CsvHelper.Configuration;
using LangChain.DocumentLoaders;
using System.Globalization;

/// <summary>
/// Loads recipes from a CSV file and converts them to a collection of Documents to load into the Vector Store.
/// 
/// note: This doesn't need to be generalized like the Python CSVLoader class.
/// it is specifically for loading our recipes CSV file.
/// </summary>
public class RecipeFileLoader : IDocumentLoader
{
    public async Task<IReadOnlyCollection<Document>> LoadAsync(DataSource dataSource, DocumentLoaderSettings? settings = null, CancellationToken cancellationToken = default)
    {
        if (dataSource.Type != DataSourceType.Path)
        {
            throw new ArgumentException("CsvFileLoader only works with local files");
        }


        var csvReaderConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            PrepareHeaderForMatch = args => args.Header.ToLower() //case-insensitive header names
        };

        using var sr = new StreamReader(dataSource.Value!);
        using var csvReader = new CsvReader(sr, csvReaderConfig);

        List<Document> documents = [];

        await foreach (var recipe in csvReader.GetRecordsAsync<Recipe>())
        {
            var document = new Document(recipe.Details, new Dictionary<string, object>
            {
                [nameof(recipe.Id)] = recipe.Id,
                [nameof(recipe.Link)] = recipe.Link,
                [nameof(recipe.Name)] = recipe.Name
            });

            documents.Add(document);
        }

        return documents.AsReadOnly();
    }
}