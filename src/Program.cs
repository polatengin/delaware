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

  private static void CreateAzureSearchIndex()
  {
    var searchIndex = new SearchIndex(indexName)
    {
      Fields = new FieldBuilder().Build(typeof(RouteInfo))
    };

    var index = indexClient.CreateOrUpdateIndex(searchIndex).Value;
  }
}

public class RouteInfo
{
  [SearchableField(IsKey = true, IsFilterable = true, IsSortable = true, AnalyzerName = LexicalAnalyzerName.Values.TrMicrosoft)] public string Id { get; set; } = string.Empty;
  [SearchableField(IsFilterable = true, AnalyzerName = LexicalAnalyzerName.Values.TrMicrosoft)] public string CarrierId { get; set; } = string.Empty;
  [SearchableField(IsFilterable = true, AnalyzerName = LexicalAnalyzerName.Values.TrMicrosoft)] public string CarrierCompanyTitle { get; set; } = string.Empty; public double? CarrierReviewRate { get; set; }
  [SearchableField(IsFilterable = true, AnalyzerName = LexicalAnalyzerName.Values.TrMicrosoft)] public string Name { get; set; } = string.Empty;
  [SearchableField(IsFilterable = true, IsFacetable = true, AnalyzerName = LexicalAnalyzerName.Values.TrMicrosoft)] public string DepartureCountry { get; set; } = string.Empty;
  [SearchableField(IsFilterable = true, AnalyzerName = LexicalAnalyzerName.Values.TrMicrosoft)] public string ArrivalCountry { get; set; } = string.Empty;
  [JsonPropertyName("departureCityList")] public List<DepartureCity> DepartureCityList { get; set; } = new List<DepartureCity>();
  [JsonPropertyName("arrivalCityList")] public List<ArrivalCity> ArrivalCityList { get; set; } = new List<ArrivalCity>();
  [JsonPropertyName("vehicleList")] public List<Vehicle> VehicleList { get; set; } = new List<Vehicle>();
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
