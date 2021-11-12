using GraphQL.Language.AST;
using OSIsoft.AF;
using PiGraphQlResolver.GraphQL;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using PiGraphQlSchema.GraphQlModel;


namespace PiGraphQlResolver.GraphQlModel
{
    public class QLAfDatabaseResolve:QLAfDatabase
    {

        public QLAfDatabaseResolve (AFDatabase aAfDatabase, Field afElementsField)
        {
            name = aAfDatabase.Name;
            path = aAfDatabase.GetPath();

            if (afElementsField != null)
            {

                var afElementsNameFilterStrings = GraphQlHelpers.GetArgumentStrings(afElementsField, "nameFilter");
                var afElementsAttributeValueFilterStrings = GraphQlHelpers.GetArgumentStrings(afElementsField, "attributeValueFilter");

                var afElementsChildField = GraphQlHelpers.GetFieldFromFieldOrContext(afElementsField, "afElements");
                var afAttributesChildField = GraphQlHelpers.GetFieldFromFieldOrContext(afElementsField, "afAttributes");

                var returnElementsObject = new ConcurrentBag<QLAfElement>();
                var afElementList = aAfDatabase.Elements;
                Parallel.ForEach(afElementList, aAfChildElement =>
                {
                    if (GraphQlHelpers.JudgeElementOnFilters(aAfChildElement, afElementsNameFilterStrings, afElementsAttributeValueFilterStrings))
                    {
                        returnElementsObject.Add(new QLAfElementResolve(aAfChildElement, afElementsChildField, afAttributesChildField));
                    }
                });
                afElements = returnElementsObject.OrderBy(x=>x.name).ToList();
            }
        }


    }
}
