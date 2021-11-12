using GraphQL.Language.AST;
using OSIsoft.AF;
using PiGraphQlResolver.GraphQL;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PiGraphQlSchema.GraphQlModel;


namespace PiGraphQlResolver.GraphQlModel
{
    public class QLPiSystemResolve:QLPiSystem
    {

        public QLPiSystemResolve(PISystem aPiSystem, Field afDbsField)
        {
            name = aPiSystem.Name;
            if (afDbsField != null)
            {

                var nameFilterStrings = GraphQlHelpers.GetArgumentStrings(afDbsField, "nameFilter");
                var afElementsField = GraphQlHelpers.GetFieldFromFieldOrContext(afDbsField, "afElements");

                var returnElementsObject = new ConcurrentBag<QLAfDatabase>();
                var afDbsList = aPiSystem.Databases;
                Parallel.ForEach(afDbsList, aAfDb =>
                {
                    if (nameFilterStrings.Count == 0 || nameFilterStrings.Contains(aAfDb.Name))
                    {
                        returnElementsObject.Add(new QLAfDatabaseResolve(aAfDb, afElementsField));
                    }
                });
                afDbs = returnElementsObject.OrderBy(x=>x.name).ToList();
            }
        }
    }
}
