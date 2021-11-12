using GraphQL;
using GraphQL.SystemTextJson;
using GraphQL.Types;
using OSIsoft.AF;
using PiGraphQlResolver.GraphQL;
using PiGraphQlSchema;
using System;
using System.Threading.Tasks;
using GraphQL.SystemTextJson;
using GraphQL.Instrumentation;

namespace PiGraphQlResolver
{
    public class ResolverEntry
    {
        public static PISystem aPiSystem { get; set; }

        public ResolverEntry(PISystem _piSystem)
        {
            aPiSystem = _piSystem;
            aPiSystem.Connect();
        }

        public async Task<ExecutionResult> GetResultAsync(GraphQLQuery query)
        {
            var start = DateTime.UtcNow;

            // https://fiyazhasan.me/graphql-with-net-core-part-v-fields-arguments-variables/
            var inputs = query.Variables.ToInputs();

            // https://github.com/graphql-dotnet/examples/blob/master/src/AspNetWebApi/WebApi/Controllers/GraphQLController.cs
            var result = await new DocumentExecuter().ExecuteAsync(_ =>
            {
                _.Schema = new MySchema().GraphQLSchema;
                _.Query = query.Query;
                _.OperationName = query.OperationName;
                _.Inputs = inputs;
                _.EnableMetrics = true;
            });

            result.EnrichWithApolloTracing(start);

            return result;

        }
    }
}
