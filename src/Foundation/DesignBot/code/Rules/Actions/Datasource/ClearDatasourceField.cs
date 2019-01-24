using Community.Foundation.DesignBot.Extensions;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Rules.Actions;
using System;

namespace Community.Foundation.DesignBot.Rules.Actions.Datasource
{
    /// <summary>
    /// Add or update one specific parameter, maintain the rest of the parameters as is
    /// </summary>
    public class ClearDatasourceField<T> : RuleAction<T>
    where T : DesignBotRuleContext
    {
        public ID FieldId { get; set; }

        public override void Apply(T ruleContext)
        {
            Assert.ArgumentNotNull(ruleContext, "ruleContext");
            Assert.ArgumentNotNullOrEmpty(FieldId, "FieldId");
            
            foreach (var rendering in ruleContext.SelectedRenderings)
            {
                var dsItem = rendering.GetDatasourceItem(ruleContext.Item);
                if (dsItem == null)
                    continue;

                if (!string.IsNullOrEmpty(dsItem[FieldId]))
                {
                    try
                    {
                        dsItem = dsItem.AsNewVersion(matchWorkflowState: true);
                        dsItem.Editing.BeginEdit();
                        dsItem[FieldId] = null;
                        dsItem.Editing.EndEdit();
                        BotLog.Log.Info($"UPDATED datasource item {dsItem.DisplayName}, {dsItem.ID}, cleared field id {FieldId}");
                        Log.Info($"DESIGNBOT:: UPDATED datasource item {dsItem.DisplayName}, {dsItem.ID}, cleared field id {FieldId}",this);
                    }
                    catch (Exception ex)
                    {
                        BotLog.Log.Error($"UPDATE FAILED on datasource item {dsItem.DisplayName}, {dsItem.ID}, attempting to clear field id {FieldId}", ex);
                        Log.Error($"DESIGNBOT:: UPDATE FAILED on datasource item {dsItem.DisplayName}, {dsItem.ID}, attempting to clear field id {FieldId}", ex, this);
                        throw;
                    }
                }
                else
                {
                    BotLog.Log.Info($"Skipped datasource item {dsItem.DisplayName}, {dsItem.ID}, field id {FieldId} already empty");
                    Log.Info($"DESIGNBOT:: Skipped datasource item {dsItem.DisplayName}, {dsItem.ID}, field id {FieldId} already empty", this);
                }
                
                
            }
        }
    }
}