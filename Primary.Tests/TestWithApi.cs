﻿using Primary.Data;
using Primary.Data.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Primary.Tests
{
    internal class TestWithApi
    {
        protected Api Api => _lazyApi.Value;

        private static readonly Lazy<Api> _lazyApi = new(() => Build.AnApi().Result);

        protected async Task<MarketData> GetSomeMarketData()
        {
            if (AllInstrumentsBySymbol == null)
            {
                AllInstrumentsBySymbol = (await Api.GetAllInstruments()).ToDictionary(i => i.Symbol, i => i);
            }

            using var socket = Api.CreateMarketDataSocket(AllInstrumentsBySymbol.Values, new[] { Entry.Offers, Entry.Bids }, 1, 1);

            MarketData retrievedData = null;
            var dataSemaphore = new SemaphoreSlim(0, 1);
            socket.OnData = ((_, marketData) =>
            {
                var instrument = AllInstrumentsBySymbol[marketData.InstrumentId.Symbol];

                if (instrument.Type == InstrumentType.Equity &&
                    (
                        (marketData.Data.Offers != null &&
                         marketData.Data.Offers[0].Size > instrument.MinimumTradeVolume * 2)
                        ||
                        (marketData.Data.Bids != null &&
                         marketData.Data.Bids[0].Size > instrument.MinimumTradeVolume * 2)
                    )
                )
                {
                    retrievedData = marketData;
                    dataSemaphore.Release();
                }
            });

            await socket.Start();
            await dataSemaphore.WaitAsync();

            return retrievedData;
        }

        protected async Task<OrderStatus> WaitForOrderToComplete(OrderId orderId)
        {
            var orderStatus = await Api.GetOrderStatus(orderId);
            while (orderStatus.Status != Status.Rejected && orderStatus.Status != Status.Filled)
            {
                Thread.Sleep(200);
                orderStatus = await Api.GetOrderStatus(orderId);
            }

            if (orderStatus.Status == Status.Rejected)
            {
                throw new InvalidOperationException($"{orderStatus.StatusText} / {orderStatus.InstrumentId.Symbol} / {orderStatus.Side}");
            }
            return orderStatus;
        }

        protected Dictionary<string, Instrument> AllInstrumentsBySymbol;
    }
}
