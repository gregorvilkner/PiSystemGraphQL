﻿# Building a GraphQL EndPoint for PI/AF using C\#

## Getting a WebPage Prepared and NuGet a few Dependencies

We're using .NET Core 3.1 Blazor for this example. Get a new blazor project started in VS and add the following 3 references:

1. GraphQL (2.4.0) - let's us define schema and process queries.
1. GraphiQL (1.2.0) - GraphiQL gives us the awesome graphiql playground page to test our queries.
1. Microsoft.AspNetCore.Mvc.NewtonsoftJson (3.1.2) - for GraphiQL to be able to properly render schema and responses.

## Getting a HelloWorld Query off the Ground

### Adjustments to Startup.cs

- services.AddRazorPages().AddNewtonsoftJson(); Simply add the NewtonsoftJson() to the existing statement.
- app.UseGraphiQl("/graphql"); I have it before UserRouting().
- endpoints.MapControllers(); Add this on top into the app.UseEndpoints block.


### Adding a GraphqlController

- add a new GraphqlController.cs file into a Controllers folder

```c#
using GraphQL;
using Microsoft.AspNetCore.Mvc;
using PiGraphNetCoreServer.GraphQl;
using System.Threading.Tasks;

namespace PiGraphNetCoreServer.Controllers
{
    [Route("graphql")]
    [ApiController]
    public class GraphqlController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] GraphQLQuery query)
        {
            var schema = new MySchema();
            var inputs = query.Variables.ToInputs();

            var result = await new DocumentExecuter().ExecuteAsync(_ =>
            {
                _.Schema = schema.GraphQLSchema;
                _.Query = query.Query;
                _.OperationName = query.OperationName;
                _.Inputs = inputs;
            });

            if (result.Errors?.Count > 0)
            {
                return BadRequest(result.Errors);
            }

            return Ok(result);

        }

    }
}
```

### Adding GraphQl Classes

We have a few classes to append to a GraphQl folder

1. GraphQLQuery.cs - a generic contract for any GraphQL query

```c#
using Newtonsoft.Json.Linq;

namespace PiGraphNetCoreServer.GraphQl
{
    public class GraphQLQuery
    {
        public string OperationName { get; set; }
        public string NamedQuery { get; set; }
        public string Query { get; set; }
        public JObject Variables { get; set; }

    }
}
```

2. PiSchema.cs - a class where we will describe our schema for AF elements, attributes, etc

```c#
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
            type Query {
                hello: String
            }
            ", _ =>
                {
                    _.Types.Include<Query>();
                });
        }
    }
}
```

3. Query.cs - a class to put what we'll query into the PI system to resolve GraphQL queries

```c#
using GraphQL;
namespace PiGraphNetCoreServer.GraphQl
{
    public class Query
    {
        [GraphQLMetadata("hello")]
        public string GetHello()
        {
            return "World";
        }
    }
}
```

### Finally a Link in the Nav to get to GraphiQL and Show Time

```html
        <li class="nav-item px-3">
            <NavLink class="nav-link" href="graphql">
                <span class="oi oi-list-rich" aria-hidden="true"></span> GraphQL
            </NavLink>
        </li>
```

We should now be able to run the app, be able to click the GraphQL link and end up in GraphiQL. On the left, we'll enter queries. In the middle, we get results - rendered as json. On the right we can interactively explore the schema.

We are now ready to hit PI...
