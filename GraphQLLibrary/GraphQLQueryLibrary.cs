using System.Diagnostics;

public class GraphQLQueryLibrary
{
    internal static string AllSellerQuery { get; }= @"
    query  getAllActiveSellers(
	$pageSize: Int
	$endCursor: String
){
	allSellers (
		first: $pageSize
		after: $endCursor
		includeDeleted: false
		accountType: RETAILER
		sort: {field: BUSINESS_NAME, ordering: ASCENDING}
	){
		totalCount
		pageInfo{
			hasNextPage
			endCursor
		}
		nodes {
			... on Seller {
				id
				legacyId
				businessName
				online
			}
		}
	}
}
    ";

internal static string PurchaseableSellerProducts { get; } =@"
query getPurchaseableProducts(
	$pageSize: Int
	$endCUrsor: String
	$sellerIds: [ID!]
) {
	advertsWhere(
		first: $pageSize, 
		after: $endCUrsor, 
		retailerIds: $sellerIds
		status: DISPLAYED
		stockStatus: HAS_STOCK
	) {
		totalCount
		pageInfo {
			hasNextPage
			endCursor
		}
		nodes {
			id
			title
			legacyId
			variants {
				nodes {
					id
					countOnHand
					lowestPriceCents
				}
			}
		}
	}
}

";

internal static string OrderCreate { get; } = @"
mutation simpleOrderCreate($variantId: ID!, $amount: Int!, $firstName: String, $surname: String) {
	orderCreate(
		input: {
			order: {
				firstName: $firstName
				surname: $surname
				phone: ""0405510001""
				emailAddress: ""hs@email.com""
				billingFirstName: $firstName
				billingSurname: $surname
				billingEmailAddress: ""hs@email.com""
				billingPhone: ""0405510001""
				address: {
					address: ""35 Collins Street""
					city: ""Sydney""
					country: { code: ""AU"" }
					postcode: ""2000""
					state: { name: ""New South Wales"" }
				}
				billingAddress: {
					address: ""35 Collins Street""
					city: ""Sydney""
					country: { code: ""AU"" }
					postcode: ""2000""
					state: { name: ""New South Wales"" }
				}
			}
			lineItems: [
				# This line item is out of stock
				{
					variantId: $variantId
					quantity: 1
					cost: {
						amount: $amount
					}
				}
			]
		}
	) {
		order {
			id
			legacyId
			invoices {
				nodes {
					id
					legacyId
				}
			}
		}
		errors {
			field
			messages
		}
	}
}
";

}