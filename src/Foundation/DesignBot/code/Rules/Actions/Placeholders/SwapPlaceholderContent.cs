using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.Layouts;
using Sitecore.Rules.Actions;

namespace Community.Foundation.DesignBot.Rules.Actions.Placeholders
{
    /// <summary>
    /// Swaps placeholder path strings, effectively swapping components in placeholder A and placeholder B.
    /// Uses selected device (defaults to default device, but can be overriden with a previous SetDevice action)
    /// NOTE: Sets ruleContext.HasLayoutChange to TRUE if change was made to any renderings
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SwapPlaceholderContent<T> : RuleAction<T> where T : DesignBotRuleContext
    {
        public string PathA { get; set; }
        public string PathB { get; set; }

        public override void Apply(T ruleContext)
        {
            Assert.ArgumentNotNull(ruleContext, nameof(ruleContext));

            if (string.IsNullOrWhiteSpace(PathA) || string.IsNullOrWhiteSpace(PathB))
                return;

            // normalize input, ensure initial slash, avoid trailing slash
            PathA = StringUtil.EnsurePrefix('/', PathA.TrimEnd('/'));
            PathB = StringUtil.EnsurePrefix('/', PathB.TrimEnd('/'));

            // Ensure Path A is the longer path (most specific in case of matching nested placeholder paths)
            if (PathB.Length > PathA.Length) {
                var x = PathA;
                PathA = PathB;
                PathB = x;
            }

            // Use all renderings of selected device
            var renderings = ruleContext.SelectedRenderings;

            // Set PathA to temporary PathX
            var pathX = "XXXSWAPXXX";
            foreach(var r in renderings)
            {
                if (
                    TrySwapPath(r, PathA, pathX)
                   )
                    ruleContext.HasLayoutChange = true;
            }

            // Set PathB to PathA, PathX to PathB
            foreach (var r in renderings)
            {
                if (
                    TrySwapPath(r, PathB, PathA)
                   )
                    ruleContext.HasLayoutChange = true;
                else
                    TrySwapPath(r, pathX, PathB);
            }

        }

        /// <summary>
        /// Swaps partial placeholder path x to use partial path y (where applicable) on rendering r
        /// </summary>
        /// <param name="r"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>True if change occurred else false</returns>
        protected bool TrySwapPath(RenderingDefinition r, string x, string y)
        {
            if (r.Placeholder.Equals(x))
            {
                BotLog.Log.Info($"MOVED rendering {r.ItemID} ({r.Placeholder}{r.UniqueId}) from {r.Placeholder} to '{y}'");
                Log.Info($"DESIGNBOT:: MOVED rendering {r.ItemID} ({r.Placeholder}{r.UniqueId}) from {r.Placeholder} to '{y}'", this);
                r.Placeholder = y;
                return true;
            }                
            else if (r.Placeholder.StartsWith(x + "/"))
            {
                var target = r.Placeholder.Replace(x + "/", y + "/");
                BotLog.Log.Info($"MOVED rendering {r.ItemID} ({r.Placeholder}{r.UniqueId}) from {r.Placeholder} to '{target}'");
                Log.Info($"DESIGNBOT:: MOVED rendering {r.ItemID} ({r.Placeholder}{r.UniqueId}) from {r.Placeholder} to '{target}'", this);
                r.Placeholder = target;
                return true;
            }

            return false;
        }
    }
}

