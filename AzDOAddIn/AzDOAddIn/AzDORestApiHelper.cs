﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using AzDOAddIn.RestApiClasses;
using Microsoft.Office.Interop.MSProject;

namespace AzDOAddIn
{
    public static class AzDORestApiHelper
    {
        static class RequestMethod
        {
            public const string GET = "GET";
            public const string POST = "POST";
            public const string POSTPATCH = "POSTPATCH";
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
            string requestUrl = string.Format("{0}/{1}/_apis/wit/workitems/{2}?$expand=relations&api-version={3}", azDoUrl, teamProject, wiId, RestApiVersion);
            return InvokeRestApiRequest<WorkItem>(RequestMethod.GET, requestUrl, "", pat);
        }

        public static WorkItemsResponse GetWorkItems(string azDoUrl, string teamProject, string wiIds, string pat, string fields = "\"System.Id\",\"System.Title\",\"System.WorkItemType\"")
        {
            string requestBody = string.Format("\"ids\":[{0}],\"fields\":[{1}]", wiIds, fields);
            //string requestBody = string.Format("\"ids\":[{0}],\"fields\":[{1}], \"$expand\":\"relations\"", wiIds, fields);
            requestBody = "{" + requestBody + "}";
            string requestUrl = string.Format("{0}/{1}/_apis/wit/workitemsbatch?api-version={2}", azDoUrl, teamProject, RestApiVersion);
            return InvokeRestApiRequest<WorkItemsResponse>(RequestMethod.POST, requestUrl, requestBody, pat);
        }

        public static WorkItemTypeResponse GetWorkItemTypes(string azDoUrl, string teamProject, string pat)
        {
            string requestUrl = string.Format("{0}/{1}/_apis/wit/workitemtypes?api-version={2}", azDoUrl, teamProject, RestApiVersion);
            return InvokeRestApiRequest<WorkItemTypeResponse>(RequestMethod.GET, requestUrl, "", pat);
        }
        public static WorkItemQueryResult GetWiqlResult(string azDoUrl, string teamProject, string wiqlText, string pat)
        {
            string requestBody = string.Format("\"query\":\"{0}\"", wiqlText);
            requestBody = "{" + requestBody + "}";
            string requestUrl = string.Format("{0}/{1}/_apis/wit/wiql?api-version={2}", azDoUrl, teamProject, RestApiVersion);
            return InvokeRestApiRequest<WorkItemQueryResult>(RequestMethod.POST, requestUrl, requestBody, pat);
        }

        public static List<WorkItem> GetChildWorkItems(string azDoUrl, string teamProject, int wiId, string pat)
        {
            List<WorkItem> childWorkItems = new List<WorkItem>();

            string queryText = string.Format("SELECT [System.Id] FROM WorkItemLinks WHERE ([Source].[System.Id] = {0}) And ([System.Links.LinkType] = 'System.LinkTypes.Hierarchy-Forward') ORDER BY [System.Id] mode(MayContain)", wiId);

            var result = GetWiqlResult(azDoUrl, teamProject, queryText, "");

            if (result.workItemRelations != null && result.workItemRelations.Count() > 0)
                foreach (var relation in result.workItemRelations)
                    if (relation.source != null) childWorkItems.Add(GetWorkItem(azDoUrl, teamProject, relation.target.id, pat));

            return childWorkItems;
        }

        public static WorkItem PublishNewWorkItem(string azDoUrl, string teamProject, string pat, string workItemType, Dictionary<string, string> fields, int parentId = 0)
        {
            string body = "";
            string fieldTemplate = "\"op\": \"add\",\"path\": \"/fields/{0}\", \"from\": null, \"value\": \"{1}\"";
            string relTemplate = "\"rel\": \"{0}\",\"url\": \"{1}/_apis/wit/workItems/{2}\"";

            foreach (string field in fields.Keys)
            {
                string fieldDef = "{" + string.Format(fieldTemplate, field, fields[field]) + "}";

                body = (body == "") ? fieldDef : body + "," + fieldDef;
            }

            if (parentId > 0)
            {
                string relDef = "{\"op\": \"add\",\"path\": \"/relations/-\",\"value\": {" + 
                    string.Format(relTemplate, "System.LinkTypes.Hierarchy-Reverse", azDoUrl, parentId) + "}}";

                body += "," + relDef;
            }

            body = "[" + body + "]";

            return PostWorkItem(azDoUrl, teamProject, pat, workItemType, body);
        }

        public static WorkItem PostWorkItem(string azDoUrl, string teamProject, string pat, string workItemType, string body)
        {
            string requestUrl = string.Format("{0}/{1}/_apis/wit/workitems/${2}?api-version={3}", azDoUrl, teamProject, workItemType,RestApiVersion);

            return InvokeRestApiRequest<WorkItem>(RequestMethod.POSTPATCH, requestUrl, body, pat);
        }

        static T InvokeRestApiRequest<T>(string requestMethod, string requestUrl, string requestBody = "", string pat = "")
        {
            HttpResponseMessage requestResponse = null;
            string responceContent = null;

            HttpClient httpClient;
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
                case RequestMethod.POST:
                    requestResponse = httpClient.PostAsync(requestUrl, new StringContent(requestBody, Encoding.UTF8, "application/json")).Result;
                    break;
                case RequestMethod.POSTPATCH:
                    requestResponse = httpClient.PostAsync(requestUrl, new StringContent(requestBody, Encoding.UTF8, "application/json-patch+json")).Result;
                    break;
            }

            httpClient.Dispose();

            if (requestResponse != null)
            {
                if (requestResponse.IsSuccessStatusCode)
                    responceContent = requestResponse.Content.ReadAsStringAsync().Result;
                else
                {
                    responceContent = requestResponse.Content.ReadAsStringAsync().Result;
                    throw new System.Exception(responceContent);
#warning process errors here!!
                }
            }

            return JsonConvert.DeserializeObject<T>(responceContent);
        }
    }
}
