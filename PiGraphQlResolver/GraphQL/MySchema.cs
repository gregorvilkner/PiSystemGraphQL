using GraphQL.Types;
using System;

namespace PiGraphQlResolver.GraphQL
{
    public class MySchema
    {
        private ISchema _schema { get; set; }
        public ISchema GraphQLSchema
        {
            get
            {
                return this._schema;
            }
        }

        public MySchema()
        {
            try
            {
                this._schema = Schema.For(PiGraphQlSchema.GraphQLSchema.schema, _ =>
                    {
                        _.Types.Include<Query>();
                        _.Types.Include<Mutation>();
                    });
            }
            catch(Exception e)
            {
                var m = e.Message;
            }
        }

    }

}
