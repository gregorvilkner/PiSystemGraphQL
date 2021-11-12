using GraphQL.Language.AST;
using OSIsoft.AF.Asset;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PiGraphQlResolver.GraphQL;
using PiGraphQlSchema.GraphQlModel;


namespace PiGraphQlResolver.GraphQlModel
{
    public class QLAfElementTemplateResolve:QLAfElementTemplate
    {
        public QLAfElementTemplateResolve(AFElementTemplate aAfElementTemplate, Field afElementsField)
        {
            name = aAfElementTemplate.Name;

            if(afElementsField!=null)
            {

                var afElementsNameFilterStrings = GraphQlHelpers.GetArgumentStrings(afElementsField, "nameFilter");
                var afElementsAttributeValueFilterStrings = GraphQlHelpers.GetArgumentStrings(afElementsField, "attributeValueFilter");

                var afElementsChildField = GraphQlHelpers.GetFieldFromFieldOrContext(afElementsField, "afElements");
                var afAttributesChildField = GraphQlHelpers.GetFieldFromFieldOrContext(afElementsField, "afAttributes");

                var returnObject = new ConcurrentBag<QLAfElement>();

                List<AFElement> afElementList = aAfElementTemplate.FindInstantiatedElements(true, OSIsoft.AF.AFSortField.Name, OSIsoft.AF.AFSortOrder.Ascending, 10000).Select(x => x as AFElement).Where(x => x != null).ToList();
                Parallel.ForEach(afElementList, aAfChildElement =>
                {
                    if (GraphQlHelpers.JudgeElementOnFilters(aAfChildElement, afElementsNameFilterStrings, afElementsAttributeValueFilterStrings))
                    {
                        returnObject.Add(new QLAfElementResolve(aAfChildElement, afElementsChildField, afAttributesChildField));
                    }
                });

                afElements = returnObject.OrderBy(x=>x.name).ToList();
            }
        }

    }
}
