using GraphQL.Language.AST;
using Newtonsoft.Json;
using OSIsoft.AF;
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

                var afElementsNameFilterStrings = GraphQlHelpers.GetArgumentStrings(afElementsField, "nameFilter");
                var afElementsAttributeValueFilterStrings = GraphQlHelpers.GetArgumentStrings(afElementsField, "attributeValueFilter");

                var afElementsChildField = GraphQlHelpers.GetFieldFromSelectionSet(afElementsField, "afElements");
                var afAttributesChildField = GraphQlHelpers.GetFieldFromSelectionSet(afElementsField, "afAttributes");

                var returnElementsObject = new ConcurrentBag<GraphQlAfElement>();
                var afElementList = aAfDatabase.Elements;
                Parallel.ForEach(afElementList, aAfChildElement =>
                {
                    if (GraphQlHelpers.JudgeElementOnFilters(aAfChildElement, afElementsNameFilterStrings, afElementsAttributeValueFilterStrings))
                    {
                        returnElementsObject.Add(new GraphQlAfElement(aAfChildElement, afElementsChildField, afAttributesChildField));
                    }
                });
                afElements = returnElementsObject.OrderBy(x=>x.name).ToList();
            }
        }


    }
}
