namespace AzDOAddIn.Forms
{
    partial class SettingsForm
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
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpPlanning = new System.Windows.Forms.GroupBox();
            this.chkUseSprintStartDate = new System.Windows.Forms.CheckBox();
            this.grpPlanning.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnSave.Location = new System.Drawing.Point(156, 206);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(303, 206);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // grpPlanning
            // 
            this.grpPlanning.Controls.Add(this.chkUseSprintStartDate);
            this.grpPlanning.Location = new System.Drawing.Point(3, 12);
            this.grpPlanning.Name = "grpPlanning";
            this.grpPlanning.Size = new System.Drawing.Size(274, 100);
            this.grpPlanning.TabIndex = 2;
            this.grpPlanning.TabStop = false;
            this.grpPlanning.Text = "Planning";
            // 
            // chkUseSprintStartDate
            // 
            this.chkUseSprintStartDate.AutoSize = true;
            this.chkUseSprintStartDate.Location = new System.Drawing.Point(6, 19);
            this.chkUseSprintStartDate.Name = "chkUseSprintStartDate";
            this.chkUseSprintStartDate.Size = new System.Drawing.Size(237, 17);
            this.chkUseSprintStartDate.TabIndex = 3;
            this.chkUseSprintStartDate.Text = "Use a sprint start date for imported workitems";
            this.chkUseSprintStartDate.UseVisualStyleBackColor = true;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(566, 241);
            this.Controls.Add(this.grpPlanning);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.grpPlanning.ResumeLayout(false);
            this.grpPlanning.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox grpPlanning;
        private System.Windows.Forms.CheckBox chkUseSprintStartDate;
    }
}