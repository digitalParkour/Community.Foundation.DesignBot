using Sitecore.Diagnostics;
using Sitecore.Rules.Actions;
using Sitecore.Text;
using System;
using System.Linq;
using System.Web;

namespace Community.Foundation.DesignBot.Rules.Actions.Parameters
{
    /// <summary>
    /// "set multilist [parameter], remove [term] (if exists)"
    /// Remove (if exists) one term of a delimited parameter value
    /// Example: remove one ID from a multilist
    /// </summary>
    public class RemoveParameterTermAction<T> : RuleAction<T>
    where T : DesignBotRuleContext
    {
        public string Name { get; set; }        // Name of rendering param
        public string Term { get; set; }        // Term of rendering param value to remove
        public string Delimiter { get; set; }   // Example "|" ... ugh this breaks the UI

        public override void Apply(T ruleContext)
        {
            Assert.ArgumentNotNull(ruleContext, "ruleContext");
            Assert.ArgumentNotNullOrEmpty(Name, "Name");
            Assert.ArgumentNotNullOrEmpty(Term, "Term");
            // Assert.ArgumentNotNullOrEmpty(Delimiter, "Delimiter");
            Delimiter = "|";

            foreach (var rendering in ruleContext.SelectedRenderings)
            {
                // Simple case, no value, nothing to clear
                if (string.IsNullOrWhiteSpace(rendering.Parameters))
                {
                    continue;
                }

                // Missing parameter case, nothing to clear
                var urlString = new UrlString(rendering.Parameters);
                var rawValue = urlString.Parameters[Name];
                if (string.IsNullOrWhiteSpace(rawValue))
                    continue;

                // Missing value case, nothing to clear
                var valueList = rawValue.Split(new string[] { Delimiter }, StringSplitOptions.RemoveEmptyEntries);
                if (!valueList.Any(x => x != null && HttpUtility.UrlDecode(x).Equals(HttpUtility.UrlDecode(Term))))
                    continue;

                // Remove term
                var newList = string.Join(Delimiter,
                    valueList.Where(x => x != null && !HttpUtility.UrlDecode(x).Equals(HttpUtility.UrlDecode(Term), StringComparison.OrdinalIgnoreCase))
                    );
                urlString.Append(Name, newList ?? string.Empty);
                rendering.Parameters = urlString.GetUrl();
                ruleContext.HasLayoutChange = true;
                BotLog.Log.Info($"UPDATED rendering {rendering.ItemID} ({rendering.Placeholder}{rendering.UniqueId}) parameter {Name} removed term '{Term}'");
                Log.Info($"DESIGNBOT:: UPDATED rendering {rendering.ItemID} ({rendering.Placeholder}{rendering.UniqueId}) parameter {Name} removed term '{Term}'", this);
            }
        }
    }
}