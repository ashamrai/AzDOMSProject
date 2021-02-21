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


        static int CheckDocProperty(string propertyName)
        {
            for (int i = 1; i <= DocPropeties.Count; i++)
                if (DocPropeties[i].Name == propertyName) 
                    return i;

            return 0;
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
    }
}
