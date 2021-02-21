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
    public partial class WorkItemTypes : Form
    {
        public void AddWorkItemTypeToList(string name)
        {
            lstBoxWiTypes.Items.Add(name);
        }

        public string SelectedItems()
        {
            string selectedItemsString = "";

            if (lstBoxWiTypes.SelectedItems.Count > 0)
                foreach (string item in lstBoxWiTypes.SelectedItems)
                    selectedItemsString += item + ";";

            return selectedItemsString;
        }

        public WorkItemTypes()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
