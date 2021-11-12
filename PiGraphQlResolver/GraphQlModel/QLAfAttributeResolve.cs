using GraphQL.Language.AST;
using OSIsoft.AF.Asset;
using OSIsoft.AF.Time;
using PiGraphQlResolver.GraphQL;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using PiGraphQlSchema.GraphQlModel;


namespace PiGraphQlResolver.GraphQlModel
{
    public class QLAfAttributeResolve:QLAfAttribute
    {
        public QLAfAttributeResolve(AFAttribute aAfAttribute, Field afAttributesField=null, Field tsPlotValuesField=null)
        {
            AFValue aAfValue = aAfAttribute.GetValue();

            name = aAfAttribute.Name;
            value = aAfValue.Value?.ToString();
            uom = aAfAttribute.DisplayUOM?.Abbreviation;
            
            if (aAfAttribute.DataReference?.Name == "PI Point")
            {
                timeStamp = aAfValue.Timestamp.UtcTime.ToString("yyyy-MM-ddTHH:mm:ssZ");
            }

            if (afAttributesField != null)
            {

                var afAttributesNameFilterStrings = GraphQlHelpers.GetArgumentStrings(afAttributesField, "nameFilter");
                var afAttributesChildField = GraphQlHelpers.GetFieldFromFieldOrContext(afAttributesField, "afAttributes");

                var returnAttributesObject = new ConcurrentBag<QLAfAttribute>();
                var afAttributeList = aAfAttribute.Attributes.ToList<AFAttribute>();
                Parallel.ForEach(afAttributeList, aAfChildAttribute =>
                {
                    if (afAttributesNameFilterStrings.Count == 0 || afAttributesNameFilterStrings.Contains(aAfChildAttribute.Name))
                    {
                        returnAttributesObject.Add(new QLAfAttributeResolve(aAfAttribute, afAttributesChildField));
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

                    AFValues asdf = aAfAttribute.GetValues(timeRange, (int)plotDensity, null);

                    var returnObject = new ConcurrentBag<QLTsValue>();
                    foreach (AFValue aAfTsValue in asdf)
                    {
                        returnObject.Add(new QLTsValueResolve()
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
