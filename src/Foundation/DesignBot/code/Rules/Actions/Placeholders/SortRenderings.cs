using Community.Foundation.DesignBot.Comparer;
using Sitecore.Diagnostics;
using Sitecore.Layouts;
using Sitecore.Rules.Actions;
using System.Text;

namespace Community.Foundation.DesignBot.Rules.Actions.Placeholders
{
    /// <summary>
    /// Order all rendering by placeholder path
    /// </summary>
    public class SortRenderings<T> : RuleAction<T>
    where T : DesignBotRuleContext
    {
        public override void Apply(T ruleContext)
        {
            Assert.ArgumentNotNull(ruleContext, "ruleContext");
            
            BotLog.Log.Info($"SORTING - all renderings by placeholder paths");
            Log.Info($"DESIGNBOT:: SORTING - all renderings by placeholder paths", this);

            var stamp1 = GenerateStamp(ruleContext.SelectedDevice);

            var comparer = new RenderingComparer();
            ruleContext.SelectedDevice.Renderings.Sort(comparer);

            var stamp2 = GenerateStamp(ruleContext.SelectedDevice);
            if(stamp1 != stamp2)
            {
                ruleContext.HasLayoutChange = true;
            }
        }

        /// <summary>
        ///  Purpose: Make a string that will change if the sort order of the renderings change
        ///  Implementation: sequential string of renderings' unique ID
        /// </summary>
        /// <param name="defn"></param>
        /// <returns></returns>
        public string GenerateStamp(DeviceDefinition defn) {
            var hash = new StringBuilder();
            foreach (RenderingDefinition rendering in defn.Renderings) {
                hash.Append(rendering.UniqueId);
            }
            return hash.ToString();
        }
    }
}