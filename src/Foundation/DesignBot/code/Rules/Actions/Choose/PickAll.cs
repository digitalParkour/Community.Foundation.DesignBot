using Sitecore.Diagnostics;
using Sitecore.Layouts;
using Sitecore.Rules.Actions;
using System.Linq;

namespace Community.Foundation.DesignBot.Rules.Actions.Choose
{
    /// <summary>
    /// Add or update one specific parameter, maintain the rest of the parameters as is
    /// </summary>
    public class PickAll<T> : RuleAction<T>
    where T : DesignBotRuleContext
    {
        public override void Apply(T ruleContext)
        {
            Assert.ArgumentNotNull(ruleContext, "ruleContext");

            BotLog.Log.Info($"PICK - picked all renderings");
            Log.Info($"DESIGNBOT:: PICK - picked all renderings", this);

            ruleContext.SelectedRenderings = ruleContext.SelectedDevice.Renderings.Cast<RenderingDefinition>().ToList();
        }
    }
}