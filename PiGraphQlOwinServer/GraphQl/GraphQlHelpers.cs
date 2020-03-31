using GraphQL.Language.AST;
using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiGraphQlOwinServer.GraphQl
{
    public static class GraphQlHelpers
    {
        public static List<string> GetArgument(Field aField, string name)
        {
            var nameFilterArgument = aField.Arguments.FirstOrDefault(x => x.Name == name);
            List<string> nameFilterStrings = new List<string>();
            if (nameFilterArgument != null)
            {
                nameFilterStrings = (nameFilterArgument.Value.Value as List<object>).Select(x => x.ToString()).ToList();
            }
            return nameFilterStrings;
        }

        public static Field GetFieldFromSelectionSet(Field aField, string name)
        {
            return aField.SelectionSet.Children.FirstOrDefault(x => (x as Field).Name == name) as Field;
        }
        public static Field GetFieldFromContext(ResolveFieldContext context, string name)
        {
            return context.SubFields.FirstOrDefault(x => x.Key == name).Value as Field;
        }
    }
}
