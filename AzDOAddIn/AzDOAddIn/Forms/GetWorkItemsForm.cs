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
    public partial class GetWorkItemsForm : Form
    {
        public string TeamProject { get; set; }
        public string OrgUrl { get; set; }

        public List<int> WiIds
        {
            get
            {
                List<int> wiIds = new List<int>();

                if (lstBoxResult.SelectedItems.Count > 0)
                {
                    foreach(string item in lstBoxResult.SelectedItems)
                    {
                        int wiId = ParseIdFromResult(item);
                        if (wiId > 0) wiIds.Add(wiId);
                    }
                }

                return wiIds;

            }
        }

        int ParseIdFromResult(string stringToParse)
        {
            int wiId = 0;
            string[] parseArray = stringToParse.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

            if (parseArray.Count() > 2)
                if (!int.TryParse(parseArray[0].Trim(), out wiId))
                    return 0;

            return wiId;
        }

        public GetWorkItemsForm()
        {
            InitializeComponent();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            var workItems = AzDORestApiHelper.GetWorkItems(ProjectOperations.ActiveOrgUrl, ProjectOperations.ActiveTeamProject, txtBoxIds.Text, ProjectOperations.ActivePAT);

            if (workItems.count > 0)
            {
                foreach (var workItem in workItems.WorkItems)
                    lstBoxResult.Items.Add(String.Format("{0, 7} : {1, 10} : {2} ", 
                        workItem.fields["System.Id"], workItem.fields["System.WorkItemType"], workItem.fields["System.Title"]));
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (lstBoxResult.SelectedItems.Count > 0)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
                MessageBox.Show("Mark work items to import", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
