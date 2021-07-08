using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Primary.Tests.Builders;

namespace Primary.Tests
{
    public static class Build
    {
        public static async Task<Api> AnApi()
        {
            var api = new Api(Api.DemoEndpoint);
            await api.Login(Api.DemoUsername, Api.DemoPassword);
            return api;
        }

        public static OrderBuilder AnOrder(Api api) { return new OrderBuilder(api); }

        public static string DollarFutureSymbol()
        {
            const string symbolBase = "DLR/";
            
            var monthMapper = new Dictionary<int, string>
            {
                {1, "ENE"}, {2, "FEB"}, {3, "MAR"}, {4, "ABR"}, 
                {5, "MAY"}, {6, "JUN"}, {7, "JUL"}, {8, "AGO"}, 
                {9, "SEP"}, {10, "OCT"}, {11, "NOV"}, {12, "DIC"}
            };
            var nextMonth = DateTime.Today.AddMonths(1);
            var nextMonthNumber = nextMonth.Month;
            var nextMonthYear = nextMonth.ToString("yy");
            
            return symbolBase + monthMapper[nextMonthNumber] + nextMonthYear;
        }
    }
}
