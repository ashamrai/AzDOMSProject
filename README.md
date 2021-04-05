# AzDOMSProject

The first release: https://github.com/ashamrai/AzDOMSProject/releases/tag/v0.1-alpha

## Install addIn
1.	Download and extract AzDOAddInSetup.zip
1.	Run setup.exe
1.	Press Next on each step.

To start work with the solution, start MS Project and switch to Azure DevOps Work Items tab.

![AddIn Tab](/images/addin_tab.png)

## Generate Personal Access Token
The solution supports access only through Personal Access Token (PAT). Use the following documentation to generate PAT: [Use personal access tokens](https://docs.microsoft.com/en-us/azure/devops/organizations/accounts/use-personal-access-tokens-to-authenticate?view=azure-devops&tabs=preview-page)

Add the following permissions to your PAT:

![PAT Permissions](/images/pat_permissions.png)

## Associate Project Plan with Team Project
To synchronize work items with plan tasks, you should associate your MS Project plan and Azure DevOps team project.

1. Press **Link to Team Project** button

![Link To Team Project](/images/link_team_project.png)

2. Add Azure DevOps organization url and PAT on the new form. Press **Get Team Project** button:

![Org URL](/images/add_org_url.png)

3.	Select a team project to sync and press **Ok**:

![Team Projects](/images/add_team_project.png)

4.	Then select work item types to sync and press **Ok** on the following form:

![Work Item Types](/images/work_items_list.png)

5. Save changes
