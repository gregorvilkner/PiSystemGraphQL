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
        QLAfDatabase GetAfDatabase(ResolveFieldContext context, string aAfDatabasePath);

        [GraphQLMetadata("afElement")]
        QLAfElement GetAfElementByPath(ResolveFieldContext context, string aAfElementPath);

        [GraphQLMetadata("afElementTemplates")]
        List<QLAfElementTemplate> GetAfElementTemplates(ResolveFieldContext context, string aAfDatabasePath, string[] nameFilter = null);


    }
}