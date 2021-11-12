namespace PiGraphQlSchema
{
    public static class GraphQLSchema
    {
        public static readonly string schema = @"
            
            type QLPiSystem {
                name: String,
                afDbs(nameFilter: [String]): [QLAfDatabase]
            }

            type QLAfDatabase {
                name: String,
                path: String,
                afElements(nameFilter: [String], attributeValueFilter: [String]): [QLAfElement]
            }

            type QLAfElement {
                name: String,
                path: String,
                template: String,
                afElements(nameFilter: [String], attributeValueFilter: [String]): [QLAfElement],
                afAttributes(nameFilter: [String]): [QLAfAttribute]
            }

            type QLAfElementTemplate {
                name: String,
                afElements(nameFilter: [String], attributeValueFilter: [String]): [QLAfElement],
            }

            type QLAfAttribute {
                name: String,
                value: String,
                timeStamp: String,
                uom: String,
                afAttributes(nameFilter: [String]): [QLAfAttribute],
                tsPlotValues(startDateTime: String, endDateTime: String, plotDensity: Int): [QLTsValue]
            }

            type QLTsValue {
                timeStamp: String,
                value: String
            }

            type Query {
                piSystem(name: String): QLPiSystem
                afDatabase(aAfDatabasePath : String!): QLAfDatabase
                afElement(aAfElementPath: String!): QLAfElement
                afElementTemplates(aAfDatabasePath : String!, nameFilter: [String]): [QLAfElementTemplate]
                hello: String
            }

            ";

    }
}
