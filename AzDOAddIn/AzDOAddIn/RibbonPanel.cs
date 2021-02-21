using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;

namespace AzDOAddIn
{
    public partial class RibbonPanel
    {
        private void RibbonPanel_Load(object sender, RibbonUIEventArgs e)
        {

        }

        private void btn_LinkToTeamProject_Click(object sender, RibbonControlEventArgs e)
        {
            ProjectOperations.LinkToTeamProject();
        }

        private void btn_AddColumns_Click(object sender, RibbonControlEventArgs e)
        {
            ProjectOperations.UpdateView();
        }

        private void btn_UpdatePlan_Click(object sender, RibbonControlEventArgs e)
        {
            //var wi = AzDORestApiHelper.GetWorkItem(AzDORestApiHelper.ORG, "TFSAgile", 689, AzDORestApiHelper.PAT);
        }
    }
}
