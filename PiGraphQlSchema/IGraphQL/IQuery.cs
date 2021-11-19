using GraphQL;
using PiGraphQlSchema.GraphQlModel;
using System.Collections.Generic;

namespace PiGraphQlSchema.IGraphQl
{
    public interface IQuery
    {
        [GraphQLMetadata("hello")]
        string GetHello();

        [GraphQLMetadata("piSystem")]
        QLPiSystem GetPiSystem(IResolveFieldContext context);

        [GraphQLMetadata("afDatabase")]
        QLAfDatabase GetAfDatabase(IResolveFieldContext context, string aAfDatabasePath);

        [GraphQLMetadata("afElement")]
        QLAfElement GetAfElementByPath(IResolveFieldContext context, string aAfElementPath);

        [GraphQLMetadata("afElementTemplates")]
        List<QLAfElementTemplate> GetAfElementTemplates(IResolveFieldContext context, string aAfDatabasePath, string[] nameFilter = null);


    }
}