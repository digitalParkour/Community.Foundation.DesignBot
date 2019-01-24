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
    public class SetDatasourceField<T> : RuleAction<T>
    where T : DesignBotRuleContext
    {
        public ID FieldId { get; set; }
        public string Value { get; set; }

        public override void Apply(T ruleContext)
        {
            Assert.ArgumentNotNull(ruleContext, "ruleContext");
            Assert.ArgumentNotNullOrEmpty(FieldId, "FieldId");
            Assert.ArgumentNotNullOrEmpty(Value, "Value");
            
            foreach (var rendering in ruleContext.SelectedRenderings)
            {
                var dsItem = rendering.GetDatasourceItem(ruleContext.Item);
                if (dsItem == null)
                    continue;
                
                if(dsItem[FieldId] != Value)
                {
                    try
                    {
                        dsItem = dsItem.AsNewVersion(matchWorkflowState: true);
                        dsItem.Editing.BeginEdit();
                        dsItem[FieldId] = Value;
                        dsItem.Editing.EndEdit();
                        BotLog.Log.Info($"UPDATED datasource item {dsItem.DisplayName}, {dsItem.ID}, field id {FieldId} to value '{Value}'");
                        Log.Info($"DESIGNBOT:: UPDATED datasource item {dsItem.DisplayName}, {dsItem.ID}, field id {FieldId} to value '{Value}'", this);
                    }
                    catch(Exception ex)
                    {
                        BotLog.Log.Error($"UPDATE FAILED on datasource item {dsItem.DisplayName}, {dsItem.ID}, field id {FieldId} attempting value '{Value}'", ex);
                        Log.Error($"DESIGNBOT:: UPDATE FAILED on datasource item {dsItem.DisplayName}, {dsItem.ID}, field id {FieldId} attempting value '{Value}'", ex, this);
                        throw;
                    }
                } else
                    BotLog.Log.Info($"Skipped datasource item {dsItem.DisplayName}, {dsItem.ID}, field id {FieldId} already matches value '{Value}'");
                    Log.Info($"DESIGNBOT:: Skipped datasource item {dsItem.DisplayName}, {dsItem.ID}, field id {FieldId} already matches value '{Value}'", this);
                
                
            }
        }
    }
}