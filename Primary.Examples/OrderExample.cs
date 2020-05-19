using Primary.Data;
using Primary.Data.Orders;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Primary.Examples
{
    internal static class OrderExample
    {
        private static async Task Main()
        {
            var api = new Api(Api.DemoEndpoint);
            await api.Login(Api.DemoUsername, Api.DemoPassword);

            var order = new Order
            {
                Instrument = new Instrument { Market = "ROFX", Symbol = "DOJul20" },
                Expiration = OrderExpiration.Day,
                Type = OrderType.Limit,
                Price = 78m,

                Quantity = 1000,
                Iceberg = true,
                DisplayQuantity = 100
            };

            var orderId = await api.SubmitOrder(Api.DemoAccount, order);
            Console.WriteLine($"{orderId.ClientId} / {orderId.Proprietary}");

            var retrievedOrder = await api.GetOrder(orderId);
            Console.WriteLine($"{retrievedOrder.Quantity} / {retrievedOrder.DisplayQuantity}" +
                $"/ {retrievedOrder.LastQuantity} / {retrievedOrder.CumulativeQuantity} / {retrievedOrder.LeavesQuantity}");
        }
    }
}
