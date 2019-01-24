using Sitecore.Diagnostics;
using Sitecore.Rules.Actions;

namespace Community.Foundation.DesignBot.Rules.Actions.Choose
{
    /// <summary>
    /// Add or update one specific parameter, maintain the rest of the parameters as is
    /// </summary>
    public class ClearPicks<T> : RuleAction<T>
    where T : DesignBotRuleContext
    {

        public override void Apply(T ruleContext)
        {
            Assert.ArgumentNotNull(ruleContext, "ruleContext");

            BotLog.Log.Info($"PICK - cleared picks");
            Log.Info($"DESIGNBOT:: PICK - cleared picks", this);
            ruleContext.SelectedRenderings.Clear();
        }
    }
}