using GraphQL;
using GraphQL.SystemTextJson;
using GraphQL.Types;
using Newtonsoft.Json;
using OSIsoft.AF;
using PiGraphQlResolver;
using PiGraphQlResolver.GraphQL;
using PiGraphQlSchema;
using PiGraphQlSchema.GraphQlModel;
using System;
using System.Threading.Tasks;

namespace PiGraphQlResolverTestConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {

            PISystems piSystems = new PISystems();
            var aPiSystem = piSystems.DefaultPISystem;

            ResolverEntry aEntry = new ResolverEntry(aPiSystem);

            //string graphQlQueryJsonString = @"{""query"": ""{\n hello \n}\n""}";

            string query = @"
            {
                piSystem {
                    name
                    afDbs {
                        name
                        afElements {
                            name
                            afAttributes {
                                name
                                value
                                tsPlotValues(startDateTime: "" - 20m"", endDateTime: ""*"") {
                                    timeStamp
                                    value
                                }
                            }
                        }
                    }
                }
            }
            ";
            string graphQlQueryJsonString = $@"{{""query"": {JsonConvert.SerializeObject(query)}}}";

            GraphQLQuery graphQLQuery = JsonConvert.DeserializeObject<GraphQLQuery>(graphQlQueryJsonString);
            var result = await aEntry.GetResultAsync(graphQLQuery);

            var resultString = await new DocumentWriter().WriteToStringAsync(result);

            Console.WriteLine(resultString);
            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }
}
