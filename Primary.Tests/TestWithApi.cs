using Primary.Data.Orders;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Primary.Tests
{
    internal class TestWithApi
    {
        protected string ApiAccount = Api.DemoAccount;
        protected Api Api
        {
            get
            {
                // Create a new one if it was logged out.
                if (_lazyApi == null || string.IsNullOrEmpty(_lazyApi.Value.AccessToken))
                {
                    _lazyApi = new Lazy<Api>(() => Build.AnApi());
                }

                return _lazyApi.Value;
            }
        }

        private static Lazy<Api> _lazyApi;

        protected string AnotherApiAccount = "REM20859";
        protected Api AnotherApi
        {
            get
            {
                // Create a new one if it was logged out.
                if (_lazyAnotherApi == null || string.IsNullOrEmpty(_lazyAnotherApi.Value.AccessToken))
                {
                    _lazyAnotherApi = new Lazy<Api>(() => Build.AnApi().WithUsername("alvarezjuandev20859").WithPassword("lolpzX4$"));
                }

                return _lazyAnotherApi.Value;
            }
        }

        private static Lazy<Api> _lazyAnotherApi;

        protected async Task<OrderStatus> WaitForOrderToComplete(Api api, OrderId orderId)
        {
            var orderStatus = await api.GetOrderStatus(orderId);
            while (orderStatus.Status == Status.PendingNew)
            {
                Thread.Sleep(200);
                orderStatus = await api.GetOrderStatus(orderId);
            }

            if (orderStatus.Status == Status.Rejected)
            {
                throw new InvalidOperationException(
                    $"{orderStatus.StatusText} / {orderStatus.InstrumentId.Symbol} / {orderStatus.Side}");
            }

            return orderStatus;
        }
    }
}
