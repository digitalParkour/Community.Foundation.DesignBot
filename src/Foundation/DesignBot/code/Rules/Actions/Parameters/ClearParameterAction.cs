using Sitecore.Diagnostics;
using Sitecore.Rules.Actions;
using Sitecore.Text;

namespace Community.Foundation.DesignBot.Rules.Actions.Parameters
{
    /// <summary>
    /// Add or update one specific parameter, maintain the rest of the parameters as is
    /// </summary>
    public class ClearParameterAction<T> : RuleAction<T>
    where T : DesignBotRuleContext
    {
        public string Name { get; set; }
               
        public override void Apply(T ruleContext)
        {
            Assert.ArgumentNotNull(ruleContext, "ruleContext");
            Assert.ArgumentNotNullOrEmpty(Name, "Name");

            foreach (var rendering in ruleContext.SelectedRenderings)
            {
                // Simple case, no parameters, nothing to remove
                if (string.IsNullOrWhiteSpace(rendering.Parameters))
                {
                    continue;
                }

                // Parse case
                var urlString = new UrlString(rendering.Parameters);
                if (string.IsNullOrEmpty(urlString.Parameters[Name]))
                {
                    continue; // does not exist, nothing to remove
                }

                urlString.Remove(Name);
                rendering.Parameters = urlString.GetUrl();
                ruleContext.HasLayoutChange = true;
                BotLog.Log.Info($"UPDATED rendering {rendering.ItemID} ({rendering.Placeholder}{rendering.UniqueId}) cleared parameter {Name}");
                Log.Info($"DESIGNBOT:: UPDATED rendering {rendering.ItemID} ({rendering.Placeholder}{rendering.UniqueId}) cleared parameter {Name}", this);
            }
        }
    }
}