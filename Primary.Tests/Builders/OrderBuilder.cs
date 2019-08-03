using System.Linq;
using Primary.Data;
using Primary.Data.Orders;

namespace Primary.Tests.Builders
{
    public class OrderBuilder
    {
        public OrderBuilder(Api api)
        {
            _api = api;
        }

        private readonly Api _api;

        private Order Build()
        {
            var instruments = _api.GetAllInstruments().Result;

            return new Order
            {
                Instrument = instruments.First(),
                Expiration = OrderExpiration.ImmediateOrCancel,
                Type = OrderType.Limit,
                Quantity = 1000,
                Price = 3
            };
        }

        public static implicit operator Order(OrderBuilder builder)
        {
            return builder.Build();
        }
    }
}
