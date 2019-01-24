using Sitecore.Diagnostics;
using Sitecore.Rules.Actions;
using Sitecore.Text;
using System.Web;

namespace Community.Foundation.DesignBot.Rules.Actions.Parameters
{
    /// <summary>
    /// "set multilist [parameter], append [term]"
    /// Add (if not already exists) one term of a delimited parameter value
    /// Example: add one ID to a multilist
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AppendParameterTermAction<T> : RuleAction<T>
    where T : DesignBotRuleContext
    {
        public string Name { get; set; }        // Name of rendering param
        public string Term { get; set; }        // Term of rendering param value to remove
        public string Delimiter { get; set; }   // Example "|" ... originally tried adding this as input to rule, but, oddly a single pipe as a value breaks the UI

        public override void Apply(T ruleContext)
        {
            Assert.ArgumentNotNull(ruleContext, "ruleContext");
            Assert.ArgumentNotNullOrEmpty(Name, "Name");
            Assert.ArgumentNotNullOrEmpty(Term, "Term");
            // Assert.ArgumentNotNullOrEmpty(Delimiter, "Delimiter");
            Delimiter = "|"; // hard code for now

            foreach (var rendering in ruleContext.SelectedRenderings)
            {
                // Simple case, no parameters, set our name-value directly (no conflicts)
                if (string.IsNullOrWhiteSpace(rendering.Parameters))
                {
                    rendering.Parameters = $"{Name}={Term ?? string.Empty}";
                    ruleContext.HasLayoutChange = true;
                    BotLog.Log.Info($"UPDATED rendering {rendering.ItemID} ({rendering.Placeholder}{rendering.UniqueId}) parameter {Name} added term '{Term}'");
                    Log.Info($"DESIGNBOT:: UPDATED rendering {rendering.ItemID} ({rendering.Placeholder}{rendering.UniqueId}) parameter {Name} added term '{Term}'", this);
                    continue;
                }

                var urlString = new UrlString(rendering.Parameters);
                var rawValue = urlString.Parameters[Name];

                // Missing parameter case, add our name-value to list
                if (string.IsNullOrWhiteSpace(rawValue))
                {
                    urlString.Append(Name, Term ?? string.Empty);
                    ruleContext.HasLayoutChange = true;
                    BotLog.Log.Info($"UPDATED rendering {rendering.ItemID} ({rendering.Placeholder}{rendering.UniqueId}) parameter {Name} added term '{Term}'");
                    Log.Info($"DESIGNBOT:: UPDATED rendering {rendering.ItemID} ({rendering.Placeholder}{rendering.UniqueId}) parameter {Name} added term '{Term}'", this);
                    continue;
                }

                if (!rawValue.EndsWith(Delimiter))
                    rawValue += Delimiter;

                // Case already exists
                if (string.Concat(Delimiter, HttpUtility.UrlDecode(rawValue)).Contains(string.Concat(Delimiter, HttpUtility.UrlDecode(Term), Delimiter)))
                    continue;

                rawValue += Term;
                urlString.Append(Name, rawValue);
                rendering.Parameters = urlString.GetUrl();
                ruleContext.HasLayoutChange = true;
                BotLog.Log.Info($"UPDATED rendering {rendering.ItemID} ({rendering.Placeholder}{rendering.UniqueId}) parameter {Name} added term '{Term}'");
                Log.Info($"DESIGNBOT:: UPDATED rendering {rendering.ItemID} ({rendering.Placeholder}{rendering.UniqueId}) parameter {Name} added term '{Term}'", this);
            }
        }
    }
}