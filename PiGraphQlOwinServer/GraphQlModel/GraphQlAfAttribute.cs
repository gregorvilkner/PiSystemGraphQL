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

                Field afAttributesChildField = afAttributesField.SelectionSet.Children.FirstOrDefault(x => (x as Field).Name == "afAttributes") as Field;

                var afAttributesNameFilterArgument = afAttributesField.Arguments.FirstOrDefault(x => x.Name == "nameFilter");
                List<object> afAttributesNameFilterStrings = afAttributesNameFilterArgument != null ? afAttributesNameFilterArgument.Value.Value as List<object> : new List<object>();

                ConcurrentBag<GraphQlAfAttribute> returnAttributesObject = new ConcurrentBag<GraphQlAfAttribute>();
                var afAttributeList = aAfAttribute.Attributes.ToList<AFAttribute>();
                Parallel.ForEach(afAttributeList, aAfChildAttribute =>
                {
                    if (afAttributesNameFilterStrings.Count == 0 || afAttributesNameFilterStrings.Contains(aAfChildAttribute.Name))
                    {
                        returnAttributesObject.Add(new GraphQlAfAttribute(aAfAttribute, afAttributesChildField));
                    }
                });
                afAttributes = returnAttributesObject.ToList();
            }
        }
    }
}
