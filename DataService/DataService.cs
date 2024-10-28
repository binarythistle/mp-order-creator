using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Microsoft.Extensions.Configuration;

namespace NestedPaginationDotNet.DataServices;

public class DataService
{
    

    public async Task<GraphQLResponse<T>> GetDataAsync<T>(
        GraphQLRequest request, 
        string endPoint,
        string key,
        string basicAuthID = "",
        string basicAuthPassword = ""
        )
    {
        var graphQLClient = new GraphQLHttpClient(endPoint, new NewtonsoftJsonSerializer());
        graphQLClient.HttpClient.DefaultRequestHeaders.Add("marketplacer-api-key", key);
        if (basicAuthID != "")
        {
            graphQLClient.HttpClient.DefaultRequestHeaders.Add("authorization", Conversion.GenerateBasicAuthHeader(basicAuthID, basicAuthPassword));
        }

        var graphQLResponse = new GraphQLResponse<T>();

        try
        {
            graphQLResponse = await graphQLClient.SendQueryAsync<T>(request);
            //ConsoleWriter.PrintColorMessage($"---> {this.GetType().Name}: Call got something generic...", ConsoleColor.Green);
            
        }
        catch (Exception ex)
        {
            ConsoleWriter.PrintColorMessage($"---> {this.GetType().Name}: Exception: {ex.Message}", ConsoleColor.Red);   
        }

        return graphQLResponse;
    }
}