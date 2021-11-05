using GraphQL;
using GraphQL.Instrumentation;
using PiGraphNetCoreServer.GraphQl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace PiGraphQlOwinServer.api
{
    [Route("graphql")]
    public class GraphqlController : ApiController
    {
        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] GraphQLQuery query)
        {
            var start = DateTime.UtcNow;

            //var headers = Request.Headers;
            //KeyValuePair<string, IEnumerable<string>> authHeaders = headers.FirstOrDefault(valuePair => valuePair.Key == "Authorization");
            //if (authHeaders.Value == null)
            //{
            //    return BadRequest("Authorization header is required!");
            //}
            //else
            //{
            //    string username;
            //    string password;
            //    string domain;
                //try
                //{
                //    var headerString = authHeaders.Value.FirstOrDefault();
                //    var credentials = headerString.Split()[1];
                //    var decoded = Convert.FromBase64String(credentials);
                //    var abc = Encoding.UTF8.GetString(decoded);

                //    var domainusername = abc.Split(':')[0];
                //    domain = domainusername.Split('\\')[0];
                //    username = domainusername.Split('\\')[1];
                //    password = abc.Split(':')[1];
                //}
                //catch
                //{
                //    return BadRequest("Error parsing Authorization header.");
                //}

                var schema = new PiSchema();
                var inputs = query.Variables.ToInputs();

                var result = await new DocumentExecuter().ExecuteAsync(_ =>
                {
                    _.Schema = schema.GraphQLSchema;
                    _.Query = query.Query;
                    _.OperationName = query.OperationName;
                    _.Inputs = inputs;
                    //_.UserContext = new NetworkCredential(username, password, domain);
                    _.ExposeExceptions = true;
                    _.EnableMetrics = true;
                });

                result.EnrichWithApolloTracing(start);

                if (result.Errors?.Count > 0)
                {
                    return Ok(result);
                    //return BadRequest(String.Join("\r\n", result.Errors.Select(x => x.Message)));
                }

                return Ok(result);
            }

        //}


    }
}
