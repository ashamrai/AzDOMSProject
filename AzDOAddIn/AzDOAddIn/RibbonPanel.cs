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
            ProjectOperations.UpdateProjectPlan();
        }

        private void btnGetWorkItems_Click(object sender, RibbonControlEventArgs e)
        {
            Forms.GetWorkItemsForm getWorkItemsForm = new Forms.GetWorkItemsForm();
            if (getWorkItemsForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ProjectOperations.AddWorkItemsToPlan(getWorkItemsForm.WiIds);
            }
        }

        private void btnImportChilds_Click(object sender, RibbonControlEventArgs e)
        {
            ProjectOperations.ImportChilds();
        }

        private void btn_PublishWorkItems_Click(object sender, RibbonControlEventArgs e)
        {
            ProjectOperations.PublishProjectPlan();
        }

        private void btn_ImportTeamMembers_Click(object sender, RibbonControlEventArgs e)
        {
            ProjectOperations.ImportTeamMembers();
        }
    }
}
