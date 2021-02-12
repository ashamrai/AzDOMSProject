using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzDOAddIn.RestApiClasses
{
    public class WorkItem
    {
        [JsonProperty("id")]
        public int id { get; set; }
        [JsonProperty("rev")]
        public int rev { get; set; }
        [JsonProperty("url")]
        public string url { get; set; }
        [JsonProperty("fields")]
        public Dictionary<string, object> fields { get; set; }
    }
}
