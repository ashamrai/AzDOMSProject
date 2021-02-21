using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AzDOAddIn.RestApiClasses;

namespace AzDOAddIn
{
    public static class AzDORestApiHelper
    {
        static class RequestMethod
        {
            public const string GET = "GET";
            public const string POST = "POST";
            public const string PATCH = "PATCH";
        }

        public static string RestApiVersion = "5.0";

        public static TeamProjectsResponse GetTeamProjects(string azDoUrl, string pat)
        {
            string requestUrl = string.Format("{0}/_apis/projects?api-version={1}", azDoUrl, RestApiVersion);
            return InvokeRestApiRequest<TeamProjectsResponse>(RequestMethod.GET, requestUrl, "", pat);
        }

        public static WorkItem GetWorkItem(string azDoUrl, string teamProject, int wiId, string pat)
        {
            string requestUrl = string.Format("{0}/{1}/_apis/wit/workitems/{2}?api-version={3}", azDoUrl, teamProject, wiId, RestApiVersion);
            return InvokeRestApiRequest<WorkItem>(RequestMethod.GET, requestUrl, "", pat);
        }

        public static WorkItemTypeResponse GetWorkItemTypes(string azDoUrl, string teamProject, string pat)
        {
            string requestUrl = string.Format("{0}/{1}/_apis/wit/workitemtypes?api-version={2}", azDoUrl, teamProject, RestApiVersion);
            return InvokeRestApiRequest<WorkItemTypeResponse>(RequestMethod.GET, requestUrl, "", pat);
        }

        static T InvokeRestApiRequest<T>(string requestMethod, string requestUrl, string requestBody = "", string pat = "")
        {
            HttpClient httpClient = null;            
            HttpResponseMessage requestResponse = null;
            string responceContent = null;

            if (pat != "")            
            {
                httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", "", pat))));
            }
            else
            {
                HttpClientHandler httpClientHandler = new HttpClientHandler();
                httpClientHandler.UseDefaultCredentials = true;
                httpClient = new HttpClient(httpClientHandler);
            }


            switch (requestMethod.ToUpper())
            {
                case RequestMethod.GET:
                    requestResponse = httpClient.GetAsync(requestUrl).Result;
                    break;
            }

            httpClient.Dispose();

            if (requestResponse != null)
            {
                if (requestResponse.IsSuccessStatusCode)
                    responceContent = requestResponse.Content.ReadAsStringAsync().Result;
                else
                {
#warning process errors here!!
                }
            }

            return JsonConvert.DeserializeObject<T>(responceContent);
        }
    }
}
