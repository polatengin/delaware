using System.Text.Json;
using System.Text.Json.Serialization;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;

public class Program
{
  const string indexName = "route";
  const string key = "<AzureSearchAPIKey>";

  static Uri endpoint = new Uri("https://<azure_search>.search.windows.net");

  static AzureKeyCredential credential = new AzureKeyCredential(key);
  static SearchIndexClient indexClient = new SearchIndexClient(endpoint, credential);
  static SearchClient searchClient = new SearchClient(endpoint, indexName, credential);

  public static void Main(string[] args)
  {
    MainAsync(args).GetAwaiter().GetResult();

    Console.WriteLine("done!...");
  }

  private static async Task MainAsync(string[] args)
  {
  }
}
