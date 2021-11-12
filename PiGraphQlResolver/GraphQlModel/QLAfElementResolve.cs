using GraphQL.Language.AST;
using OSIsoft.AF.Asset;
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
    public class QLAfElementResolve:QLAfElement
    {
        public QLAfElementResolve(AFElement aAfElement, Field afElementsField, Field afAttributesField)
        {
            name = aAfElement.Name;
            template = aAfElement.Template?.Name;
            path = aAfElement.GetPath();

            if (afElementsField != null)
            {

                var afElementsNameFilterStrings = GraphQlHelpers.GetArgumentStrings(afElementsField, "nameFilter");
                var afElementsAttributeValueFilterStrings = GraphQlHelpers.GetArgumentStrings(afElementsField, "attributeValueFilter");
                var afElementsChildField = GraphQlHelpers.GetFieldFromFieldOrContext(afElementsField, "afElements");
                var afAttributesChildField = GraphQlHelpers.GetFieldFromFieldOrContext(afElementsField, "afAttributes");

                var returnElementsObject = new ConcurrentBag<QLAfElement>();
                var afElementList = aAfElement.Elements;
                Parallel.ForEach(afElementList, aAfChildElement =>
                {
                    if (GraphQlHelpers.JudgeElementOnFilters(aAfChildElement, afElementsNameFilterStrings, afElementsAttributeValueFilterStrings))
                    {
                        returnElementsObject.Add(new QLAfElementResolve(aAfChildElement, afElementsChildField, afAttributesChildField));
                    }
                });
                afElements = returnElementsObject.OrderBy(x=>x.name).ToList();
            }

            if (afAttributesField != null)
            {

                var afAttributesNameFilterStrings = GraphQlHelpers.GetArgumentStrings(afAttributesField, "nameFilter");
                var afAttributesChildField = GraphQlHelpers.GetFieldFromFieldOrContext(afAttributesField, "afAttributes");
                var tsValuesField= GraphQlHelpers.GetFieldFromFieldOrContext(afAttributesField, "tsPlotValues");

                var returnAttributesObject = new ConcurrentBag<QLAfAttribute>();
                var afAttributeList = aAfElement.Attributes.ToList<AFAttribute>();
                Parallel.ForEach(afAttributeList, aAfAttribute =>
                {
                    if (afAttributesNameFilterStrings.Count == 0 || afAttributesNameFilterStrings.Contains(aAfAttribute.Name))
                    {
                        returnAttributesObject.Add(new QLAfAttributeResolve(aAfAttribute, afAttributesChildField, tsValuesField));
                    }
                });
                afAttributes = returnAttributesObject.OrderBy(x=>x.name).ToList();
            }

        }

    }
}
