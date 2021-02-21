using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzDOAddIn.RestApiClasses
{

    public class WorkItemTypeResponse
    {
        [JsonProperty("count")]
        public int count { get; set; }
        [JsonProperty("value")]
        public WorkItemType[] WorkItemTypes { get; set; }
    }

    public class WorkItemType
    {
        [JsonProperty("color")]
        public string color { get; set; }
        [JsonProperty("description")]
        public string description { get; set; }
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("referenceName")]
        public string referenceName { get; set; }
        [JsonProperty("url")]
        public string url { get; set; }
        [JsonProperty("xmlForm")]
        public string xmlForm { get; set; }
        [JsonProperty("_links")]
        public ReferenceLinks _links { get; set; }
        [JsonProperty("fieldInstances")]
        public WorkItemTypeFieldInstance[] fieldInstances { get; set; }
        [JsonProperty("fields")]
        public WorkItemTypeFieldInstance[] fields { get; set; }
        [JsonProperty("icon")]
        public WorkItemIcon icon { get; set; }
        [JsonProperty("isDisabled")]
        public bool isDisabled { get; set; }
        [JsonProperty("states")]
        public WorkItemStateColor[] states { get; set; }
        [JsonProperty("transitions")]
        public object transitions { get; set; }

        public class ReferenceLinks
        {
            [JsonProperty("links")]
            public object links { get; set; }
        }

        public class WorkItemStateColor
        {
            [JsonProperty("category")]
            public string category { get; set; }
            [JsonProperty("color")]
            public string color { get; set; }
            [JsonProperty("name")]
            public string name { get; set; }
        }

        public class WorkItemIcon
        {
            [JsonProperty("id")]
            public string id { get; set; }
            [JsonProperty("url")]
            public string url { get; set; }
        }

        public class WorkItemTypeFieldInstance
        {
            [JsonProperty("allowedValues")]
            public string[] allowedValues { get; set; }
            [JsonProperty("alwaysRequired")]
            public bool alwaysRequired { get; set; }
            [JsonProperty("defaultValue")]
            public string defaultValue { get; set; }
            [JsonProperty("dependentFields")]
            public WorkItemFieldReference[] dependentFields { get; set; }
            [JsonProperty("helpText")]
            public string helpText { get; set; }
            [JsonProperty("name")]
            public string name { get; set; }
            [JsonProperty("referenceName")]
            public string referenceName { get; set; }
            [JsonProperty("url")]
            public string url { get; set; }
        }

        public class WorkItemFieldReference
        {
            [JsonProperty("name")]
            public string name { get; set; }
            [JsonProperty("referenceName")]
            public string referenceName { get; set; }
            [JsonProperty("url")]
            public string url { get; set; }
        }
    }
}
