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
    public class GraphQlAfElement
    {
        [JsonIgnore]
        public AFElement ThisAfElement { get; set; }
        public string name { get; set; }
        public string template { get; set; }
        public string path { get; set; }

        public List<GraphQlAfElement> afElements { get; set; }
        public List<GraphQlAfAttribute> afAttributes { get; set; }

        public GraphQlAfElement()
        {

        }
        public GraphQlAfElement(AFElement aAfElement, Field afElementsField, Field afAttributesField)
        {
            name = aAfElement.Name;
            template = aAfElement.Template?.Name;
            path = aAfElement.GetPath();
            ThisAfElement = aAfElement;

            if (afElementsField != null)
            {

                var afElementsNameFilterStrings = GraphQlHelpers.GetArgument(afElementsField, "nameFilter");
                var afElementsChildField = GraphQlHelpers.GetFieldFromSelectionSet(afElementsField, "afElements");
                var afAttributesChildField = GraphQlHelpers.GetFieldFromSelectionSet(afElementsField, "afAttributes");

                var returnElementsObject = new ConcurrentBag<GraphQlAfElement>();
                var afElementList = aAfElement.Elements;
                Parallel.ForEach(afElementList, aAfChildElement =>
                {
                    if (afElementsNameFilterStrings.Count == 0 || afElementsNameFilterStrings.Contains(aAfChildElement.Name))
                    {
                        returnElementsObject.Add(new GraphQlAfElement(aAfChildElement, afElementsChildField, afAttributesChildField));
                    }
                });
                afElements = returnElementsObject.OrderBy(x=>x.name).ToList();
            }

            if (afAttributesField != null)
            {

                var afAttributesNameFilterStrings = GraphQlHelpers.GetArgument(afAttributesField, "nameFilter");
                var afAttributesChildField = GraphQlHelpers.GetFieldFromSelectionSet(afAttributesField, "afAttributes");

                var returnAttributesObject = new ConcurrentBag<GraphQlAfAttribute>();
                var afAttributeList = aAfElement.Attributes.ToList<AFAttribute>();
                Parallel.ForEach(afAttributeList, aAfAttribute =>
                {
                    if (afAttributesNameFilterStrings.Count == 0 || afAttributesNameFilterStrings.Contains(aAfAttribute.Name))
                    {
                        returnAttributesObject.Add(new GraphQlAfAttribute(aAfAttribute, afAttributesChildField));
                    }
                });
                afAttributes = returnAttributesObject.OrderBy(x=>x.name).ToList();
            }

        }

    }
}
