using Sitecore;
using Sitecore.Data.Items;

namespace Community.Foundation.DesignBot.Extensions
{
    public static class ItemExtensions
    {
        /// <summary>
        /// Add version, select and return that new version, match workflow state
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static Item AsNewVersion(this Item item, bool matchWorkflowState = false) {
            // save current workflow state
            var workflowState = item[FieldIDs.WorkflowState];
            var workflow = item[FieldIDs.Workflow];

            item.Versions.AddVersion();
            // Now get new Version
            item = item.Database.GetItem(item.ID);

            // Match workflow
            if (matchWorkflowState && item[FieldIDs.WorkflowState] != workflowState)
            {
                item.Editing.BeginEdit();
                {
                    item[FieldIDs.Workflow] = workflow;
                    item[FieldIDs.WorkflowState] = workflowState;
                }
                item.Editing.EndEdit();
            }

            return item;
        }
    }
}