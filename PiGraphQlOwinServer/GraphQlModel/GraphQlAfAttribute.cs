using GraphQL.Language.AST;
using Newtonsoft.Json;
using OSIsoft.AF.Asset;
using OSIsoft.AF.Time;
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
        
        public List<GraphQlTsValue> tsPlotValues { get; set; }

        public GraphQlAfAttribute()
        {

        }
        public GraphQlAfAttribute(AFAttribute aAfAttribute, Field afAttributesField=null, Field tsPlotValuesField=null)
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

                var afAttributesNameFilterStrings = GraphQlHelpers.GetArgumentStrings(afAttributesField, "nameFilter");
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

            if(tsPlotValuesField!=null)
            {
                if (aAfAttribute.DataReference?.Name == "PI Point")
                {
                    var plotDensity = GraphQlHelpers.GetArgumentDouble(tsPlotValuesField, "plotDensity");
                    var startDateTime = GraphQlHelpers.GetArgumentDateTime(tsPlotValuesField, "startDateTime");
                    var endDateTime = GraphQlHelpers.GetArgumentDateTime(tsPlotValuesField, "endDateTime");

                    var timeRange = new AFTimeRange(startDateTime, endDateTime);

                    AFValues asdf = ThisAfAttribute.GetValues(timeRange, (int)plotDensity, null);

                    var returnObject = new ConcurrentBag<GraphQlTsValue>();
                    foreach (AFValue aAfTsValue in asdf)
                    {
                        returnObject.Add(new GraphQlTsValue()
                        {
                            timeStamp = aAfTsValue.Timestamp.UtcTime.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                            value = aAfTsValue.Value.ToString()
                        });
                    }
                    tsPlotValues = returnObject.OrderBy(x => x.timeStamp).ToList();
                }
            }
        }
    }
}
