using Microsoft.Office.Core;
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

namespace AzDOAddIn
{
    static class ProjectOperations
    {
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
            for (int i = 1; i <= ActiveProject.Tasks.Count; i++)
            {
                int wiPrjId = GetProjectTaskWorkItedId(ActiveProject.Tasks[i]);

                if (wiPrjId == 0) continue;

                var workItem = AzDORestApiHelper.GetWorkItem(ActiveOrgUrl, ActiveTeamProject, wiPrjId, ActivePAT);
                AddCoreFields(ActiveProject.Tasks[i], workItem);
            }
        }

        internal static void PublishProjectPlan()
        {
            try
            {
                for (int i = 1; i <= ActiveProject.Tasks.Count; i++)
                {
                    int wiPrjId = GetProjectTaskWorkItedId(ActiveProject.Tasks[i]);

                    if (wiPrjId > 0) { /*PublishChanges();*/ }
                    else { PublishNewWorkItem(ActiveProject.Tasks[i]); }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        internal static void ImportTeamMembers()
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
        }

        private static void PublishNewWorkItem(MSProject.Task task)
        {
            string wiType = GetProjectTaskWorkItedType(task);
            if (string.IsNullOrEmpty(wiType)) return;

            string wiName = task.Name;
            if (string.IsNullOrEmpty(wiName)) throw new Exception(string.Format("Task [{0}] without name", task.ID));

            int parentId = 0;

            if (WorkItemTreeInConsistency(task))
            {
                Dictionary<string, string> fields = new Dictionary<string, string>();
                fields.Add(PlanCoreColumns.WITitle.AzDORefName, wiName);
                if (task.Resources.Count == 1) fields.Add(PlanCoreColumns.WIAssignedTo.AzDORefName, task.Resources[1].Name);

                if (task.OutlineLevel > 1) parentId = GetProjectTaskWorkItedId(task.OutlineParent);

                var workItem = AzDORestApiHelper.PublishNewWorkItem(ActiveOrgUrl, ActiveTeamProject, ActivePAT, wiType, fields, parentId);

                AddCoreFields(task, workItem);               
            }
        }

        private static bool WorkItemTreeInConsistency(MSProject.Task task)
        {
            if (task.OutlineLevel > 2)
            {
                if (GetProjectTaskWorkItedId(task.OutlineParent) != 0) return true;

                MSProject.Task ancestor = task.OutlineParent.OutlineParent;

                do
                {
                    if (GetProjectTaskWorkItedId(ancestor) > 0) 
                        throw new Exception(string.Format("Task [{0}]:\"{1}\"\n should have a parent work item in the plan", 
                            task.ID, (task.Name.Length > 20) ? task.Name.Substring(0, 17) + "..." : task.Name));

                    ancestor = ancestor.OutlineParent;

                } while (ancestor.OutlineLevel > 1);
            }

            return true;
        }

        private static int GetProjectTaskWorkItedId(MSProject.Task task)
        {
            return GetIntFieldValue(task, PlanCoreColumns.WIId.PjValue);
        }

        private static string GetProjectTaskWorkItedState(MSProject.Task task)
        {
            return GetStringFieldValue(task, PlanCoreColumns.WIState.PjValue);
        }

        private static string GetProjectTaskWorkItedType(MSProject.Task task)
        {
            return GetStringFieldValue(task, PlanCoreColumns.WIType.PjValue);
        }

        static MSProject.Project ActiveProject { get { return AppObj.ActiveProject; } }
        static MSProject.Task SelectedTask { get { return (AppObj.ActiveSelection.Tasks == null) ? null : AppObj.ActiveSelection.Tasks[1]; } }
        static DocumentProperties DocPropeties { get { return ActiveProject.CustomDocumentProperties; } }        
        static internal string ActiveTeamProject { get { return GetDocProperty(PlanDocProperties.AzDoTeamProject); } }
        static internal string ActiveOrgUrl { get { return GetDocProperty(PlanDocProperties.AzDoUrl); } }
        static internal string ActivePAT { get { return PatHelper.GetPat(ActiveOrgUrl); } }


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
            Forms.WndLinkToTeamProject linkForm = new Forms.WndLinkToTeamProject();

            if (linkForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SaveDocSetting(PlanDocProperties.AzDoUrl, linkForm.Url);
                SaveDocSetting(PlanDocProperties.AzDoTeamProject, linkForm.TeamProject);

                Forms.WorkItemTypes workItemTypesForm = new Forms.WorkItemTypes();

                var workItemTypes = AzDORestApiHelper.GetWorkItemTypes(linkForm.Url, linkForm.TeamProject, linkForm.PAT);

                foreach(var item in workItemTypes.WorkItemTypes)
                    workItemTypesForm.AddWorkItemTypeToList(item.name);

                if (workItemTypesForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    SaveDocSetting(PlanDocProperties.AzDoWorkItemTypes, workItemTypesForm.SelectedItems());

                PatHelper.SetPat(linkForm.Url, linkForm.PAT);
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
            foreach(int wiId in wiIds)
            {
                var workItem = AzDORestApiHelper.GetWorkItem(ActiveOrgUrl, ActiveTeamProject, wiId, ActivePAT);

                MSProject.Task projectTask = AddWorkItem(workItem);
            }
        }

        internal static void ImportChilds()
        {
            var currentTask = SelectedTask;

            if (currentTask == null) return;

            int currentWiId = GetIntFieldValue(currentTask, PlanCoreColumns.WIId.PjValue);

            if (currentWiId == 0) return;

            var workItems = AzDORestApiHelper.GetChildWorkItems(ActiveOrgUrl, ActiveTeamProject, currentWiId, ActivePAT);

            if (workItems.Count == 0) return;

            foreach(var workItem in workItems)
            {
                AddWorkItem(workItem, currentTask);
            }
        }

        private static MSProject.Task AddWorkItem(RestApiClasses.WorkItem workItem, MSProject.Task parentTask = null)
        {
            if (GetPlanTaskByWorkItemId(workItem.id) != null) return null;

            if (parentTask == null) parentTask = FindParentInPlan(workItem);

            int taskInsertPosition = GetTaskChildInsertPosition(workItem, parentTask);

            MSProject.Task projectTask = ActiveProject.Tasks.Add(workItem.fields[PlanCoreColumns.WITitle.AzDORefName],
                (taskInsertPosition == 0) ? Type.Missing : taskInsertPosition);

            AddCoreFields(projectTask, workItem);

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
                catch (Exception) { break; }
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
