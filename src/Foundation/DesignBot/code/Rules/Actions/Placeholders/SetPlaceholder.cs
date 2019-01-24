using Sitecore.Diagnostics;
using Sitecore.Rules.Actions;

namespace Community.Foundation.DesignBot.Rules.Actions.Placeholders
{
    /// <summary>
    /// Set placeholder path of selected renderings
    /// </summary>
    public class SetPlaceholders<T> : RuleAction<T>
    where T : DesignBotRuleContext
    {
        public string Path { get; set; }

        public override void Apply(T ruleContext)
        {
            Assert.ArgumentNotNull(ruleContext, "ruleContext");

            BotLog.Log.Info($"PLACEHOLDER - set all selected to {Path}");
            Log.Info($"DESIGNBOT:: PLACEHOLDER - set all selected to {Path}", this);

            // Use selected renderings only
            var renderings = ruleContext.SelectedRenderings;
            
            foreach (var r in renderings)
            {
                if (!r.Placeholder.Equals(Path))
                {
                    r.Placeholder = Path;
                    ruleContext.HasLayoutChange = true;
                }
            }
        }
    }
}