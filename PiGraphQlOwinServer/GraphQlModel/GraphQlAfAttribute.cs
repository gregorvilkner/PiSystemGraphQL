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
    public class GraphQlAfAttribute
    {
        [JsonIgnore]
        public AFAttribute ThisAfAttribute { get; set; }
        public string name { get; set; }
        public string value { get; set; }
        public string timeStamp { get; set; }
        public string uom { get; set; }

        public List<GraphQlAfAttribute> afAttributes { get; set; }
        
        public GraphQlAfAttribute()
        {

        }
        public GraphQlAfAttribute(AFAttribute aAfAttribute, Field afAttributesField)
        {
            AFValue aAfValue = aAfAttribute.GetValue();

            name = aAfAttribute.Name;
            ThisAfAttribute = aAfAttribute;
            value = aAfValue.Value?.ToString();
            uom = aAfAttribute.DisplayUOM?.Abbreviation;
            
            if (aAfAttribute.DataReference?.Name == "PI Point")
            {
                timeStamp = aAfValue.Timestamp.UtcTime.ToString("yyyy-MM-ddTHH:mm:ssZ");
            }

            if (afAttributesField != null)
            {

                var afAttributesNameFilterStrings = GraphQlHelpers.GetArgument(afAttributesField, "nameFilter");
                var afAttributesChildField = GraphQlHelpers.GetFieldFromSelectionSet(afAttributesField, "afAttributes");

                var returnAttributesObject = new ConcurrentBag<GraphQlAfAttribute>();
                var afAttributeList = aAfAttribute.Attributes.ToList<AFAttribute>();
                Parallel.ForEach(afAttributeList, aAfChildAttribute =>
                {
                    if (afAttributesNameFilterStrings.Count == 0 || afAttributesNameFilterStrings.Contains(aAfChildAttribute.Name))
                    {
                        returnAttributesObject.Add(new GraphQlAfAttribute(aAfAttribute, afAttributesChildField));
                    }
                });
                afAttributes = returnAttributesObject.OrderBy(x=>x.name).ToList();
            }
        }
    }
}
