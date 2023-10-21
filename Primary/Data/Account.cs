using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Primary.Data;

public class Account
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("brokerId")]
    public int BrokerId { get; set; }

    [JsonProperty("status")]
    public bool Status { get; set; }
}
