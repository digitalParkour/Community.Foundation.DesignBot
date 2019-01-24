using Sitecore.Diagnostics;
using Sitecore.Rules.Actions;
using Sitecore.Text;
using System.Web;

namespace Community.Foundation.DesignBot.Rules.Actions.Parameters
{
    /// <summary>
    /// Add or update one specific parameter, maintain the rest of the parameters as is
    /// </summary>
    public class SetParameterAction<T> : RuleAction<T>
    where T : DesignBotRuleContext
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public override void Apply(T ruleContext)
        {
            Assert.ArgumentNotNull(ruleContext, "ruleContext");
            Assert.ArgumentNotNullOrEmpty(Name, "Name");

            foreach(var rendering in ruleContext.SelectedRenderings)
            {             
                // Simple case, no existing parameters
                if (string.IsNullOrWhiteSpace(rendering.Parameters))
                {
                    rendering.Parameters = $"{Name}={Value ?? string.Empty}}}";
                    ruleContext.HasLayoutChange = true;
                    BotLog.Log.Info($"UPDATED rendering {rendering.ItemID} ({rendering.Placeholder}{rendering.UniqueId}) parameter {Name} to value '{Value}'");
                    Log.Info($"DESIGNBOT:: UPDATED rendering {rendering.ItemID} ({rendering.Placeholder}{rendering.UniqueId}) parameter {Name} to value '{Value}'", this);
                    continue;
                }

                // Parse case, set new value keeping other name-value pairs

                // Value matches case, exit
                var urlString = new UrlString(rendering.Parameters);
                if (!string.IsNullOrWhiteSpace(urlString[Name]) 
                    && HttpUtility.UrlDecode(urlString[Name]).Equals(HttpUtility.UrlDecode(Value)))
                    continue;

                urlString.Append(Name,Value ?? string.Empty);
                rendering.Parameters = urlString.GetUrl();
                ruleContext.HasLayoutChange = true;
                BotLog.Log.Info($"UPDATED rendering {rendering.ItemID} ({rendering.Placeholder}{rendering.UniqueId}) parameter {Name} to value '{Value}'");
                Log.Info($"DESIGNBOT:: UPDATED rendering {rendering.ItemID} ({rendering.Placeholder}{rendering.UniqueId}) parameter {Name} to value '{Value}'", this);
            }
        }
    }
}