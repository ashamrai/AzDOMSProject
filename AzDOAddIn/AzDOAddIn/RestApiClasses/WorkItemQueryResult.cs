using Newtonsoft.Json;

namespace AzDOAddIn.RestApiClasses
{
    public class WorkItemQueryResult
    {
        [JsonProperty("asOf")]
        public string asOf { get; set; }
        [JsonProperty("columns")]
        public WorkItemFieldReference[] columns { get; set; }
        [JsonProperty("queryResultType")]
        public string queryResultType { get; set; }
        [JsonProperty("queryType")]
        public string queryType { get; set; }
        [JsonProperty("sortColumns")]
        public WorkItemQuerySortColumn[] sortColumns { get; set; }
        [JsonProperty("workItemRelations")]
        public WorkItemLink[] workItemRelations { get; set; }
        [JsonProperty("workItems")]
        public WorkItemReference[] workItems { get; set; }

        public class WorkItemFieldReference
        {
            [JsonProperty("name")]
            public string name { get; set; }
            [JsonProperty("referenceName")]
            public string referenceName { get; set; }
            [JsonProperty("url")]
            public string url { get; set; }
        }

        public class WorkItemQuerySortColumn
        {
            [JsonProperty("descending")]
            public bool descending { get; set; }
            [JsonProperty("field")]
            public WorkItemFieldReference field { get; set; }
        }

        public class WorkItemLink
        {
            [JsonProperty("rel")]
            public string rel { get; set; }
            [JsonProperty("source")]
            public WorkItemReference source { get; set; }
            [JsonProperty("target")]
            public WorkItemReference target { get; set; }            
        }

        public class WorkItemReference
        {
            [JsonProperty("id")]
            public int id { get; set; }
            [JsonProperty("url")]
            public string url { get; set; }
        }
    }
}
