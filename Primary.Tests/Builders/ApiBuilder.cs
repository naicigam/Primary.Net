using System.Threading.Tasks;

namespace Primary.Tests.Builders
{
    public class ApiBuilder
    {
        public ApiBuilder WithUsername(string username)
        {
            _username = username;
            return this;
        }
        private string _username = Api.DemoUsername;

        public ApiBuilder WithPassword(string password)
        {
            _password = password;
            return this;
        }
        private string _password = Api.DemoPassword;

        protected async Task<Api> Build()
        {
            var api = new Api(Api.DemoEndpoint);
            await api.Login(_username, _password);
            return api;
        }

        public static implicit operator Api(ApiBuilder builder)
        {
            return builder.Build().Result;
        }
    }
}
