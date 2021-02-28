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
    public partial class Teams : Form
    {
        public void AddTeam(string team)
        {
            lst_Teams.Items.Add(team);
        }

        public string GetTeam()
        {
            return lst_Teams.SelectedItem.ToString();
        }

        public Teams()
        {
            InitializeComponent();
        }

        private void btn_Ok_Click(object sender, EventArgs e)
        {
            if (lst_Teams.SelectedIndex < 0) return;

            DialogResult = DialogResult.OK;

            Close();
        }
    }
}
