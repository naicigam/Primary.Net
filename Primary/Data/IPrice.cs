using Newtonsoft.Json;

namespace Primary.Data
{
    /// <summary>
    /// Interface for data with a price component.
    /// </summary>
    interface IPrice
    {        
        [JsonProperty("price")]
        decimal Price { get; set; }
    }
}
