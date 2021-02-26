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
    public partial class WndLinkToTeamProject : Form
    {
        public string Url { get { return cmdAzDoUrl.Text; } }
        public string TeamProject { get { return cmdBox_TeamProjects.Text; } }
        public string PAT { get { return txtBox_PAT.Text; } set { txtBox_PAT.Text = value; } }

        public WndLinkToTeamProject()
        {
            InitializeComponent();

            List<string> azdoUrls = PatHelper.GetStoredUrls();

            if (azdoUrls.Count > 0)
                foreach (string url in azdoUrls) cmdAzDoUrl.Items.Add(url);
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btn_GetTeamProjects_Click(object sender, EventArgs e)
        {
            var projects = AzDORestApiHelper.GetTeamProjects(Url, PAT);

            cmdBox_TeamProjects.Items.Clear();

            foreach (var project in projects.TeamProjects) cmdBox_TeamProjects.Items.Add(project.name);

            if (cmdBox_TeamProjects.Items.Count > 0)
            {
                cmdAzDoUrl.Enabled = false;
                txtBox_PAT.Enabled = false;
            }
        }

        private void cmdAzDoUrl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Url != "")
                PAT = PatHelper.GetPat(Url);
        }
    }
}
