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

  static Uri endpoint = new Uri(GetRequiredEnvironmentVariable("DELAWARE_AZURE_SEARCH_ENDPOINT"));

  static AzureKeyCredential credential = new AzureKeyCredential(GetRequiredEnvironmentVariable("DELAWARE_AZURE_SEARCH_ADMIN_KEY"));
  static SearchIndexClient indexClient = new SearchIndexClient(endpoint, credential);
  static SearchClient searchClient = new SearchClient(endpoint, indexName, credential);

  public static async Task Main(string[] args)
  {
    await CreateAzureSearchIndex();

    await FillAzureSearchIndexWithData();

    await SendSearchRequest("france-");

    Console.WriteLine("`france-` search done!...");

    await SendSearchRequest("tur");

    Console.WriteLine("`tur` search done!...");
  }

  private static string GetRequiredEnvironmentVariable(string name)
  {
    var value = Environment.GetEnvironmentVariable(name);

    if (string.IsNullOrWhiteSpace(value))
    {
      throw new InvalidOperationException($"Missing required environment variable: {name}");
    }

    return value;
  }

  private static async Task CreateAzureSearchIndex()
  {
    var searchIndex = new SearchIndex(indexName)
    {
      Fields = new FieldBuilder().Build(typeof(RouteInfo))
    };

    await indexClient.CreateOrUpdateIndexAsync(searchIndex);
  }

  private static async Task FillAzureSearchIndexWithData()
  {
    var json = await File.ReadAllTextAsync("data.json");
    var travelInfos = JsonSerializer.Deserialize<RouteInfo[]>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

    if (travelInfos == null)
    {
      return;
    }

    var actions = new List<IndexDocumentsAction<RouteInfo>>();

    foreach (var travelInfo in travelInfos)
    {
      actions.Add(IndexDocumentsAction.Upload(travelInfo));
    }

    await searchClient.IndexDocumentsAsync(IndexDocumentsBatch.Create(actions.ToArray()), new IndexDocumentsOptions { ThrowOnAnyError = true });
  }

  private static async Task<string> NormalizeSearchTextAsync(string text)
  {
    return text.Replace("-", "");
  }

  private static async Task<SearchResults<RouteInfo>> SendSearchRequest(string text)
  {
    var normalized = await NormalizeSearchTextAsync(text);
    var result = await searchClient.SearchAsync<RouteInfo>($"{normalized}*");
    result.Value.GetResults().ToList().ForEach(x =>
    {
      Console.WriteLine(x.Document.Name);
    });
    return result.Value;
  }
}

public class RouteInfo
{
  [SearchableField(IsKey = true, IsFilterable = true, IsSortable = true, AnalyzerName = LexicalAnalyzerName.Values.TrMicrosoft)]
  public string Id { get; set; } = string.Empty;
  [SearchableField(IsFilterable = true, AnalyzerName = LexicalAnalyzerName.Values.TrMicrosoft)]
  public string CarrierId { get; set; } = string.Empty;
  [SearchableField(IsFilterable = true, AnalyzerName = LexicalAnalyzerName.Values.TrMicrosoft)]
  public string CarrierCompanyTitle { get; set; } = string.Empty;
  [SearchableField(IsFilterable = true, AnalyzerName = LexicalAnalyzerName.Values.TrMicrosoft)]
  public string Name { get; set; } = string.Empty;
  [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.TrMicrosoft)]
  public string Description { get; set; } = string.Empty;
  [SearchableField(IsFilterable = true, IsFacetable = true, AnalyzerName = LexicalAnalyzerName.Values.TrMicrosoft)]
  public string DepartureCountry { get; set; } = string.Empty;
  [SearchableField(IsFilterable = true, AnalyzerName = LexicalAnalyzerName.Values.TrMicrosoft)]
  public string ArrivalCountry { get; set; } = string.Empty;
  [JsonPropertyName("departureCityList")]
  public List<DepartureCity> DepartureCityList { get; set; } = new List<DepartureCity>();
  [JsonPropertyName("arrivalCityList")]
  public List<ArrivalCity> ArrivalCityList { get; set; } = new List<ArrivalCity>();
  [JsonPropertyName("vehicleList")]
  public List<Vehicle> VehicleList { get; set; } = new List<Vehicle>();
  public double? CarrierReviewRate { get; set; }
}

public class DepartureCity
{
  [SearchableField(IsFilterable = true, AnalyzerName = LexicalAnalyzerName.Values.TrMicrosoft)]
  public string Name { get; set; } = string.Empty;
}

public class ArrivalCity
{
  [SearchableField(IsFilterable = true, AnalyzerName = LexicalAnalyzerName.Values.TrMicrosoft)]
  public string Name { get; set; } = string.Empty;
}

public class Vehicle
{
  public string PriceId { get; set; } = string.Empty;
  public string VehicleId { get; set; } = string.Empty;
  [SearchableField(IsFilterable = true, AnalyzerName = LexicalAnalyzerName.Values.TrMicrosoft)]
  public string VehicleTypeId { get; set; } = string.Empty;
  [SearchableField(IsFilterable = true, AnalyzerName = LexicalAnalyzerName.Values.TrMicrosoft)]
  public string BaggageTypeId { get; set; } = string.Empty;
  public int? CurrencyForEntireVehicle { get; set; }
  public int? PriceForEntireVehicle { get; set; }
  public int? CurrencyForOneDesi { get; set; }
  public int? PriceForOneDesi { get; set; }
}
