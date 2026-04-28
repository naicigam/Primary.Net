using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Primary.Tests
{
    [TestFixture]
    public class LoginTests
    {
        [Test]
        [NonParallelizable]
        public async Task AccessTokenCanBeObtainedByLoginInWithUserAndPassword()
        {
            var api = new Api(Api.DemoEndpoint);
            Assert.That(api.AccessToken, Is.Null);

            await api.Login(DemoCredentials.Username, DemoCredentials.Password);
            Assert.That(api.AccessToken, Is.Not.Null);
        }

        [Test]
        [NonParallelizable]
        public async Task CanLogoutFromServer()
        {
            var api = new Api(Api.DemoEndpoint);
            await api.Login(DemoCredentials.Username, DemoCredentials.Password);

            await api.Logout();
            Assert.That(api.AccessToken, Is.Null);

            Assert.ThrowsAsync<HttpRequestException>(() => api.GetAllInstruments());
        }
    }
}
