using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.Layouts;
using Sitecore.Rules.Actions;

namespace Community.Foundation.DesignBot.Rules.Actions.Placeholders
{
    /// <summary>
    /// Moves selected renderings within "FromPath" to live within "ToPath" honoring nested sub-structure.
    /// Uses selected device (defaults to default device, but can be overriden with a previous SetDevice action)
    /// NOTE: Sets ruleContext.HasLayoutChange to TRUE if change was made to any renderings
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MoveToPlaceholder<T> : RuleAction<T> where T : DesignBotRuleContext
    {
        public string FromPath { get; set; }
        public string ToPath { get; set; }

        public override void Apply(T ruleContext)
        {
            Assert.ArgumentNotNull(ruleContext, nameof(ruleContext));

            if (string.IsNullOrWhiteSpace(FromPath) || string.IsNullOrWhiteSpace(ToPath))
                return;
            if (FromPath.Equals(ToPath))
                return;

            // normalize input, no trailing slash
            FromPath = StringUtil.EnsurePrefix('/',FromPath.TrimEnd('/'));
            ToPath = StringUtil.EnsurePrefix('/', ToPath.TrimEnd('/'));

            // Use all renderings of selected device
            var renderings = ruleContext.SelectedRenderings;

            // Handle Special case (target lives inside broader source)
            // Solution: flatten in-between problem paths to be FromPath
            if (ToPath.TrimStart('/').StartsWith(FromPath.TrimStart('/')))
            {
                var segments = ToPath.Split(new[] { '/' }, System.StringSplitOptions.RemoveEmptyEntries);
                // For each problem path, backwards, between FROM and TO.. (ie given /A/B/C/D => /A/B/C, /A/B)
                for (var s = segments.Length - 1; s > 1; s--) // 3 .. 2 => 3, 2
                {
                    string problemPath = string.Empty;
                    for (var i = 0; i < s; i++) // 0, 1, 2; 0, 1; 
                    {
                        problemPath = string.Concat(problemPath, "/", segments[i]);
                    }
                    // Set to FROM path
                    foreach (var r in renderings)
                    {
                        if (r.Placeholder.Equals(problemPath))
                            r.Placeholder = FromPath;
                    }
                }
            }

            // Move to new placeholder (maintaining nested structure)
            foreach (var r in renderings)
            {
                if(TryReplacePath(r, FromPath, ToPath))
                    ruleContext.HasLayoutChange = true;
            }

        }

        /// <summary>
        /// Moves rendering to new placeholder path honoring nested structure
        /// </summary>
        /// <param name="r"></param>
        /// <param name="from">segment(s) to disgard</param>
        /// <param name="to">segment(s) to use as new root path (in place of from path)</param>
        /// <returns>True if change occurred else false</returns>
        protected bool TryReplacePath(RenderingDefinition r, string from, string to)
        {
            // Ignore renderings not within "from" path
            // Compare carefully as "main" or first placeholder often does not have the initial '/'
            if (!r.Placeholder.TrimStart('/').StartsWith(from.TrimStart('/')))
                return false;

            // Ignore renderings already within "to" path
            // Compare carefully as "main" or first placeholder often does not have the initial '/'
            if (r.Placeholder.TrimStart('/').StartsWith(to.TrimStart('/')))
                return false;
            
            if (
                r.Placeholder.Equals(from) // simple case
                || r.Placeholder.Equals(from.TrimStart('/')) // single segment case
                )
            {
                r.Placeholder = to;
                BotLog.Log.Info($"MOVED rendering {r.ItemID} ({r.Placeholder}{r.UniqueId}) from {r.Placeholder} to '{to}'");
                Log.Info($"DESIGNBOT:: MOVED rendering {r.ItemID} ({r.Placeholder}{r.UniqueId}) from {r.Placeholder} to '{to}'", this);
                return true;
            }                
            else if (r.Placeholder.StartsWith(from + "/")) // nested case
            {
                var target = r.Placeholder.Replace(from + "/", to + "/");
                BotLog.Log.Info($"MOVED rendering {r.ItemID} ({r.Placeholder}{r.UniqueId}) from {r.Placeholder} to '{target}'");
                Log.Info($"DESIGNBOT:: MOVED rendering {r.ItemID} ({r.Placeholder}{r.UniqueId}) from {r.Placeholder} to '{target}'", this);
                r.Placeholder = target;
                return true;
            }

            return false;
        }
    }
}

