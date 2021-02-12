using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzDOAddIn.RestApiClasses
{
    public class TeamProjectsResponse
    {
        [JsonProperty("count")]
        public int count { get; set; }
        [JsonProperty("value")]
        public TeamProjectReference[] TeamProjects { get; set; }
    }

    public class TeamProjectReference
    {
        [JsonProperty("abbreviation")]
        public string abbreviation { get; set; }
        [JsonProperty("defaultTeamImageUrl")]
        public string defaultTeamImageUrl { get; set; }
        [JsonProperty("description")]
        public string description { get; set; }
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("lastUpdateTime")]
        public string lastUpdateTime { get; set; }
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("revision")]
        public int revision { get; set; }
        [JsonProperty("state")]
        public string state { get; set; }
        [JsonProperty("url")]
        public string url { get; set; }
        [JsonProperty("visibility")]
        public string visibility { get; set; }        
    }
}
