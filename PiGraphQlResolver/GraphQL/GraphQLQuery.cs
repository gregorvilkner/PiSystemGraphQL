using System.Collections.Generic;
using System.Text.Json.Serialization;
using GraphQL.SystemTextJson;

namespace PiGraphQlResolver.GraphQL
{
    public class GraphQLQuery
    {
        public string OperationName { get; set; }
        public string NamedQuery { get; set; }
        public string Query { get; set; }


        // https://fiyazhasan.me/graphql-with-net-core-part-v-fields-arguments-variables/
        [JsonConverter(typeof(ObjectDictionaryConverter))]
        //[JsonConverter(typeof(ObjectDictionaryConverter))]
        public Dictionary<string, object> Variables
        {
            get; set;
        }

    }
}
