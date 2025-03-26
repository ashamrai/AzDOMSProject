using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AzDOAddIn.Forms
{
    public partial class SettingsForm : Form
    {
        public bool useSprintStartDate = false;
        public bool savePlan = false;
        public string workItemTag = string.Empty;
        public SettingsForm()
        {
            InitializeComponent();            
        }

        public void UpdateSettingsValues(PlanningSettings planningSettings, OperationalSettings operationalSettings)
        {
            if (planningSettings != null)
            {
                chkUseSprintStartDate.Checked = planningSettings.useSprintStartDate;
            }

            if (operationalSettings != null)
            {
                chkSavePlan.Checked = operationalSettings.savePlan;
                chkTag.Checked = !string.IsNullOrEmpty(operationalSettings.workItemTag);
                txtWorkItemTag.Enabled = chkTag.Checked;
                txtWorkItemTag.Text = operationalSettings.workItemTag;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            useSprintStartDate = chkUseSprintStartDate.Checked;
            savePlan = chkSavePlan.Checked;

            if (chkTag.Checked) workItemTag = txtWorkItemTag.Text;
            else workItemTag = string.Empty;


            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void chkTag_CheckedChanged(object sender, EventArgs e)
        {
            if (chkTag.Checked) { txtWorkItemTag.Enabled = true; txtWorkItemTag.Text = "msproject"; }
            else { txtWorkItemTag.Enabled = false; txtWorkItemTag.Text = string.Empty; }
        }
    }
}
