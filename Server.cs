using GraphQL;
using GraphQL.Types;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Function {
    public class Server
    {
        private ISchema schema { get; set; }
        public Server()
        {
            this.schema = Schema.For(@"
                type Jedi {
                    id: ID,
                    name: String,
                    side: String
                }

                input JediInput {
                    name: String
                    side: String
                    id: ID
                }

                type Mutation {
                    addJedi(input: JediInput): Jedi
                    updateJedi(input: JediInput): Jedi
                    removeJedi(id: ID): String
                }

                type Query {
                    jedis: [Jedi]
                    jedi(id: ID): Jedi
                }
            ", _ =>
            {
                _.Types.Include<Query>();
                _.Types.Include<Mutation>();
            });
        }
        public async Task<string> QueryAsync(string query)
        {
            var result = await new DocumentExecuter().ExecuteAsync(_ =>
            {
                _.Schema = schema;
                _.Query = query;
            });

            if(result.Errors != null) {
                return result.Errors[0].Message;
            } else {
                return JsonConvert.SerializeObject(result.Data);
            }
        }
    }
}