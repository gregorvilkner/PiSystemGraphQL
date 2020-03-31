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

            //Field afElementsField = subField.SelectionSet.Children.FirstOrDefault(x => (x as Field).Name == "afElements") as Field;
            if (afElementsField != null)
            {

                Field afElementsChildField = afElementsField.SelectionSet.Children.FirstOrDefault(x => (x as Field).Name == "afElements") as Field;
                Field afAttributesChildField = afElementsField.SelectionSet.Children.FirstOrDefault(x => (x as Field).Name == "afAttributes") as Field;

                var afElementsNameFilterArgument = afElementsField.Arguments.FirstOrDefault(x => x.Name == "nameFilter");
                List<object> afElementsNameFilterStrings = afElementsNameFilterArgument != null ? afElementsNameFilterArgument.Value.Value as List<object> : new List<object>();

                ConcurrentBag<GraphQlAfElement> returnElementsObject = new ConcurrentBag<GraphQlAfElement>();
                var afElementList = aAfElement.Elements;
                Parallel.ForEach(afElementList, aAfChildElement =>
                {
                    if (afElementsNameFilterStrings.Count == 0 || afElementsNameFilterStrings.Contains(aAfChildElement.Name))
                    {
                        returnElementsObject.Add(new GraphQlAfElement(aAfChildElement, afElementsChildField, afAttributesChildField));
                    }
                });
                afElements = returnElementsObject.ToList();
            }

            //Field afAttributesField = subField.SelectionSet.Children.FirstOrDefault(x => (x as Field).Name == "afAttributes") as Field;
            if (afAttributesField != null)
            {

                Field afAttributesChildField = afAttributesField.SelectionSet.Children.FirstOrDefault(x => (x as Field).Name == "afAttributes") as Field;

                var afAttributesNameFilterArgument = afAttributesField.Arguments.FirstOrDefault(x => x.Name == "nameFilter");
                List<object> afAttributesNameFilterStrings = afAttributesNameFilterArgument != null ? afAttributesNameFilterArgument.Value.Value as List<object> : new List<object>();

                ConcurrentBag<GraphQlAfAttribute> returnAttributesObject = new ConcurrentBag<GraphQlAfAttribute>();
                var afAttributeList = aAfElement.Attributes.ToList<AFAttribute>();
                Parallel.ForEach(afAttributeList, aAfAttribute =>
                {
                    if (afAttributesNameFilterStrings.Count == 0 || afAttributesNameFilterStrings.Contains(aAfAttribute.Name))
                    {
                        returnAttributesObject.Add(new GraphQlAfAttribute(aAfAttribute, afAttributesChildField));
                    }
                });
                afAttributes = returnAttributesObject.ToList();
            }

        }

    }
}
