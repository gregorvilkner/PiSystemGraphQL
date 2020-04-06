using GraphQL.Language.AST;
using Newtonsoft.Json;
using OSIsoft.AF.Asset;
using PiGraphQlOwinServer.GraphQl;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiGraphQlOwinServer.GraphQlModel
{
    public class GraphQlAfElementTemplate
    {
        [JsonIgnore]
        public AFElementTemplate ThisAfElementTemplate { get; set; }
        public string name { get; set; }

        public List<GraphQlAfElement> afElements { get; set; }

        public GraphQlAfElementTemplate()
        {

        }
        public GraphQlAfElementTemplate(AFElementTemplate aAfElementTemplate, Field afElementsField)
        {
            name = aAfElementTemplate.Name;
            ThisAfElementTemplate = aAfElementTemplate;
            if(afElementsField!=null)
            {

                var afElementsNameFilterStrings = GraphQlHelpers.GetArgumentStrings(afElementsField, "nameFilter");
                var afElementsAttributeValueFilterStrings = GraphQlHelpers.GetArgumentStrings(afElementsField, "attributeValueFilter");

                var afElementsChildField = GraphQlHelpers.GetFieldFromSelectionSet(afElementsField, "afElements");
                var afAttributesChildField = GraphQlHelpers.GetFieldFromSelectionSet(afElementsField, "afAttributes");

                var returnObject = new ConcurrentBag<GraphQlAfElement>();

                List<AFElement> afElementList = aAfElementTemplate.FindInstantiatedElements(true, OSIsoft.AF.AFSortField.Name, OSIsoft.AF.AFSortOrder.Ascending, 10000).Select(x => x as AFElement).Where(x => x != null).ToList();
                Parallel.ForEach(afElementList, aAfChildElement =>
                {
                    if (GraphQlHelpers.JudgeElementOnFilters(aAfChildElement, afElementsNameFilterStrings, afElementsAttributeValueFilterStrings))
                    {
                        returnObject.Add(new GraphQlAfElement(aAfChildElement, afElementsChildField, afAttributesChildField));
                    }
                });

                afElements = returnObject.OrderBy(x=>x.name).ToList();
            }
        }

    }
}
