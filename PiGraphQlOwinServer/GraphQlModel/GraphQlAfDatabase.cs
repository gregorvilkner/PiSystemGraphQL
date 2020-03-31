using GraphQL.Language.AST;
using Newtonsoft.Json;
using OSIsoft.AF;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiGraphQlOwinServer.GraphQlModel
{
    public class GraphQlAfDatabase
    {
        public string name { get; set; }

        public string path { get; set; }

        [JsonIgnore]
        public AFDatabase thisAfDatabase { get; set; }

        public List<GraphQlAfElement> afElements { get; set; }

        public GraphQlAfDatabase()
        {

        }

        public GraphQlAfDatabase (AFDatabase aAfDatabase, Field afElementsField)
        {
            name = aAfDatabase.Name;
            path = aAfDatabase.GetPath();
            thisAfDatabase = aAfDatabase;

            if (afElementsField != null)
            {

                Field afElementsChildField = afElementsField.SelectionSet.Children.FirstOrDefault(x => (x as Field).Name == "afElements") as Field;
                Field afAttributesChildField = afElementsField.SelectionSet.Children.FirstOrDefault(x => (x as Field).Name == "afAttributes") as Field;

                var afElementsNameFilterArgument = afElementsField.Arguments.FirstOrDefault(x => x.Name == "nameFilter");
                List<object> afElementsNameFilterStrings = afElementsNameFilterArgument != null ? afElementsNameFilterArgument.Value.Value as List<object> : new List<object>();

                ConcurrentBag<GraphQlAfElement> returnElementsObject = new ConcurrentBag<GraphQlAfElement>();
                var afElementList = aAfDatabase.Elements;
                Parallel.ForEach(afElementList, aAfChildElement =>
                {
                    if (afElementsNameFilterStrings.Count == 0 || afElementsNameFilterStrings.Contains(aAfChildElement.Name))
                    {
                        returnElementsObject.Add(new GraphQlAfElement(aAfChildElement, afElementsChildField, afAttributesChildField));
                    }
                });
                afElements = returnElementsObject.ToList();
            }
        }


    }
}
