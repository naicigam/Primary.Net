using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

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
