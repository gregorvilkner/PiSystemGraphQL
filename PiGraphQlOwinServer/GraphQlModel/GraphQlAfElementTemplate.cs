using GraphQL.Language.AST;
using Newtonsoft.Json;
using OSIsoft.AF.Asset;
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

                Field afElementsChildField = afElementsField.SelectionSet.Children.FirstOrDefault(x => (x as Field).Name == "afElements") as Field;
                Field afAttributesChildField = afElementsField.SelectionSet.Children.FirstOrDefault(x => (x as Field).Name == "afAttributes") as Field;

                var nameFilterArgument = afElementsField.Arguments.FirstOrDefault(x => x.Name == "nameFilter");
                List<object> nameFilterStrings = nameFilterArgument != null ? nameFilterArgument.Value.Value as List<object> : new List<object>();

                ConcurrentBag<GraphQlAfElement> returnObject = new ConcurrentBag<GraphQlAfElement>();

                List<AFElement> afElementList = aAfElementTemplate.FindInstantiatedElements(true, OSIsoft.AF.AFSortField.Name, OSIsoft.AF.AFSortOrder.Ascending, 10000).Select(x => x as AFElement).Where(x => x != null).ToList();
                Parallel.ForEach(afElementList, aAfElement =>
                {
                    if (nameFilterStrings.Count == 0 || nameFilterStrings.Contains(aAfElement.Name))
                    {
                        returnObject.Add(new GraphQlAfElement(aAfElement, afElementsChildField, afAttributesChildField));
                    }
                });

                afElements = returnObject.ToList();
            }
        }

    }
}
