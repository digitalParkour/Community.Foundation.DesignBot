using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Rules.Actions;

namespace Community.Foundation.DesignBot.Rules.Actions.Choose
{
    /// <summary>
    /// Set Layout of selected device
    /// </summary>
    public class SetLayout<T> : RuleAction<T>
    where T : DesignBotRuleContext
    {
        public ID LayoutId { get; set; }

        public override void Apply(T ruleContext)
        {
            Assert.ArgumentNotNull(ruleContext, "ruleContext");

            BotLog.Log.Info($"LAYOUT - set to {LayoutId}");
            Log.Info($"DESIGNBOT:: LAYOUT - set to {LayoutId}", this);

            ruleContext.SelectedDevice.Layout = LayoutId.ToString();
        }
    }
}