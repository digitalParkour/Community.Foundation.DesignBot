using Sitecore.Diagnostics;
using Sitecore.Layouts;
using Sitecore.Rules.Actions;
using System.Collections.Generic;
using System.Linq;

namespace Community.Foundation.DesignBot.Rules.Actions.Placeholders
{
    /// <summary>
    /// Move selected renderings to front of all listed renderings
    /// </summary>
    public class MoveRenderingsToFront<T> : RuleAction<T>
    where T : DesignBotRuleContext
    {
        public override void Apply(T ruleContext)
        {
            Assert.ArgumentNotNull(ruleContext, "ruleContext");

            if (!ruleContext.SelectedRenderings.Any())
                return;

            BotLog.Log.Info($"MOVE - renderings to front");
            Log.Info($"DESIGNBOT:: MOVE - renderings to front", this);

            var indexes = new List<int>();
            var queue = new List<RenderingDefinition>();
            var uids = ruleContext.SelectedRenderings.Select(x => x.UniqueId).ToArray();

            // Build list of indexes from current selection (backwards)
            for (var i = ruleContext.SelectedDevice.Renderings.Count - 1; i >= 0 ; i--)
            {
                var r = ruleContext.SelectedDevice.Renderings[i] as RenderingDefinition;
                if (uids.Contains(r.UniqueId))
                { 
                    indexes.Add(i);
                    queue.Add(r);
                }
            }

            // Remove (backwards - so index is descending to keep indexes accurate while deleting)
            foreach (var index in indexes)
            {
                ruleContext.SelectedDevice.Renderings.RemoveAt(index);
            }

            // Add to front (backwards so items maintain same order)
            foreach (var p in queue)
            {
                ruleContext.SelectedDevice.Insert(0, p);
                ruleContext.HasLayoutChange = true;
            }
        }
    }
}