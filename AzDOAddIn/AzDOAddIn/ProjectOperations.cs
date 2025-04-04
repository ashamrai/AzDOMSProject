﻿using Microsoft.Office.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSProject = Microsoft.Office.Interop.MSProject;
using Office = Microsoft.Office.Core;
using AzDOAddIn.AzDOAddInSettings;
using System.Windows.Forms;
using Newtonsoft.Json;
using AzDOAddIn.RestApiClasses;
using Microsoft.Office.Interop.MSProject;

namespace AzDOAddIn
{
    static class ProjectOperations
    {
        static MSProject.Project ActiveProject { get { return AppObj.ActiveProject; } }
        static MSProject.Task SelectedTask { get { return (AppObj.ActiveSelection.Tasks == null) ? null : AppObj.ActiveSelection.Tasks[1]; } }
        static DocumentProperties DocPropeties { get { return ActiveProject.CustomDocumentProperties; } }
        static internal string ActiveTeamProject { get { return GetDocProperty(PlanDocProperties.AzDoTeamProject); } }
        static internal string ActiveOrgUrl { get { return GetDocProperty(PlanDocProperties.AzDoUrl); } }
        static internal string ActivePAT { get { return PatHelper.GetPat(ActiveOrgUrl); } }

        static class SyncSettings
        {
            public static bool useSprintStartDate = false;
            public static bool savePlan = true;
            public static string workItemTag = "";
            public static string workItemTypes = "";
        }

        internal static MSProject.Application AppObj { get; set; }        

        internal static bool TeamProjectLinked
        {
            get
            {
                if (AppObj == null) return false;

                if (ActiveProject == null) return false;

                if (CheckDocProperty(PlanDocProperties.AzDoUrl) > 0)
                {

                }
                else return false;

                if (CheckDocProperty(PlanDocProperties.AzDoTeamProject) > 0)
                {

                }
                else return false;

                return true;
            }
        }

        public static PlanningSettings GetPlanSettings()
        {
            if (CheckDocProperty(PlanDocProperties.PlanningSettings) == 0)
                return null;

            string planningJson = GetDocProperty(PlanDocProperties.PlanningSettings);

            PlanningSettings planningSettings = JsonConvert.DeserializeObject<PlanningSettings>(planningJson);

            SyncSettings.useSprintStartDate = planningSettings.useSprintStartDate;

            return planningSettings;
        }

        public static OperationalSettings GetOperationalSettings()
        {
            if (CheckDocProperty(PlanDocProperties.PlanningSettings) == 0)
                return null;

            string operationalJson = GetDocProperty(PlanDocProperties.OperationalSettings);

            OperationalSettings operationalSettings = JsonConvert.DeserializeObject<OperationalSettings>(operationalJson);

            SyncSettings.savePlan= operationalSettings.savePlan;
            SyncSettings.workItemTag = operationalSettings.workItemTag;

            return operationalSettings;
        }

        private static bool CheckUserInResources(string name)
        {
            if (ActiveProject.Resources.Count == 0) return false;

            for (int i = 1; i <= ActiveProject.Resources.Count; i++)
                if (ActiveProject.Resources[i].Name == name)
                    return true;

            return false;
        }

        internal static void UpdateProjectPlan()
        {
            AppObj.StatusBar = "Update Project Plan";

            try
            {                
                for (int i = 1; i <= ActiveProject.Tasks.Count; i++)
                {
                    int wiPrjId = GetProjectTaskWorkItemId(ActiveProject.Tasks[i]);

                    if (wiPrjId == 0) continue;

                    var workItem = AzDORestApiHelper.GetWorkItem(ActiveOrgUrl, ActiveTeamProject, wiPrjId, ActivePAT);
                    AddCoreFields(ActiveProject.Tasks[i], workItem);
                    AddWork(ActiveProject.Tasks[i], workItem);
                }
            }
            catch(System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            AppObj.StatusBar = "Ready";
        }

        private static void AddWork(MSProject.Task task, RestApiClasses.WorkItem workItem)
        {
            if (workItem.fields.ContainsKey(WorkItemWorkFileds.Completed))
            {
                task.ActualWork = (double)workItem.fields[WorkItemWorkFileds.Completed] * 60;
            }
        }

        internal static void PublishProjectPlan()
        {
            try
            {
                GetSettings();

                for (int i = 1; i <= ActiveProject.Tasks.Count; i++)
                {
                    int wiPrjId = GetProjectTaskWorkItemId(ActiveProject.Tasks[i]);

                    if (wiPrjId > 0) { PublishChanges(ActiveProject.Tasks[i]); }
                    else { PublishNewWorkItem(ActiveProject.Tasks[i]); }
                }

                if (SyncSettings.savePlan) ActiveProject.Application.FileSave();
            }
            catch(System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void PublishChanges(MSProject.Task task)
        {
            AppObj.StatusBar = "Publish Changes to Azure DevOps";

            try
            {
                string wiName = task.Name;
                if (string.IsNullOrEmpty(wiName)) throw new System.Exception(string.Format("Task [{0}] without name", task.ID));

                if (WorkItemTreeInConsistency(task))
                {
                    int wiId = GetProjectTaskWorkItemId(task);
                    Dictionary<string, string> fields = new Dictionary<string, string>();
                    fields.Add(PlanCoreColumns.WITitle.AzDORefName, wiName);
                    AddBaselineDates(task, fields);

                    if (task.Resources.Count == 1) fields.Add(PlanCoreColumns.WIAssignedTo.AzDORefName, task.Resources[1].Name);

                    string area = GetStringFieldValue(task, PlanCoreColumns.WIArea.PjValue);
                    string iteration = GetStringFieldValue(task, PlanCoreColumns.WIIteration.PjValue);

                    if (!string.IsNullOrEmpty(area)) fields.Add(PlanCoreColumns.WIArea.AzDORefName, ActiveTeamProject + "\\\\" + area.Replace("\\", "\\\\"));
                    if (!string.IsNullOrEmpty(iteration)) fields.Add(PlanCoreColumns.WIIteration.AzDORefName, ActiveTeamProject + "\\\\" + iteration.Replace("\\", "\\\\"));

                    var workItem = AzDORestApiHelper.UpdateWorkItem(ActiveOrgUrl, ActiveTeamProject, ActivePAT, wiId, fields);

                    if (workItem.fields.ContainsKey(WorkItemSystemFileds.Parent) && task.OutlineLevel == 1)
                        workItem = AzDORestApiHelper.RemoveParentLink(ActiveOrgUrl, ActiveTeamProject, ActivePAT, wiId);

                    if(task.OutlineLevel > 1)
                    {
                        int parentWiId = GetProjectTaskWorkItemId(task.OutlineParent);

                        if (parentWiId > 0)
                        {
                            if (!workItem.fields.ContainsKey(WorkItemSystemFileds.Parent))
                                workItem = AzDORestApiHelper.AddParentLink(ActiveOrgUrl, ActiveTeamProject, ActivePAT, wiId, parentWiId);
                            else
                                if (parentWiId != (long)workItem.fields[WorkItemSystemFileds.Parent])
                                {
                                    AzDORestApiHelper.RemoveParentLink(ActiveOrgUrl, ActiveTeamProject, ActivePAT, wiId);
                                    workItem = AzDORestApiHelper.AddParentLink(ActiveOrgUrl, ActiveTeamProject, ActivePAT, wiId, parentWiId);
                                }
                        }
                    }

                    AddCoreFields(task, workItem);
                }

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            AppObj.StatusBar = "Ready";
        }

        internal static void ImportTeamMembers()
        {
            try
            {
                var teams = AzDORestApiHelper.GetTeams(ActiveOrgUrl, ActiveTeamProject, ActivePAT);

                if (teams.count > 0)
                {
                    Forms.Teams teamsForm = new Forms.Teams();

                    foreach (var team in teams.WebApiTeams) teamsForm.AddTeam(team.name);

                    if (teamsForm.ShowDialog() == DialogResult.OK)
                    {
                        int teamMembersCount = 0;
                        var teamMembers = AzDORestApiHelper.GetTeamMembers(ActiveOrgUrl, ActiveTeamProject, teamsForm.GetTeam(), ActivePAT);

                        if (teamMembers.count > 0)
                        {
                            foreach (var teamMember in teamMembers.TeamMembers)
                                if (!CheckUserInResources(teamMember.identity.displayName))
                                {
                                    ActiveProject.Resources.Add(teamMember.identity.displayName);
                                    teamMembersCount++;
                                }
                        }

                        MessageBox.Show(teamMembersCount + " user(s) were imported to the plan", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                if (SyncSettings.savePlan) ActiveProject.Application.FileSave();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void PublishNewWorkItem(MSProject.Task task)
        {
            try
            {
                string wiType = GetProjectTaskWorkItedType(task);
                if (string.IsNullOrEmpty(wiType)) return;

                string wiName = task.Name;
                if (string.IsNullOrEmpty(wiName)) throw new System.Exception(string.Format("Task [{0}] without name", task.ID));

                int parentId = 0;

                if (WorkItemTreeInConsistency(task))
                {
                    Dictionary<string, string> fields = new Dictionary<string, string>();
                    
                    fields.Add(PlanCoreColumns.WITitle.AzDORefName, wiName);
                    if (SyncSettings.workItemTag != "") fields.Add(PlanCoreColumns.WITags.AzDORefName, SyncSettings.workItemTag);


                    AddBaselineDates(task, fields);

                    if (task.Resources.Count == 1) fields.Add(PlanCoreColumns.WIAssignedTo.AzDORefName, task.Resources[1].Name);

                    if (task.OutlineLevel > 1) parentId = GetProjectTaskWorkItemId(task.OutlineParent);

                    string area = GetStringFieldValue(task, PlanCoreColumns.WIArea.PjValue);
                    string iteration = GetStringFieldValue(task, PlanCoreColumns.WIIteration.PjValue);

                    if (!string.IsNullOrEmpty(area)) fields.Add(PlanCoreColumns.WIArea.AzDORefName, ActiveTeamProject + "\\\\" + area.Replace("\\", "\\\\"));
                    if (!string.IsNullOrEmpty(iteration)) fields.Add(PlanCoreColumns.WIIteration.AzDORefName, ActiveTeamProject + "\\\\" + iteration.Replace("\\", "\\\\"));

                    var workItem = AzDORestApiHelper.PublishNewWorkItem(ActiveOrgUrl, ActiveTeamProject, ActivePAT, wiType, fields, parentId);

                    AddCoreFields(task, workItem);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void AddBaselineDates(MSProject.Task task, Dictionary<string, string> fields)
        {
            DateTime start, finish;

            if (!DateTime.TryParse(task.BaselineStartText, out _)) return;
            if (!DateTime.TryParse(task.BaselineFinishText, out _)) return;

            start = (DateTime)task.BaselineStart;
            finish = (DateTime)task.BaselineFinish;

            fields.Add(WorkItemSchedulingFileds.Start, start.ToUniversalTime().ToString("s") + "Z");
            fields.Add(WorkItemSchedulingFileds.Finish, finish.ToUniversalTime().ToString("s") + "Z");

            if (task.BaselineWork > 0)
            {
                double hours = task.BaselineWork / 60;
                fields.Add(WorkItemWorkFileds.OriginalEstimate, hours.ToString().Replace(",", "."));
            }
        }

        private static bool WorkItemTreeInConsistency(MSProject.Task task)
        {
            if (task.OutlineLevel > 2)
            {
                if (GetProjectTaskWorkItemId(task.OutlineParent) != 0) return true;

                MSProject.Task ancestor = task.OutlineParent.OutlineParent;

                do
                {
                    if (GetProjectTaskWorkItemId(ancestor) > 0) 
                        throw new System.Exception(string.Format("Task [{0}]:\"{1}\"\n should have a parent work item in the plan", 
                            task.ID, (task.Name.Length > 20) ? task.Name.Substring(0, 17) + "..." : task.Name));

                    ancestor = ancestor.OutlineParent;

                } while (ancestor.OutlineLevel > 1);
            }

            return true;
        }

        private static int GetProjectTaskWorkItemId(MSProject.Task task)
        {
            return GetIntFieldValue(task, PlanCoreColumns.WIId.PjValue);
        }

        private static string GetProjectTaskWorkItemState(MSProject.Task task)
        {
            return GetStringFieldValue(task, PlanCoreColumns.WIState.PjValue);
        }

        private static string GetProjectTaskWorkItedType(MSProject.Task task)
        {
            return GetStringFieldValue(task, PlanCoreColumns.WIType.PjValue);
        }       


        static int CheckDocProperty(string propertyName)
        {
            for (int i = 1; i <= DocPropeties.Count; i++)
                if (DocPropeties[i].Name == propertyName) 
                    return i;

            return 0;
        }

        static string GetDocProperty(string propertyName)
        {
            int propIndex = CheckDocProperty(propertyName);
            if (propIndex > 0) return DocPropeties[propIndex].Value;
            return "";
        }

        internal static void LinkToTeamProject()
        {
            try
            {
                Forms.WndLinkToTeamProject linkForm = new Forms.WndLinkToTeamProject();

                var formResult = linkForm.ShowDialog();

                if (formResult == System.Windows.Forms.DialogResult.OK)
                {
                    SaveDocSetting(PlanDocProperties.AzDoUrl, linkForm.Url);
                    SaveDocSetting(PlanDocProperties.AzDoTeamProject, linkForm.TeamProject);

                    Forms.WorkItemTypes workItemTypesForm = new Forms.WorkItemTypes();

                    var workItemTypes = AzDORestApiHelper.GetWorkItemTypes(linkForm.Url, linkForm.TeamProject, linkForm.PAT);

                    foreach (var item in workItemTypes.WorkItemTypes)
                        workItemTypesForm.AddWorkItemTypeToList(item.name);

                    if (workItemTypesForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        SaveDocSetting(PlanDocProperties.AzDoWorkItemTypes, workItemTypesForm.SelectedItems());
                        UpdateView();
                    }

                    PatHelper.SetPat(linkForm.Url, linkForm.PAT);
                }
                else
                {
                    if (formResult == System.Windows.Forms.DialogResult.Yes)
                    {
                        PatHelper.SetPat(linkForm.Url, linkForm.PAT);
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void SaveDocSetting(string settingName, string settingValue)
        {
            int propIndex = CheckDocProperty(settingName);

            if (propIndex > 0)
                DocPropeties[propIndex].Value = settingValue;
            else
                DocPropeties.Add(settingName, false, Type: MsoDocProperties.msoPropertyTypeString, Value: settingValue);
        }

        internal static void AddWorkItemsToPlan(List<int> wiIds)
        {
            try
            {
                GetSettings();

                foreach (int wiId in wiIds)
                {
                    var workItem = AzDORestApiHelper.GetWorkItem(ActiveOrgUrl, ActiveTeamProject, wiId, ActivePAT);

                    MSProject.Task projectTask = AddWorkItemToPlan(workItem);
                }

                if (SyncSettings.savePlan) ActiveProject.Application.FileSave();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void GetSettings()
        {
            GetPlanSettings();
            GetOperationalSettings();
            SyncSettings.workItemTypes = GetDocProperty(PlanDocProperties.AzDoWorkItemTypes);
        }

        internal static void ImportChilds()
        {
            try
            {
                GetSettings();

                var currentTask = SelectedTask;

                if (currentTask == null) return;

                int currentWiId = GetIntFieldValue(currentTask, PlanCoreColumns.WIId.PjValue);

                if (currentWiId == 0) return;

                var workItems = AzDORestApiHelper.GetChildWorkItems(ActiveOrgUrl, ActiveTeamProject, currentWiId, ActivePAT);

                if (workItems.Count == 0) return;

                foreach (var workItem in workItems)
                {
                    string wiType = workItem.fields[PlanCoreColumns.WIType.AzDORefName].ToString();
                    if (SyncSettings.workItemTypes.Contains(wiType + ";"))
                        AddWorkItemToPlan(workItem, currentTask);
                }

                if (SyncSettings.savePlan) ActiveProject.Application.FileSave();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static MSProject.Task AddWorkItemToPlan(RestApiClasses.WorkItem workItem, MSProject.Task parentTask = null)
        {
            if (GetPlanTaskByWorkItemId(workItem.id) != null) return null;

            if (parentTask == null) parentTask = FindParentInPlan(workItem);

            int taskInsertPosition = GetTaskChildInsertPosition(workItem, parentTask);

            MSProject.Task projectTask = ActiveProject.Tasks.Add(workItem.fields[PlanCoreColumns.WITitle.AzDORefName],
                (taskInsertPosition == 0) ? Type.Missing : taskInsertPosition);

            AddCoreFields(projectTask, workItem);
            AddPlanFields(projectTask, workItem);

            if (workItem.fields.ContainsKey(PlanCoreColumns.WIAssignedTo.AzDORefName))
            {
                projectTask.ResourceNames = GetUserDisplayName(workItem.fields[PlanCoreColumns.WIAssignedTo.AzDORefName]);
            }

            if (parentTask != null)
                while (projectTask.OutlineLevel <= parentTask.OutlineLevel)
                    projectTask.OutlineIndent();
            else
                while (projectTask.OutlineLevel != 1)
                    projectTask.OutlineOutdent();

            return projectTask;
        }

        private static void AddPlanFields(MSProject.Task projectTask, WorkItem workItem)
        {
            string wiType = workItem.fields[PlanCoreColumns.WIType.AzDORefName].ToString();

            if ((wiType == "Product Backlog Item" || wiType == "Requirement" || wiType == "User Story" || wiType == "Task") && SyncSettings.useSprintStartDate)
            {
                if (workItem.fields[PlanCoreColumns.WIIteration.AzDORefName].ToString() == ActiveTeamProject) return;

                string wiIteration = workItem.fields[PlanCoreColumns.WIIteration.AzDORefName].ToString().Replace(ActiveTeamProject + "\\", "");
                var wiIterationValue = AzDORestApiHelper.GetIterationNode(ActiveOrgUrl, ActiveTeamProject, ActivePAT, wiIteration);

                if (wiIterationValue.attributes != null)
                {
                    if (wiIterationValue.attributes.startDate != null)
                        projectTask.Start = new DateTime(wiIterationValue.attributes.startDate.Year, wiIterationValue.attributes.startDate.Month, wiIterationValue.attributes.startDate.Day, 8, 0, 0);
                    if (wiIterationValue.attributes.finishDate != null && wiType == "User Story" || wiType == "Product Backlog Item" || wiType == "Requirement")
                        projectTask.Finish = new DateTime(wiIterationValue.attributes.finishDate.Year, wiIterationValue.attributes.finishDate.Month, wiIterationValue.attributes.finishDate.Day, 17, 0, 0); 
                }

                if (wiType == "Task" && workItem.fields.ContainsKey(WorkItemWorkFileds.Remaining))
                {
                    projectTask.Duration = (double)workItem.fields[WorkItemWorkFileds.Remaining] * 60;
                }
            }
        }

        private static void AddCoreFields(MSProject.Task projectTask, RestApiClasses.WorkItem workItem)
        {
            AddCoreFieldToTask(projectTask, workItem, PlanCoreColumns.WIId);
            AddCoreFieldToTask(projectTask, workItem, PlanCoreColumns.WIRev);
            AddCoreFieldToTask(projectTask, workItem, PlanCoreColumns.WIState);
            AddCoreFieldToTask(projectTask, workItem, PlanCoreColumns.WIReason);
            AddCoreFieldToTask(projectTask, workItem, PlanCoreColumns.WIType);
            AddCoreFieldToTask(projectTask, workItem, PlanCoreColumns.WIIteration);
            AddCoreFieldToTask(projectTask, workItem, PlanCoreColumns.WIArea);
        }

        private static string GetUserDisplayName(object userObject)
        {
            var teamMember = JsonConvert.DeserializeObject<RestApiClasses.IdentityRef>(userObject.ToString());

            return teamMember.displayName;
        }

        private static MSProject.Task FindParentInPlan(RestApiClasses.WorkItem workItem)
        {
            if (!workItem.fields.ContainsKey(WorkItemSystemFileds.Parent)) return null;

            return GetPlanTaskByWorkItemId((long)workItem.fields[WorkItemSystemFileds.Parent]);
        }

        private static MSProject.Task GetPlanTaskByWorkItemId(long id)
        {
            for (int i = 1; i <= ActiveProject.Tasks.Count; i++)
                if (id == GetIntFieldValue(ActiveProject.Tasks[i], PlanCoreColumns.WIId.PjValue)) return ActiveProject.Tasks[i];

            return null;
        }

        private static int GetTaskChildInsertPosition(RestApiClasses.WorkItem workItem, MSProject.Task parentTask)
        {
            if (parentTask == null) return 0;

            if (parentTask.ID + parentTask.OutlineChildren.Count < ActiveProject.Tasks.Count)
                return parentTask.ID + parentTask.OutlineChildren.Count + 1;

            return 0;
        }

        private static void AddCoreFieldToTask(MSProject.Task projectTask, RestApiClasses.WorkItem workItem, FieldPlanMapping fieldPlanMapping)
        {
            switch (fieldPlanMapping.AzDORefName)
            {
                case WorkItemSystemFileds.ID: 
                    projectTask.SetField(fieldPlanMapping.PjValue, workItem.id.ToString());
                    break;
                case WorkItemSystemFileds.Rev:
                    projectTask.SetField(fieldPlanMapping.PjValue, workItem.rev.ToString());
                    break;
                case WorkItemSystemFileds.Iteration:
                    AddClassificationFieldToPlan(projectTask, workItem, fieldPlanMapping);
                    break;
                case WorkItemSystemFileds.Area:
                    AddClassificationFieldToPlan(projectTask, workItem, fieldPlanMapping);
                    break;
                default:
                    projectTask.SetField(fieldPlanMapping.PjValue, workItem.fields[fieldPlanMapping.AzDORefName].ToString());
                    break;
            }
        }

        private static string GetStringFieldValue(MSProject.Task projectTask, MSProject.PjField pjField)
        {
            return projectTask.GetField(pjField);
        }

        private static int GetIntFieldValue(MSProject.Task projectTask, MSProject.PjField pjField)
        {
            string fieldStringValue = GetStringFieldValue(projectTask, pjField);

            int fieldIntValue;

            if (int.TryParse(fieldStringValue, out fieldIntValue)) return fieldIntValue;

            return 0;
        }

        private static void AddClassificationFieldToPlan(MSProject.Task projectTask, RestApiClasses.WorkItem workItem, FieldPlanMapping fieldPlanMapping)
        {
            string teamProject = workItem.fields[WorkItemSystemFileds.TeamProject].ToString();
            string classificationField = workItem.fields[fieldPlanMapping.AzDORefName].ToString();
            if (teamProject != classificationField)
            {
                string fieldValue = classificationField.Remove(0, teamProject.Length + 1);
                projectTask.SetField(fieldPlanMapping.PjValue, fieldValue);
            }
        }

        internal static void SavePlanningSettings(bool useSprintStartDate)
        {
            PlanningSettings planningSettings = new PlanningSettings();
            planningSettings.useSprintStartDate = useSprintStartDate;

            SaveDocSetting(PlanDocProperties.PlanningSettings, JsonConvert.SerializeObject(planningSettings));
        }

        internal static void SaveOperationalSettings(bool savePlan, string workItemTag)
        {
            OperationalSettings operationalSettings = new OperationalSettings();
            operationalSettings.savePlan = savePlan;
            operationalSettings.workItemTag = workItemTag;

            SaveDocSetting(PlanDocProperties.OperationalSettings, JsonConvert.SerializeObject(operationalSettings));
        }

        #region project table configs

        internal static void UpdateView()
        {
            AddFieldToView(PlanCoreColumns.WIId.PjValue, PlanCoreColumns.WIId.Name, 10);
            AddFieldToView(PlanCoreColumns.WIType.PjValue, PlanCoreColumns.WIType.Name, 20);
            AddFieldToView(PlanCoreColumns.WIState.PjValue, PlanCoreColumns.WIState.Name, 15);
            AddFieldToView(PlanCoreColumns.WIArea.PjValue, PlanCoreColumns.WIArea.Name);
            AddFieldToView(PlanCoreColumns.WIIteration.PjValue, PlanCoreColumns.WIIteration.Name);

            AddLookUpWorkItemTypes();
        }

        private static void AddLookUpWorkItemTypes()
        {
            AppObj.CustomFieldValueList((MSProject.PjCustomField)PlanCoreColumns.WIType.PjValue, Type.Missing, Type.Missing, true, false, false);

            AppObj.CustomFieldProperties((MSProject.PjCustomField)PlanCoreColumns.WIType.PjValue, MSProject.PjCustomFieldAttribute.pjFieldAttributeValueList, MSProject.PjSummaryCalc.pjCalcNone);

            string mappedWorkItemsString = GetDocProperty(PlanDocProperties.AzDoWorkItemTypes);

            if (mappedWorkItemsString == "") return;

            string[] mappedWorkItems = mappedWorkItemsString.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            List<string> existingValues = new List<string>();

            for (int i = 1; i < 100; i++)
            {
                try { existingValues.Add(AppObj.CustomFieldValueListGetItem((MSProject.PjCustomField)PlanCoreColumns.WIType.PjValue, MSProject.PjValueListItem.pjValueListValue, i)); }
                catch (System.Exception) { break; }
            }

            foreach (var mappedWorkItem in mappedWorkItems)
                if (!existingValues.Contains(mappedWorkItem)) AppObj.CustomFieldValueListAdd((MSProject.PjCustomField)PlanCoreColumns.WIType.PjValue, mappedWorkItem);            
        }

        private static void AddFieldToView(MSProject.PjField fieldPjValue, string fieldViewName, int ColumnWidth = 30)
        {
            if (AppObj.CustomFieldGetName((MSProject.PjCustomField)fieldPjValue) != fieldViewName)
            {
                AppObj.CustomFieldRename((MSProject.PjCustomField)fieldPjValue, fieldViewName, Type.Missing);
                AppObj.TableEdit(ActiveProject.CurrentTable, true, Type.Missing, Type.Missing,
                                  Type.Missing, Type.Missing, fieldViewName, Type.Missing, ColumnWidth, 0, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                AppObj.TableApply(ActiveProject.CurrentTable);
            }
            else
            {
                foreach (MSProject.TableField _tableField in ActiveProject.TaskTables[ActiveProject.CurrentTable].TableFields)
                {
                    if (_tableField.Field == fieldPjValue)
                        return;
                }

                AppObj.TableEdit(ActiveProject.CurrentTable, true, Type.Missing, Type.Missing,
                                  Type.Missing, Type.Missing, fieldViewName, Type.Missing, ColumnWidth, 0, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                AppObj.TableApply(ActiveProject.CurrentTable);
            }
        }       

        #endregion
    }
}
