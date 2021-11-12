using GraphQL;
using GraphQL.Language.AST;
using OSIsoft.AF.Asset;
using OSIsoft.AF.Time;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PiGraphQlResolver.GraphQL
{
    public static class GraphQlHelpers
    {
        public static List<string> GetArgumentStrings(Field aField, string name)
        {
            var nameFilterArgument = aField.Arguments?.FirstOrDefault(x => x.Name == name);
            List<string> nameFilterStrings = new List<string>();
            if (nameFilterArgument != null)
            {
                nameFilterStrings = (nameFilterArgument.Value.Value as List<object>).Select(x => x.ToString()).ToList();
            }
            return nameFilterStrings;
        }
        public static double GetArgumentDouble(Field aField, string name)
        {
            var nameFilterArgument = aField.Arguments?.FirstOrDefault(x => x.Name == name);
            double argumentValueDouble = 0;
            if (nameFilterArgument != null)
            {
                argumentValueDouble = double.Parse(nameFilterArgument.Value.Value.ToString());
            }
            return argumentValueDouble;
        }

        public static DateTime GetArgumentDateTime(Field aField, string name)
        {
            var nameFilterArgument = aField.Arguments.FirstOrDefault(x => x.Name == name);
            AFTime argumentValueDateTime = DateTime.UtcNow;
            if (nameFilterArgument != null)
            {
                AFTime.TryParse(nameFilterArgument.Value.Value.ToString(), out argumentValueDateTime);
            }
            return argumentValueDateTime.UtcTime;
        }


        public static Field GetFieldFromFieldOrContext(object aFieldOrContext, string name)
        {
            //https://systemoutofmemory.com/blogs/the-programmer-blog/c-sharp-switch-on-type
            switch (aFieldOrContext)
            {
                case Field aField:
                    return aField.SelectionSet.Children.FirstOrDefault(x => (x as Field).Name == name) as Field;
                case IResolveFieldContext aContext:
                    return aContext.SubFields.FirstOrDefault(x => x.Key == name).Value as Field;
                default:
                    return null;
            }
        }

        public static bool JudgeElementOnFilters(AFElement aAfElement, List<string> afElementsNameFilterStrings, List<string> afElementsAttributeValueFilterStrings)
        {
            bool includeAfElement = false;
            if (afElementsNameFilterStrings.Count == 0 && afElementsAttributeValueFilterStrings.Count == 0)
            {
                includeAfElement = true;
            }
            else
            {
                if (afElementsNameFilterStrings.Count > 0)
                {
                    if (afElementsNameFilterStrings.Contains(aAfElement.Name))
                    {
                        includeAfElement = true;
                    }
                }
                if (afElementsAttributeValueFilterStrings.Count > 0)
                {
                    var afAttributeList = aAfElement.Attributes.ToList<AFAttribute>().Where(x => x.DataReference == null).ToList();
                    afElementsAttributeValueFilterStrings.ForEach(aAttributeValueFilterString =>
                    {
                        var stringBits = aAttributeValueFilterString.Split('=');
                        if (stringBits.Length == 2)
                        {
                            var aAfAttribute = afAttributeList.FirstOrDefault(x => x.Name == stringBits[0]);
                            if (aAfAttribute != null)
                            {
                                if (aAfAttribute.GetValue().Value.ToString() == stringBits[1])
                                {
                                    includeAfElement = true;
                                }
                            }
                        }
                    });
                }
            }

            return includeAfElement;
        }

    }
}
