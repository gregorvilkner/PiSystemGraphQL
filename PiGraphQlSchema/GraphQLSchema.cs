namespace PiGraphQlSchema
{
    public static class GraphQLSchema
    {
        public static readonly string schema = @"
            
            type GraphQlPiSystem {
                name: String,
                afDbs(nameFilter: [String]): [GraphQlAfDatabase]
            }

            type GraphQlAfDatabase {
                name: String,
                path: String,
                afElements(nameFilter: [String], attributeValueFilter: [String]): [GraphQlAfElement]
            }

            type GraphQlAfElement {
                name: String,
                path: String,
                template: String,
                afElements(nameFilter: [String], attributeValueFilter: [String]): [GraphQlAfElement],
                afAttributes(nameFilter: [String]): [GraphQlAfAttribute]
            }

            type GraphQlAfElementTemplate {
                name: String,
                afElements(nameFilter: [String], attributeValueFilter: [String]): [GraphQlAfElement],
            }

            type GraphQlAfAttribute {
                name: String,
                value: String,
                timeStamp: String,
                uom: String,
                afAttributes(nameFilter: [String]): [GraphQlAfAttribute],
                tsPlotValues(startDateTime: String, endDateTime: String, plotDensity: Int): [GraphQlTsValue]
            }

            type GraphQlTsValue {
                timeStamp: String,
                value: String
            }

            type Query {
                piSystem(name: String): GraphQlPiSystem
                afDatabase(aAfDatabasePath : String!): GraphQlAfDatabase
                afElement(aAfElementPath: String!): GraphQlAfElement
                afElementTemplates(aAfDatabasePath : String!, nameFilter: [String]): [GraphQlAfElementTemplate]
                hello: String
            }

            ";

    }
}
