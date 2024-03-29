﻿using Primary.Data.Orders;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Primary.Examples
{
    internal static class OrderExample
    {
        public static async Task Run()
        {
            var api = new Api(Api.DemoEndpoint);
            await api.Login(Api.DemoUsername, Api.DemoPassword);

            // Get a valid instrument and price
            var instruments = await api.GetAllInstruments();
            var instrumentId = instruments.Last(i => i.Symbol == Tests.Build.DollarFutureSymbol());

            var today = DateTime.Today;
            var prices = await api.GetHistoricalTrades(instrumentId, today.AddDays(-3), today);

            var order = new Order
            {
                InstrumentId = instrumentId,
                Expiration = Expiration.Day,
                Type = Data.Orders.Type.Limit,
                Price = prices.Last().Price,
                Side = Side.Sell,
                Quantity = 100
            };

            var orderId = await api.SubmitOrder(Api.DemoAccount, order);
            Console.WriteLine($"{orderId.ClientOrderId} / {orderId.Proprietary}");

            var retrievedOrder = await api.GetOrderStatus(orderId);
            Console.WriteLine($"{retrievedOrder.Status} / {retrievedOrder.StatusText}");

            Console.WriteLine($"{retrievedOrder.Quantity} / {retrievedOrder.DisplayQuantity}" +
                $"/ {retrievedOrder.LastQuantity} / {retrievedOrder.CumulativeQuantity} / {retrievedOrder.LeavesQuantity}");
        }
    }
}
