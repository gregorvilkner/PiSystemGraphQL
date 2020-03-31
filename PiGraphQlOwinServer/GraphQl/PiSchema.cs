using GraphQL.Types;

namespace PiGraphNetCoreServer.GraphQl
{
    public class PiSchema
    {
        private ISchema _schema { get; set; }
        public ISchema GraphQLSchema
        {
            get
            {
                return this._schema;
            }
        }

        public PiSchema()
        {
            this._schema = Schema.For(@"
            
            type GraphQlPiSystem {
                name: String,
                afDbs(nameFilter: [String]): [GraphQlAfDatabase]
            }

            type GraphQlAfDatabase {
                name: String,
                path: String,
                afElements(nameFilter: [String]): [GraphQlAfElement]
            }

            type GraphQlAfElement {
                name: String,
                path: String,
                template: String,
                afElements(nameFilter: [String]): [GraphQlAfElement],
                afAttributes(nameFilter: [String]): [GraphQlAfAttribute]
            }

            type GraphQlAfElementTemplate {
                name: String,
                afElements(nameFilter: [String]): [GraphQlAfElement],
            }

            type GraphQlAfAttribute {
                name: String,
                value: String,
                timeStamp: String,
                uom: String,
                afAttributes(nameFilter: [String]): [GraphQlAfAttribute]
            }

            type Query {
                piSystem(name: String): GraphQlPiSystem
                afDatabase(aAfDatabasePath : String!): GraphQlAfDatabase
                afElement(aAfElementPath: String!): GraphQlAfElement
                afElementTemplates(aAfDatabasePath : String!, nameFilter: [String]): [GraphQlAfElementTemplate]
                hello: String
            }

            ", _ =>
                {
                    _.Types.Include<Query>();
                });
        }

    }

}
