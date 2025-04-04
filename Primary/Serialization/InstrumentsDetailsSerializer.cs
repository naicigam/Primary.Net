using System;
using System.Collections.Generic;
using System.Text.Json;

using Primary.Data;
using System.IO;
using System.Diagnostics;

namespace Primary.Serialization
{
    internal class CacheInstrumentsDetails
    {
        /// <summary>
        /// Timestamp of the cached information
        /// </summary>
        public DateTime CacheDate { get; set; }
        public List<Instrument> InstrumentDetailList { get; set; } = new List<Instrument>();
    }

    internal static class InstrumentsDetailsSerializer
    {
        public static void SerializeInstrumentsDetails(CacheInstrumentsDetails cache, string fileName)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string info = JsonSerializer.Serialize<CacheInstrumentsDetails>(cache, options);
            File.WriteAllText(fileName, info);
        }

        public static CacheInstrumentsDetails DesSerializeInstrumentDetail(string fileName)
        {
            CacheInstrumentsDetails cache = null;
            try
            {
                if (File.Exists(fileName) == true)
                {
                    string info = File.ReadAllText(fileName);
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    cache = new CacheInstrumentsDetails();
                    cache = JsonSerializer.Deserialize<CacheInstrumentsDetails>(info, options);
                }
            }
            catch (System.Text.Json.JsonException jex)
            {
                Trace.TraceError(jex.Message);
                cache = null;
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                cache = null;
            }

            return cache;
        }
    }
}
