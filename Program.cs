using GraphQL;
using Microsoft.Extensions.Configuration;
using NestedPaginationDotNet.DataServices;

IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddUserSecrets(typeof(Program).Assembly, optional: true)
    .Build();

var verticals = configuration.GetSection("verticals").Get<List<Vertical>>();

if (verticals == null)
{
    Console.WriteLine("You don't appear to have any Marketplacer instances configured.");
    Console.WriteLine("Please refer to the README file on how to do this..");
    Console.WriteLine("Exiting...");
    Environment.Exit(0);
}


Console.WriteLine("Select the vertical ID you want to create orders on: ");
foreach (var vertical in verticals)
{
    Console.WriteLine($"-> ID: {vertical.Id} - {vertical.Name}");
}

int verticalId = 0;
try
{
    verticalId = Convert.ToInt32(Console.ReadLine());
}
catch (Exception ex)
{
    Console.WriteLine($"-> {ex.Message}");
    Console.WriteLine("Exiting...");
    Environment.Exit(0);
}

Vertical configVertical = verticals.Find(obj => obj.Id == verticalId)!;

if (configVertical == null)
{
    ConsoleWriter.PrintColorMessage("--> The vertical id you selected does not appear to be valid.", ConsoleColor.DarkRed);
    Console.WriteLine("Exiting...");
    Environment.Exit(0);
}

ConsoleWriter.PrintColorMessage($"-> Endpoint: {configVertical.EndPoint}", ConsoleColor.Cyan);

if ((!configVertical.EndPoint.ToLower().Contains("staging"))
    && (!configVertical.EndPoint.ToLower().Contains("test"))
    && (!configVertical.EndPoint.ToLower().Contains("sandbox")))
{
    Console.WriteLine("");
    ConsoleWriter.PrintColorMessage("!!! WARNING !!!", ConsoleColor.Red);
    Console.WriteLine("");
    ConsoleWriter.PrintColorMessage("THIS APPEARS TO BE A PRODUCTION INSTANCE", ConsoleColor.Red);
    ConsoleWriter.PrintColorMessage("PROCEEDING FURTHER WILL RESULT IN THE CREATION OF ORDERS", ConsoleColor.Red);
    ConsoleWriter.PrintColorMessage("ARE YOU SURE YOU WANT TO USE IT - (TYPE 'YES')", ConsoleColor.Red);


    var checkProduction = Console.ReadLine();
    if (checkProduction!.ToLower() != "yes")
    {
        Console.WriteLine("Exiting...");
        Environment.Exit(0);
    }
}



Console.ForegroundColor = ConsoleColor.Blue;
Console.WriteLine("Getting all Sellers on that vertical...");
Console.ResetColor();


bool hasNextPage = true;
DataService sellerDataService = new();

var sellers = new List<SellerNode>();

var sellerRequest = new GraphQLRequest
{
    Query = GraphQLQueryLibrary.AllSellerQuery,
    OperationName = "getAllActiveSellers",
    Variables = new
    {
        pageSize = 50,
        endCursor = string.Empty
    }
};

do
{
    var graphQLResponse = await sellerDataService.GetDataAsync<SellerReadDto>(sellerRequest, configVertical.EndPoint, configVertical.Key, configVertical.BasicAuthId, configVertical.BasicAuthPassword);
    if (graphQLResponse.Data == null)
    {
        ConsoleWriter.PrintColorMessage("--> We have no seller data", ConsoleColor.Yellow);
        Environment.Exit(1);
    }


    if (graphQLResponse.Errors != null)
        ConsoleWriter.PrintColorMessage("--> Errors (Sellers) - process further to see what we do next", ConsoleColor.Red);

    foreach (var seller in graphQLResponse.Data.Sellers!.SellerNodes!)
    {
        sellers.Add(seller);
    }

    sellerRequest.Variables = new
    {
        paggSize = 50,
        endCursor = graphQLResponse.Data.Sellers!.PageInfo.EndCursor
    };

    if (!graphQLResponse.Data.Sellers!.PageInfo.HasNextPage)
    {
        hasNextPage = false;
        Console.WriteLine("---> hasNextPage set to false we should exit paging loop");
    }

} while (hasNextPage);

foreach (var seller in sellers)
{
    Console.WriteLine($"-> ID: {seller.LegacyId} - {seller.BusinessName} - [{seller.Id}]");
}

Console.WriteLine("Select the seller ID you want to create orders for: ");



var sellerId = Console.ReadLine()!;
Console.WriteLine($"--> Seller ID: {sellerId}");

SellerNode configSeller = sellers.Find(obj => obj.LegacyId.ToString() == sellerId)!;

if (configSeller == null)
{
    ConsoleWriter.PrintColorMessage("--> The seller id you selected does not appear to be valid.", ConsoleColor.DarkRed);
    Console.WriteLine("Exiting...");
    Environment.Exit(0);
}
Console.Write("--> Fetching first 100 buyable products for: ");
ConsoleWriter.PrintColorMessage($"{configSeller.BusinessName}", ConsoleColor.Cyan);

// Deliberately not paginating round the full result set, we only need 100 products to create orders
// Retrieving all products would be wasteful

DataService productDataService = new();

var products = new List<ProductNode>();
var variants = new List<VariantNode>();

var productRequest = new GraphQLRequest
{
    Query = GraphQLQueryLibrary.PurchaseableSellerProducts,
    OperationName = "getPurchaseableProducts",
    Variables = new
    {
        pageSize = 10,
        endCursor = string.Empty,
        sellerIds = configSeller.Id
        //sellerIds = $"[\"{configSeller.Id}\"]"

    }
};

Console.WriteLine(productRequest.Variables);

var gqlProductResponse = await productDataService.GetDataAsync<ProductReadDto>(productRequest, configVertical.EndPoint, configVertical.Key, configVertical.BasicAuthId, configVertical.BasicAuthPassword);
if (gqlProductResponse.Data == null)
{
    ConsoleWriter.PrintColorMessage("--> We have no product data", ConsoleColor.Yellow);
    Environment.Exit(1);
}


if (gqlProductResponse.Errors != null)
    ConsoleWriter.PrintColorMessage("--> Errors (products) - process further to see what we do next", ConsoleColor.Red);

foreach (var product in gqlProductResponse.Data.AdvertsWhere!.ProductNodes!)
{
    products.Add(product);
    Console.WriteLine($"{product.LegacyId} - {product.Title} ({product.Variants!.VariantNodes!.Count()})");
    foreach (var variant in product.Variants!.VariantNodes!)
    {
        variants.Add(variant);
    }
}

Console.WriteLine($"{products.Count} products returned.");
Console.WriteLine($"{variants.Count} variants returned.");

DataService orderDataService = new();

int numOrders = 20;

for (int i = 0; i < numOrders; i++)
{
    Random random = new Random();
    int randomVariantIndex = random.Next(0, variants.Count);
    var randomVariant = variants[randomVariantIndex];

    var orderRequest = new GraphQLRequest
    {
        Query = GraphQLQueryLibrary.OrderCreate,
        OperationName = "simpleOrderCreate",
        Variables = new
        {
            variantId = randomVariant.Id,
            amount = randomVariant.LowestPriceCents,
            firstName = Faker.Name.First(),
            surname = Faker.Name.Last()
        }
    };

    var gqlOrderResponse = await orderDataService.GetDataAsync<OrderCreateDto>(orderRequest, configVertical.EndPoint, configVertical.Key, configVertical.BasicAuthId, configVertical.BasicAuthPassword);

    if (gqlOrderResponse.Errors != null)
        ConsoleWriter.PrintColorMessage("--> Errors (order create) - process further to see what we do next", ConsoleColor.Red);

    Console.WriteLine($"-> Order Id: {gqlOrderResponse.Data.OrderCreate.Order!.LegacyId}");

}


// foreach (var variant in variants)
// {

//     var orderRequest = new GraphQLRequest
//     {
//         Query = GraphQLQueryLibrary.OrderCreate,
//         OperationName = "simpleOrderCreate",
//         Variables = new
//         {
//             variantId = variant.Id,
//             amount = variant.LowestPriceCents,
//             firstName = Faker.Name.First(),
//             surname = Faker.Name.Last()
//         }
//     };

//     var gqlOrderResponse = await orderDataService.GetDataAsync<OrderCreateDto>(orderRequest, configVertical.EndPoint, configVertical.Key, configVertical.BasicAuthId, configVertical.BasicAuthPassword);

//     if (gqlOrderResponse.Errors != null)
//         ConsoleWriter.PrintColorMessage("--> Errors (order create) - process further to see what we do next", ConsoleColor.Red);

//     Console.WriteLine($"-> Order Id: {gqlOrderResponse.Data.OrderCreate.Order!.LegacyId}");

// }

