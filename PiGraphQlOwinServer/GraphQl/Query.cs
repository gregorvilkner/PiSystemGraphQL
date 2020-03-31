using GraphQL;
using GraphQL.Language.AST;
using GraphQL.Types;
using OSIsoft.AF;
using OSIsoft.AF.Asset;
using PiGraphQlOwinServer.GraphQl;
using PiGraphQlOwinServer.GraphQlModel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PiGraphNetCoreServer.GraphQl
{
    public class Query : ObjectGraphType
    {
        PISystem aPiSystem { get; set; }
        NetworkCredential getCreds(ResolveFieldContext context)
        {
            return context.UserContext as NetworkCredential;
        }
        bool attemptLogin(ResolveFieldContext context, string piSystemName)
        {
            PISystems piSystems = new PISystems();
            if (!piSystems.Contains(piSystemName))
            {
                context.Errors.Add(new ExecutionError($"PISystem {piSystemName} not found."));
                return false;
            }
            else
            {
                aPiSystem = piSystems[piSystemName];
                try
                {
                    aPiSystem.Connect(getCreds(context));
                }
                catch (Exception e)
                {
                    context.Errors.Add(new ExecutionError($"Connection to PiSystem {piSystemName} failed - Connect() failed."));
                }
                if (aPiSystem.ConnectionInfo.IsConnected)
                {
                    return true;
                }
                else
                {
                    context.Errors.Add(new ExecutionError($"Connection to PiSystem {piSystemName} failed - IsConnected() is false."));
                    return false;
                }
            }
        }

        string getPiSystemName(string aPath)
        {
            return aPath.Split('\\').Where(x => x != "").First();
        }

        string cleanupPath(string aPath)
        {
            return $"\\\\{String.Join("\\", aPath.Split('\\').Where(x => x != ""))}";
        }

        [GraphQLMetadata("hello")]
        public string GetHello()
        {
            return "World";
        }

        [GraphQLMetadata("piSystem")]
        public GraphQlPiSystem GetPiSystem(ResolveFieldContext context, string name)
        {
            if(name==null)
            {
                string aDedicatedPiSystemName= ConfigurationManager.AppSettings["dedicatedPiSystem"];
                if(aDedicatedPiSystemName=="")
                {
                    context.Errors.Add(new ExecutionError($"No PISystem specified."));
                    return null;
                }
                else
                {
                    name = aDedicatedPiSystemName;
                }
            }
            if (attemptLogin(context, name))
            {
                var afDbsField = GraphQlHelpers.GetFieldFromContext(context, "afDbs");
                return new GraphQlPiSystem(aPiSystem, afDbsField);
            }
            else
            {
                return null;
            }

        }

        [GraphQLMetadata("afDatabase")]
        public GraphQlAfDatabase GetAfDatabase(ResolveFieldContext context, string aAfDatabasePath)
        {
            aAfDatabasePath = cleanupPath(aAfDatabasePath);
            if (attemptLogin(context, getPiSystemName(aAfDatabasePath)))
            {
                var aAfDb = AFDatabase.FindObject(aAfDatabasePath) as AFDatabase;

                var afElementsField = GraphQlHelpers.GetFieldFromContext(context, "afElements");
                
                var aGraphQlAfDatabase = new GraphQlAfDatabase(aAfDb, afElementsField);

                return aGraphQlAfDatabase;
            }
            else
            {
                return null;
            }

        }

        [GraphQLMetadata("afElement")]
        public GraphQlAfElement GetAfElementByPath(ResolveFieldContext context, string aAfElementPath)
        {
            aAfElementPath = cleanupPath(aAfElementPath);
            if (attemptLogin(context, getPiSystemName(aAfElementPath)))
            {
                var aAfElementSearch = AFElement.FindElementsByPath(new string[] { aAfElementPath }, null);
                if (aAfElementSearch.Count == 0)
                {
                    context.Errors.Add(new ExecutionError($"AFElement path '{aAfElementPath}' not correct."));
                    return null;
                }
                else
                {
                    var aAfElement = aAfElementSearch.First() as AFElement;
                    var afElementsField = GraphQlHelpers.GetFieldFromContext(context, "afElements");
                    var afAttributesField = GraphQlHelpers.GetFieldFromContext(context, "afAttributes");
                    var graphQlAfElement = new GraphQlAfElement(aAfElement, afElementsField, afAttributesField);
                    return graphQlAfElement;
                }
            }
            else
            {
                return null;
            }
        }
        
        [GraphQLMetadata("afElementTemplates")]
        public List<GraphQlAfElementTemplate> GetAfElementTemplates(ResolveFieldContext context, string aAfDatabasePath, string[] nameFilter = null)
        {
            aAfDatabasePath = cleanupPath(aAfDatabasePath);
            if (attemptLogin(context, getPiSystemName(aAfDatabasePath)))
            {
                var aAfDb = AFDatabase.FindObject(aAfDatabasePath) as AFDatabase;
                if (aAfDb == null)
                {
                    context.Errors.Add(new ExecutionError($"AFDatabase not found."));
                    return null;
                }

                var nameFilterStrings = nameFilter != null ? nameFilter.ToList() : new List<string>();
                var afElementsField = GraphQlHelpers.GetFieldFromContext(context, "afElements");
                
                var returnObject = new ConcurrentBag<GraphQlAfElementTemplate>();

                var aAfElementTemplateList = aAfDb.ElementTemplates;
                Parallel.ForEach(aAfElementTemplateList, aAfElementTemplate =>
                {
                    if (nameFilterStrings.Count == 0 || nameFilterStrings.Contains(aAfElementTemplate.Name))
                    {
                        var graphQlAfElementTemplate = new GraphQlAfElementTemplate(aAfElementTemplate, afElementsField);
                        returnObject.Add(graphQlAfElementTemplate);
                    }
                });
                return returnObject.OrderBy(x => x.name).ToList();
            }
            else
            {
                return null;
            }

        }


    }
}
