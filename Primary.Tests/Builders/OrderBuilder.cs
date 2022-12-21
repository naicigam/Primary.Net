using Primary.Data;
using Primary.Data.Orders;
using System;
using System.Linq;

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
            var instrumentId = new InstrumentId()
            {
                Market = "ROFX",
                Symbol = Tests.Build.DollarFutureSymbol()
            };

            // Get a valid price
            var today = DateTime.Today;
            var prices = _api.GetHistoricalTrades(instrumentId, today.AddDays(-5), today).Result;

            return new Order
            {
                InstrumentId = instrumentId,
                Expiration = Expiration.Day,
                Type = Data.Orders.Type.Limit,
                Side = Side.Buy,
                Quantity = 1,
                Price = prices.Last().Price - 1m
            };
        }

        public static implicit operator Order(OrderBuilder builder)
        {
            return builder.Build();
        }
    }
}
