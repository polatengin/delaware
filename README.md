# Azure Search Data Indexing

This code demonstrates how to create an _Azure Search Index_, fill it with data, and perform a search query using the _Azure Search Service_

## Prerequisites

Before running the code, ensure you have the following:

- An _Azure Search Service_ instance set up with the necessary API keys.

- A data file (`data.json`) containing the information to be indexed. Update the file path in the code (`data.json`) to match your environment.

## Code Explanation

The code is written in `C#` and demonstrates the following functionality:

- Creating an Azure Search Index.
- Filling the index with data from a `json` file.
- Performing a search query.

The `Program` class contains the main entry point and orchestrates the process.

The `CreateAzureSearchIndex` method creates or updates the _Azure Search Index_ named "_route_" by defining its fields using a `SearchIndex` object. The `FieldBuilder` class is used to build the fields based on the `RouteInfo` class properties.

The `FillAzureSearchIndexWithData` method reads the contents of the `data.json` file and deserializes it into an array of `RouteInfo` objects. It then creates a list of `IndexDocumentsAction<RouteInfo>` objects to define the indexing actions. The `IndexDocumentsBatch` is used to upload the documents in a batch to the search index using the `IndexDocuments` method of the `searchClient`.

The `SendSearchRequest` method performs a search query by calling the `SearchAsync` method of the `searchClient`. It searches for documents whose names contain the provided text parameter. The search results are printed to the console.

The `RouteInfo` class represents the structure of the documents to be indexed. It includes properties with attributes such as `SearchableField`, `IsFilterable`, `IsSortable`, `AnalyzerName`, and more, to define the behavior of the fields in the index.

The `DepartureCity` and `ArrivalCity` classes represent the departure and arrival cities within the `RouteInfo` class, respectively.

The `Vehicle` class represents a vehicle within the `RouteInfo` class, including various properties related to pricing and types.

## Running the Code

To run the code:

1. Ensure you have the .NET SDK installed.
2. Update the `key` and `endpoint` variables in the code to match your Azure Search service credentials.
3. Update the file path in the code (`data.json`) to the location of your `data.json` file.
4. Compile and execute the code.

When executed, the code will create the _Azure Search Index_, fill it with data from the JSON file, and perform a search query for documents matching the provided text parameter.

Ensure that you have the necessary permissions and credentials to access the _Azure Search Service_.
