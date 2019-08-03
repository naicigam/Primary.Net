using System.Threading.Tasks;
using Primary.Tests.Builders;

namespace Primary.Tests
{
    public static class Build
    {
        public static async Task<Api> AnApi()
        {
            var api = new Api(Api.DemoEndpoint);
            await api.Login(Api.DemoUsername, Api.DemoPassword);
            return api;
        }

        public static OrderBuilder AnOrder(Api api) { return new OrderBuilder(api); }
    }
}
