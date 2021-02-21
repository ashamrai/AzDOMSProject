using Microsoft.Office.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSProject = Microsoft.Office.Interop.MSProject;
using Office = Microsoft.Office.Core;
using AzDOAddIn.AzDOAddInSettings;

namespace AzDOAddIn
{
    static class ProjectOperations
    {
        static public MSProject.Application AppObj { get; set; }

        static public bool TeamProjectLinked
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

        static MSProject.Project ActiveProject { get { return AppObj.ActiveProject; } }
        static DocumentProperties DocPropeties { get { return ActiveProject.CustomDocumentProperties; } }
        static public string ActiveTeamProject { get { return GetDocProperty(PlanDocProperties.AzDoTeamProject); } }
        static public string ActiveOrgUrl { get { return GetDocProperty(PlanDocProperties.AzDoUrl); } }


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

        static public void LinkToTeamProject()
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

        internal static void UpdateView()
        {
            AddFieldToView(PlanCoreColumns.WIId.PjValue, PlanCoreColumns.WIId.Name);
            AddFieldToView(PlanCoreColumns.WIType.PjValue, PlanCoreColumns.WIType.Name);
            AddFieldToView(PlanCoreColumns.WIState.PjValue, PlanCoreColumns.WIState.Name);
            AddFieldToView(PlanCoreColumns.WIArea.PjValue, PlanCoreColumns.WIArea.Name);
            AddFieldToView(PlanCoreColumns.WIIteration.PjValue, PlanCoreColumns.WIIteration.Name);
        }

        private static void AddFieldToView(MSProject.PjField fieldPjValue, string fieldViewName)
        {
            if (AppObj.CustomFieldGetName((MSProject.PjCustomField)fieldPjValue) != fieldViewName)
            {
                AppObj.CustomFieldRename((MSProject.PjCustomField)fieldPjValue, fieldViewName, Type.Missing);
                AppObj.TableEdit(ActiveProject.CurrentTable, true, Type.Missing, Type.Missing,
                                  Type.Missing, Type.Missing, fieldViewName, Type.Missing, 30, 2, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
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
                                  Type.Missing, Type.Missing, fieldViewName, Type.Missing, 30, 2, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                AppObj.TableApply(ActiveProject.CurrentTable);
            }
        }

        public static void AddWorkItemsToPlan(List<int> wiIds)
        {
            foreach(int wiId in wiIds)
            {
                var workItem = AzDORestApiHelper.GetWorkItem(ActiveOrgUrl, ActiveTeamProject, wiId, "");

                MSProject.Task projectTask = AddWorkItem(workItem);
            }
        }

        private static MSProject.Task AddWorkItem(RestApiClasses.WorkItem workItem)
        {
            MSProject.Task projectTask = ActiveProject.Tasks.Add(workItem.fields[PlanCoreColumns.WITitle.AzDORefName]);
            AddFiledToTask(projectTask, workItem, PlanCoreColumns.WIId);
            AddFiledToTask(projectTask, workItem, PlanCoreColumns.WIRev);
            AddFiledToTask(projectTask, workItem, PlanCoreColumns.WIState);
            AddFiledToTask(projectTask, workItem, PlanCoreColumns.WIRev);
            AddFiledToTask(projectTask, workItem, PlanCoreColumns.WIType);
            AddFiledToTask(projectTask, workItem, PlanCoreColumns.WIIteration);
            AddFiledToTask(projectTask, workItem, PlanCoreColumns.WIArea);

            return projectTask;
        }

        private static void AddFiledToTask(MSProject.Task projectTask, RestApiClasses.WorkItem workItem, FieldPlanMapping fieldPlanMapping)
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
    }
}
