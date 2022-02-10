using System;
using System.Security;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.ProjectServer.Client;

namespace ProjectListFApp
{
    public static class Function1
    {
        [Function("Function1")]
        public static void Run([TimerTrigger("0 */1 * * * *")] MyInfo myTimer, FunctionContext context)
        {
            var logger = context.GetLogger("Function1");           

            // Note: The PnP Sites Core AuthenticationManager class also supports this
            using (var authenticationManager = new AuthenticationManager())
            {
                Uri site = new Uri("https://<org_name>.sharepoint.com/sites/<project_instance>");
                string user = "<user_email>";
                string password = "<password>";

                using (var clientContext = authenticationManager.GetContext(site, user, password))
                {
                    clientContext.Load(clientContext.Web, p => p.Title);
                    clientContext.ExecuteQueryAsync().Wait();
                    logger.LogInformation($"Title: {clientContext.Web.Title}");

                    var projects = clientContext.Projects;
                    int j = 0;
                    clientContext.Load(projects);
                    clientContext.ExecuteQuery();
                    foreach (PublishedProject pubProj in clientContext.Projects)
                    {
                        logger.LogInformation("\n{0}. {1}   {2} \t{3} \n", j++, pubProj.Id, pubProj.Name, pubProj.CreatedDate);
                    }
                }
            }


            logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
        }
    }

    public class MyInfo
    {
        public MyScheduleStatus ScheduleStatus { get; set; }

        public bool IsPastDue { get; set; }
    }

    public class MyScheduleStatus
    {
        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
