using Primary.Data;
using Primary.Data.Orders;
using System.Collections.Generic;
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
            if (_instruments == null)
            {
                _instruments = _api.GetAllInstruments().Result;
            }

            var instrument = _instruments.First(i => i.Symbol.Contains("GGAL"));
            return new Order
            {
                InstrumentId = instrument,
                Expiration = Expiration.Day,
                Type = Type.Limit,
                Side = Side.Buy,
                Quantity = 1,
                Price = instrument.MinimumTradePrice
            };
        }

        public static implicit operator Order(OrderBuilder builder)
        {
            return builder.Build();
        }

        private IEnumerable<Instrument> _instruments;
    }
}
