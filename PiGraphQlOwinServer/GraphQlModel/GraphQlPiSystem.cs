using GraphQL.Language.AST;
using OSIsoft.AF;
using PiGraphQlOwinServer.GraphQl;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiGraphQlOwinServer.GraphQlModel
{
    public class GraphQlPiSystem
    {
        public PISystem ThisPiSystem { get; set; }
        public string name { get; set; }

        public List<GraphQlAfDatabase> afDbs { get; set; }

        public GraphQlPiSystem()
        {

        }

        public GraphQlPiSystem(PISystem aPiSystem, Field afDbsField)
        {
            name = aPiSystem.Name;
            ThisPiSystem = aPiSystem;
            if (afDbsField != null)
            {

                var nameFilterStrings = GraphQlHelpers.GetArgumentStrings(afDbsField, "nameFilter");
                var afElementsField = GraphQlHelpers.GetFieldFromSelectionSet(afDbsField, "afElements");

                var returnElementsObject = new ConcurrentBag<GraphQlAfDatabase>();
                var afDbsList = aPiSystem.Databases;
                Parallel.ForEach(afDbsList, aAfDb =>
                {
                    if (nameFilterStrings.Count == 0 || nameFilterStrings.Contains(aAfDb.Name))
                    {
                        returnElementsObject.Add(new GraphQlAfDatabase(aAfDb, afElementsField));
                    }
                });
                afDbs = returnElementsObject.OrderBy(x=>x.name).ToList();
            }
        }
    }
}
