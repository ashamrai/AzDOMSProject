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
        public SettingsForm()
        {
            InitializeComponent();            
        }

        public void UpdateSettingsValues(PlanningSettings planningSettings)
        {
            if (planningSettings != null)
            {
                chkUseSprintStartDate.Checked = planningSettings.useSprintStartDate;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            useSprintStartDate = chkUseSprintStartDate.Checked;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
