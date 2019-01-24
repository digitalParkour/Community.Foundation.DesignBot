using Sitecore.Diagnostics;
using Sitecore.Layouts;
using Sitecore.Rules.Actions;
using System.Linq;

namespace Community.Foundation.DesignBot.Rules.Actions.Choose
{
    /// <summary>
    /// Add or update one specific parameter, maintain the rest of the parameters as is
    /// </summary>
    public class InvertSelection<T> : RuleAction<T>
    where T : DesignBotRuleContext
    {
        public override void Apply(T ruleContext)
        {
            Assert.ArgumentNotNull(ruleContext, "ruleContext");

            BotLog.Log.Info($"PICK - inverted picks");
            Log.Info($"DESIGNBOT:: PICK - inverted picks", this);

            ruleContext.SelectedRenderings = ruleContext.SelectedDevice.Renderings.Cast<RenderingDefinition>()
                .Where(x => !ruleContext.SelectedRenderings.Any(i => i.UniqueId == x.UniqueId)).ToList();
        }
    }
}