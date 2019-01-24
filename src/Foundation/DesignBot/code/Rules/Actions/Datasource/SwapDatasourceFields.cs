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
    public class SwapDatasourceFields<T> : RuleAction<T>
    where T : DesignBotRuleContext
    {
        public ID FieldIdA { get; set; }
        public ID FieldIdB { get; set; }

        public override void Apply(T ruleContext)
        {
            Assert.ArgumentNotNull(ruleContext, "ruleContext");
            Assert.ArgumentNotNullOrEmpty(FieldIdA, "FieldIdA");
            Assert.ArgumentNotNullOrEmpty(FieldIdB, "FieldIdB");
            
            foreach (var rendering in ruleContext.SelectedRenderings)
            {
                var dsItem = rendering.GetDatasourceItem(ruleContext.Item);
                if (dsItem == null)
                    continue;
                
                try
                {
                    dsItem = dsItem.AsNewVersion(matchWorkflowState: true);
                    dsItem.Editing.BeginEdit();
                    {
                        var swap = dsItem[FieldIdA];
                        dsItem[FieldIdA] = dsItem[FieldIdB];
                        dsItem[FieldIdB] = swap;
                    }
                    dsItem.Editing.EndEdit();
                    BotLog.Log.Info($"UPDATED datasource item {dsItem.DisplayName}, {dsItem.ID}, swapped field id {FieldIdA} value with field id {FieldIdB}");
                    Log.Info($"DESIGNBOT:: UPDATED datasource item {dsItem.DisplayName}, {dsItem.ID}, swapped field id {FieldIdA} value with field id {FieldIdB}", this);
                }
                catch(Exception ex)
                {
                    dsItem.Editing.CancelEdit();
                    BotLog.Log.Error($"UPDATE FAILED on datasource item {dsItem.DisplayName}, {dsItem.ID}, field id {FieldIdA} attempting swap with field id {FieldIdB}", ex);
                    Log.Error($"DESIGNBOT:: UPDATE FAILED on datasource item {dsItem.DisplayName}, {dsItem.ID}, field id {FieldIdA} attempting swap with field id {FieldIdB}", ex, this);
                    throw;
                }
                
            }
        }
    }
}