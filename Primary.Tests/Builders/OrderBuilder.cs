using System;
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
            var instrument = instruments.Last( i => i.Symbol == Tests.Build.DollarFutureSymbol() );

            // Get a valid price
            var yesterday = DateTime.Today.AddDays(-1);
            var prices = _api.GetHistoricalTrades(instrument, yesterday.AddDays(-5), yesterday).Result;

            return new Order
            {
                Instrument = instrument,
                Expiration = OrderExpiration.Day,
                Type = OrderType.Limit,
                Quantity = 100,
                Price = prices.Last().Price
            };
        }

        public static implicit operator Order(OrderBuilder builder)
        {
            return builder.Build();
        }
    }
}
