using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AzDOAddIn
{
    public class PlanningSettings
    {
        [JsonProperty("sprintstart")] 
        public bool useSprintStartDate { get; set; }        
    }

    public class OperationalSettings
    {
        [JsonProperty("saveplan")]
        public bool savePlan { get; set; }

        [JsonProperty("witag")]
        public string workItemTag { get; set; }
    }
}
