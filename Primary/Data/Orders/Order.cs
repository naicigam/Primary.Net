using System;
using Primary.Data.Orders;

namespace Primary.Data
{
    public class Order : IOrderStatus
    {
        public string Symbol => Instrument.Symbol;

        public bool CancelPrevious { get; set; }

        public bool Iceberg { get; set; }
        public uint DisplayQuantity { get; set; }

        // IOrderStatus implementation
        public OrderStatus Status { get; set; }
        public string StatusText { get; set; }

        public ulong ClientId { get; set; }
        public string Proprietary { get; set; }
        public Instrument Instrument { get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; }
        public OrderType Type { get; set; }
        public OrderSide Side { get; set; }
        public OrderExpiration Expiration { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
