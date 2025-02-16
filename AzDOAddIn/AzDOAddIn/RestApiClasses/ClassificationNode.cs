using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzDOAddIn.RestApiClasses
{
    public class ClassificationNode
    {
        [JsonProperty("id")]
        public int id { get; set; }
        [JsonProperty("identifier")]
        public string identifier { get; set; }
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("structureType")]
        public string structureType { get; set; }
        [JsonProperty("hasChildren")]
        public bool hasChildren { get; set; }
        [JsonProperty("url")]
        public string url { get; set; }
        [JsonProperty("attributes")]
        public ClassificationNodeAttributes attributes { get; set; }
        [JsonProperty("children")]
        public ClassificationNode[] children { get; set; }

        public class ClassificationNodeAttributes
        {
            [JsonProperty("startDate")]
            public DateTime startDate { get; set; }
            [JsonProperty("finishDate")]
            public DateTime finishDate { get; set; }
        }
    }
}
