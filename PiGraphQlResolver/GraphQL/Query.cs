using GraphQL;
using GraphQL.Types;
using OSIsoft.AF;
using OSIsoft.AF.Asset;
using PiGraphQlResolver.GraphQlModel;
using PiGraphQlSchema.GraphQlModel;
using PiGraphQlSchema.IGraphQl;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PiGraphQlResolver.GraphQL
{
    public class Query : ObjectGraphType, IQuery
    {

        string cleanupPath(string aPath)
        {
            return $"\\\\{String.Join("\\", aPath.Split('\\').Where(x => x != ""))}";
        }

        [GraphQLMetadata("hello")]
        public string GetHello()
        {
            return ResolverEntry.aPiSystem.Name;
        }

        [GraphQLMetadata("piSystem")]
        public QLPiSystem GetPiSystem(IResolveFieldContext context)
        {
            var afDbsField = GraphQlHelpers.GetFieldFromFieldOrContext(context, "afDbs");
            return new QLPiSystemResolve(ResolverEntry.aPiSystem, afDbsField);
        }

        [GraphQLMetadata("afDatabase")]
        public QLAfDatabase GetAfDatabase(IResolveFieldContext context, string aAfDatabasePath)
        {
            aAfDatabasePath = cleanupPath(aAfDatabasePath);

            var aAfDb = AFDatabase.FindObject(aAfDatabasePath) as AFDatabase;

            var afElementsField = GraphQlHelpers.GetFieldFromFieldOrContext(context, "afElements");

            var aGraphQlAfDatabase = new QLAfDatabaseResolve(aAfDb, afElementsField);

            return aGraphQlAfDatabase;
        }

        [GraphQLMetadata("afElement")]
        public QLAfElement GetAfElementByPath(IResolveFieldContext context, string aAfElementPath)
        {
            aAfElementPath = cleanupPath(aAfElementPath);
            var aAfElementSearch = AFElement.FindElementsByPath(new string[] { aAfElementPath }, null);
            if (aAfElementSearch.Count == 0)
            {
                context.Errors.Add(new ExecutionError($"AFElement path '{aAfElementPath}' not correct."));
                return null;
            }
            else
            {
                var aAfElement = aAfElementSearch.First() as AFElement;
                var afElementsField = GraphQlHelpers.GetFieldFromFieldOrContext(context, "afElements");
                var afAttributesField = GraphQlHelpers.GetFieldFromFieldOrContext(context, "afAttributes");
                var graphQlAfElement = new QLAfElementResolve(aAfElement, afElementsField, afAttributesField);
                return graphQlAfElement;
            }
        }

        [GraphQLMetadata("afElementTemplates")]
        public List<QLAfElementTemplate> GetAfElementTemplates(IResolveFieldContext context, string aAfDatabasePath, string[] nameFilter = null)
        {
            aAfDatabasePath = cleanupPath(aAfDatabasePath);
            var aAfDb = AFDatabase.FindObject(aAfDatabasePath) as AFDatabase;
            if (aAfDb == null)
            {
                context.Errors.Add(new ExecutionError($"AFDatabase not found."));
                return null;
            }

            var nameFilterStrings = nameFilter != null ? nameFilter.ToList() : new List<string>();
            var afElementsField = GraphQlHelpers.GetFieldFromFieldOrContext(context, "afElements");

            var returnObject = new ConcurrentBag<QLAfElementTemplate>();

            var aAfElementTemplateList = aAfDb.ElementTemplates;
            Parallel.ForEach(aAfElementTemplateList, aAfElementTemplate =>
            {
                if (nameFilterStrings.Count == 0 || nameFilterStrings.Contains(aAfElementTemplate.Name))
                {
                    var graphQlAfElementTemplate = new QLAfElementTemplateResolve(aAfElementTemplate, afElementsField);
                    returnObject.Add(graphQlAfElementTemplate);
                }
            });
            return returnObject.OrderBy(x => x.name).ToList();
        }

    }
}
