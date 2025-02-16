using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.MSProject;

namespace AzDOAddIn.AzDOAddInSettings
{
    static class WorkItemSystemFileds
    {
        public const string ID = "System.Id";
        public const string Title = "System.Title";
        public const string AssignedTo = "System.AssignedTo";
        public const string Type = "System.WorkItemType";
        public const string State = "System.State";
        public const string Reason = "System.Reason";
        public const string Iteration = "System.IterationPath";
        public const string Area = "System.AreaPath";
        public const string Rev = "System.Rev";
        public const string TeamProject = "System.TeamProject";
        public const string Parent = "System.Parent";
    }

    static class WorkItemWorkFileds
    {
        public const string OriginalEstimate = "Microsoft.VSTS.Scheduling.OriginalEstimate";
        public const string Remaining = "Microsoft.VSTS.Scheduling.RemainingWork";
        public const string Completed = "Microsoft.VSTS.Scheduling.CompletedWork";
    }

    static class WorkItemSchedulingFileds
    {
        public const string Start = "Microsoft.VSTS.Scheduling.StartDate";
        public const string Finish = "Microsoft.VSTS.Scheduling.FinishDate";
    }

    static class WorkItemSystemLinks
    {
        public const string Parent = "System.LinkTypes.Hierarchy-Reverse";
        public const string Child = "System.LinkTypes.Hierarchy-Forward";
    }

    public static class PlanDocProperties
    {
        public const string AzDoUrl = "Azure DevOps Service URL";
        public const string AzDoTeamProject = "Azure DevOps Team Project";
        public const string AzDoWorkItemTypes = "Azure DevOps Work Item Types";
        public const string PlanningSettings = "Azure DevOps Planning Settings";
    }

    public static class PlanCoreColumns
    {
        public static FieldPlanMapping WITitle = new FieldPlanMapping { Name = "Title", AzDORefName = WorkItemSystemFileds.Title };
        public static FieldPlanMapping WIAssignedTo = new FieldPlanMapping { Name = "AssignedTo", AzDORefName = WorkItemSystemFileds.AssignedTo };
        public static FieldPlanMapping WIId = new FieldPlanMapping { Name = "Work Item Id", PjValue = PjField.pjTaskText23, AzDORefName = WorkItemSystemFileds.ID };
        public static FieldPlanMapping WIType = new FieldPlanMapping { Name = "Work Item Type", PjValue = PjField.pjTaskText24, AzDORefName = WorkItemSystemFileds.Type };
        public static FieldPlanMapping WIState = new FieldPlanMapping { Name = "Work Item State", PjValue = PjField.pjTaskText25, AzDORefName = WorkItemSystemFileds.State };
        public static FieldPlanMapping WIReason = new FieldPlanMapping { Name = "Work Item Reason", PjValue = PjField.pjTaskText26, AzDORefName = WorkItemSystemFileds.Reason };
        public static FieldPlanMapping WIIteration = new FieldPlanMapping { Name = "Iteration Path", PjValue = PjField.pjTaskText27, AzDORefName = WorkItemSystemFileds.Iteration };
        public static FieldPlanMapping WIArea = new FieldPlanMapping { Name = "Area Path", PjValue = PjField.pjTaskText28, AzDORefName = WorkItemSystemFileds.Area };
        public static FieldPlanMapping WIRev = new FieldPlanMapping { Name = "Work Item Rev", PjValue = PjField.pjTaskText29, AzDORefName = WorkItemSystemFileds.Rev };
    }

    public class FieldPlanMapping
    {
        public string Name;
        public PjField PjValue;
        public string AzDORefName;
    }

}
