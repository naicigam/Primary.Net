using Newtonsoft.Json;
using System;

namespace Primary.Data
{
    /// <summary>
    /// Data with a date component.
    /// </summary>
    public class Date
    {
        [JsonProperty("datetime")]
        public DateTime DateTime { get; set; }

        [JsonProperty("date")]
        private long EpochDate { set { DateTime = FromUnixTime(value); } }

        public static DateTime FromUnixTime(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(unixTime);
        }
    }
}
