using GraphQL;
using PiGraphQlSchema.GraphQlModel;
using System.Collections.Generic;

namespace PiGraphQlSchema.IGraphQl
{
    public interface IQuery
    {
        
        string GetHello();

        [GraphQLMetadata("piSystem")]
        QLPiSystem GetPiSystem(ResolveFieldContext context, string name);

        [GraphQLMetadata("afDatabase")]
        QLAfDatabase GetAfDatabase(ResolveFieldContext context, string aAfDatabasePath);

        [GraphQLMetadata("afElement")]
        QLAfElement GetAfElementByPath(ResolveFieldContext context, string aAfElementPath);

        [GraphQLMetadata("afElementTemplates")]
        List<QLAfElementTemplate> GetAfElementTemplates(ResolveFieldContext context, string aAfDatabasePath, string[] nameFilter = null);

    }
}