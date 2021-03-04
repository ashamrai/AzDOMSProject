namespace AzDOAddIn
{
    partial class RibbonPanel : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public RibbonPanel()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RibbonPanel));
            this.tab1 = this.Factory.CreateRibbonTab();
            this.group_Sync = this.Factory.CreateRibbonGroup();
            this.btn_LinkToTeamProject = this.Factory.CreateRibbonButton();
            this.btn_PublishWorkItems = this.Factory.CreateRibbonButton();
            this.btn_UpdatePlan = this.Factory.CreateRibbonButton();
            this.btnGetWorkItems = this.Factory.CreateRibbonButton();
            this.btnImportChilds = this.Factory.CreateRibbonButton();
            this.group_Settings = this.Factory.CreateRibbonGroup();
            this.btn_ImportTeamMembers = this.Factory.CreateRibbonButton();
            this.btn_AddColumns = this.Factory.CreateRibbonButton();
            this.tab1.SuspendLayout();
            this.group_Sync.SuspendLayout();
            this.group_Settings.SuspendLayout();
            this.SuspendLayout();
            // 
            // tab1
            // 
            this.tab1.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tab1.Groups.Add(this.group_Sync);
            this.tab1.Groups.Add(this.group_Settings);
            this.tab1.Label = "Azure DevOps Work Items";
            this.tab1.Name = "tab1";
            // 
            // group_Sync
            // 
            this.group_Sync.Items.Add(this.btn_LinkToTeamProject);
            this.group_Sync.Items.Add(this.btn_PublishWorkItems);
            this.group_Sync.Items.Add(this.btn_UpdatePlan);
            this.group_Sync.Items.Add(this.btnGetWorkItems);
            this.group_Sync.Items.Add(this.btnImportChilds);
            this.group_Sync.Label = "Sync Work Items";
            this.group_Sync.Name = "group_Sync";
            // 
            // btn_LinkToTeamProject
            // 
            this.btn_LinkToTeamProject.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btn_LinkToTeamProject.Image = ((System.Drawing.Image)(resources.GetObject("btn_LinkToTeamProject.Image")));
            this.btn_LinkToTeamProject.Label = "Link To Team Project";
            this.btn_LinkToTeamProject.Name = "btn_LinkToTeamProject";
            this.btn_LinkToTeamProject.ShowImage = true;
            this.btn_LinkToTeamProject.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btn_LinkToTeamProject_Click);
            // 
            // btn_PublishWorkItems
            // 
            this.btn_PublishWorkItems.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btn_PublishWorkItems.Image = ((System.Drawing.Image)(resources.GetObject("btn_PublishWorkItems.Image")));
            this.btn_PublishWorkItems.Label = "Publish Work Items";
            this.btn_PublishWorkItems.Name = "btn_PublishWorkItems";
            this.btn_PublishWorkItems.ShowImage = true;
            this.btn_PublishWorkItems.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btn_PublishWorkItems_Click);
            // 
            // btn_UpdatePlan
            // 
            this.btn_UpdatePlan.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btn_UpdatePlan.Image = global::AzDOAddIn.Properties.Resources.inbox;
            this.btn_UpdatePlan.Label = "Update Plan";
            this.btn_UpdatePlan.Name = "btn_UpdatePlan";
            this.btn_UpdatePlan.ShowImage = true;
            this.btn_UpdatePlan.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btn_UpdatePlan_Click);
            // 
            // btnGetWorkItems
            // 
            this.btnGetWorkItems.Image = global::AzDOAddIn.Properties.Resources.checklist;
            this.btnGetWorkItems.Label = "Get Work Items";
            this.btnGetWorkItems.Name = "btnGetWorkItems";
            this.btnGetWorkItems.ShowImage = true;
            this.btnGetWorkItems.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnGetWorkItems_Click);
            // 
            // btnImportChilds
            // 
            this.btnImportChilds.Image = global::AzDOAddIn.Properties.Resources.hierarchical_structure;
            this.btnImportChilds.Label = "Import Childs";
            this.btnImportChilds.Name = "btnImportChilds";
            this.btnImportChilds.ShowImage = true;
            this.btnImportChilds.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnImportChilds_Click);
            // 
            // group_Settings
            // 
            this.group_Settings.Items.Add(this.btn_ImportTeamMembers);
            this.group_Settings.Items.Add(this.btn_AddColumns);
            this.group_Settings.Label = "Settings";
            this.group_Settings.Name = "group_Settings";
            // 
            // btn_ImportTeamMembers
            // 
            this.btn_ImportTeamMembers.Image = global::AzDOAddIn.Properties.Resources.user;
            this.btn_ImportTeamMembers.Label = "Import Team Members";
            this.btn_ImportTeamMembers.Name = "btn_ImportTeamMembers";
            this.btn_ImportTeamMembers.ShowImage = true;
            this.btn_ImportTeamMembers.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btn_ImportTeamMembers_Click);
            // 
            // btn_AddColumns
            // 
            this.btn_AddColumns.Image = global::AzDOAddIn.Properties.Resources.volume_control;
            this.btn_AddColumns.Label = "Add Columns";
            this.btn_AddColumns.Name = "btn_AddColumns";
            this.btn_AddColumns.ShowImage = true;
            this.btn_AddColumns.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btn_AddColumns_Click);
            // 
            // RibbonPanel
            // 
            this.Name = "RibbonPanel";
            this.RibbonType = "Microsoft.Project.Project";
            this.Tabs.Add(this.tab1);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.RibbonPanel_Load);
            this.tab1.ResumeLayout(false);
            this.tab1.PerformLayout();
            this.group_Sync.ResumeLayout(false);
            this.group_Sync.PerformLayout();
            this.group_Settings.ResumeLayout(false);
            this.group_Settings.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tab1;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group_Sync;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btn_LinkToTeamProject;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btn_PublishWorkItems;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btn_UpdatePlan;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group_Settings;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btn_AddColumns;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnGetWorkItems;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnImportChilds;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btn_ImportTeamMembers;
    }

    partial class ThisRibbonCollection
    {
        internal RibbonPanel RibbonPanel
        {
            get { return this.GetRibbon<RibbonPanel>(); }
        }
    }
}
