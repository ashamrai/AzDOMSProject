using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using MSProject = Microsoft.Office.Interop.MSProject;
using Office = Microsoft.Office.Core;

namespace AzDOAddIn
{
    public partial class ThisAddIn
    {
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            this.Application.WindowDeactivate += Application_WindowDeactivate;
            this.Application.NewProject += Application_NewProject;

            ProjectOperations.AppObj = this.Application;
            bool l = ProjectOperations.TeamProjectLinked;
        }

        private void Application_NewProject(MSProject.Project pj)
        {
            bool l = ProjectOperations.TeamProjectLinked;

            
        }

        private void Application_WindowDeactivate(MSProject.Window deactivatedWindow)
        {
            bool l = ProjectOperations.TeamProjectLinked;
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion
    }
}
