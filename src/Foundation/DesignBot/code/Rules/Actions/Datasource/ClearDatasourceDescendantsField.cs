using Community.Foundation.DesignBot.Extensions;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Rules.Actions;
using System;
using System.Linq;

namespace Community.Foundation.DesignBot.Rules.Actions.Datasource
{
    /// <summary>
    /// Add or update one specific parameter, maintain the rest of the parameters as is
    /// </summary>
    public class ClearDatasourceDescendantsField<T> : RuleAction<T>
    where T : DesignBotRuleContext
    {
        public ID TemplateId { get; set; }
        public ID FieldId { get; set; }

        public override void Apply(T ruleContext)
        {
            Assert.ArgumentNotNull(ruleContext, "ruleContext");
            Assert.ArgumentNotNullOrEmpty(TemplateId, "TemplateId");
            Assert.ArgumentNotNullOrEmpty(FieldId, "FieldId");
            
            foreach (var rendering in ruleContext.SelectedRenderings)
            {
                var dsItem = rendering.GetDatasourceItem(ruleContext.Item);
                if (dsItem == null)
                    continue;

                var subItems = dsItem.Axes.GetDescendants().Where(x => x.TemplateID.Equals(TemplateId)).ToList();
                foreach(var subItem in subItems)
                {
                    if (!string.IsNullOrEmpty(subItem[FieldId]))
                    {
                        try
                        {
                            var sItem2 = subItem.AsNewVersion(matchWorkflowState:true);
                            sItem2.Editing.BeginEdit();
                            sItem2[FieldId] = null;
                            sItem2.Editing.EndEdit();
                            BotLog.Log.Info($"UPDATED datasource descendant item {subItem.DisplayName}, {subItem.ID}, field id {FieldId}");
                            Log.Info($"DESIGNBOT:: UPDATED datasource descendant item {subItem.DisplayName}, {subItem.ID}, field id {FieldId}", this);
                        }
                        catch (Exception ex)
                        {
                            BotLog.Log.Error($"UPDATE FAILED on datasource descendant item {subItem.DisplayName}, {subItem.ID}, attempting to clear field id {FieldId}", ex);
                            Log.Error($"DESIGNBOT:: UPDATE FAILED on datasource descendant item {subItem.DisplayName}, {subItem.ID}, attempting to clear field id {FieldId}", ex, this);
                            throw;
                        }
                    }
                    else
                    {
                        BotLog.Log.Info($"Skipped datasource descendant item {subItem.DisplayName}, {subItem.ID}, field id {FieldId} already empty");
                        Log.Info($"DESIGNBOT:: Skipped datasource descendant item {subItem.DisplayName}, {subItem.ID}, field id {FieldId} already empty", this);
                    }
                }
                
            }
        }
    }
}