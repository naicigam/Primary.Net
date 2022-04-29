using Newtonsoft.Json;

namespace Primary.Data
{
    /// <summary>
    /// Interface for data with a size component.
    /// </summary>
    interface ISize
    {        
        [JsonProperty("size")]
        decimal Size { get; set; }
    }
}
