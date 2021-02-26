namespace AzDOAddIn.Forms
{
    partial class WndLinkToTeamProject
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtBox_PAT = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmdBox_TeamProjects = new System.Windows.Forms.ComboBox();
            this.btn_GetTeamProjects = new System.Windows.Forms.Button();
            this.btn_OK = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.cmdAzDoUrl = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(225, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Azure DevOps Service/Server Collection URL";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(120, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Personal Access Token";
            // 
            // txtBox_PAT
            // 
            this.txtBox_PAT.Location = new System.Drawing.Point(255, 41);
            this.txtBox_PAT.Name = "txtBox_PAT";
            this.txtBox_PAT.PasswordChar = '*';
            this.txtBox_PAT.Size = new System.Drawing.Size(368, 20);
            this.txtBox_PAT.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Team Project Name";
            // 
            // cmdBox_TeamProjects
            // 
            this.cmdBox_TeamProjects.FormattingEnabled = true;
            this.cmdBox_TeamProjects.Location = new System.Drawing.Point(255, 69);
            this.cmdBox_TeamProjects.Name = "cmdBox_TeamProjects";
            this.cmdBox_TeamProjects.Size = new System.Drawing.Size(240, 21);
            this.cmdBox_TeamProjects.TabIndex = 5;
            // 
            // btn_GetTeamProjects
            // 
            this.btn_GetTeamProjects.Location = new System.Drawing.Point(501, 68);
            this.btn_GetTeamProjects.Name = "btn_GetTeamProjects";
            this.btn_GetTeamProjects.Size = new System.Drawing.Size(122, 23);
            this.btn_GetTeamProjects.TabIndex = 6;
            this.btn_GetTeamProjects.Text = "Get Team Projects";
            this.btn_GetTeamProjects.UseVisualStyleBackColor = true;
            this.btn_GetTeamProjects.Click += new System.EventHandler(this.btn_GetTeamProjects_Click);
            // 
            // btn_OK
            // 
            this.btn_OK.Location = new System.Drawing.Point(188, 112);
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Size = new System.Drawing.Size(75, 23);
            this.btn_OK.TabIndex = 7;
            this.btn_OK.Text = "Ok";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btn_Cancel.Location = new System.Drawing.Point(313, 112);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
            this.btn_Cancel.TabIndex = 8;
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            // 
            // cmdAzDoUrl
            // 
            this.cmdAzDoUrl.FormattingEnabled = true;
            this.cmdAzDoUrl.Location = new System.Drawing.Point(255, 14);
            this.cmdAzDoUrl.Name = "cmdAzDoUrl";
            this.cmdAzDoUrl.Size = new System.Drawing.Size(368, 21);
            this.cmdAzDoUrl.TabIndex = 9;
            this.cmdAzDoUrl.SelectedIndexChanged += new System.EventHandler(this.cmdAzDoUrl_SelectedIndexChanged);
            // 
            // WndLinkToTeamProject
            // 
            this.AcceptButton = this.btn_OK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btn_Cancel;
            this.ClientSize = new System.Drawing.Size(636, 147);
            this.ControlBox = false;
            this.Controls.Add(this.cmdAzDoUrl);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_OK);
            this.Controls.Add(this.btn_GetTeamProjects);
            this.Controls.Add(this.cmdBox_TeamProjects);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtBox_PAT);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "WndLinkToTeamProject";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LinkToTeamProject";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtBox_PAT;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmdBox_TeamProjects;
        private System.Windows.Forms.Button btn_GetTeamProjects;
        private System.Windows.Forms.Button btn_OK;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.ComboBox cmdAzDoUrl;
    }
}