using Sitecore.Diagnostics;
using Sitecore.Layouts;
using Sitecore.Rules.Actions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Community.Foundation.DesignBot.Rules.Actions.Remove
{
    /// <summary>
    /// Move selected renderings to index of first location of provided rendering ID
    /// </summary>
    public class DeleteRendering<T> : RuleAction<T>
    where T : DesignBotRuleContext
    {
        public string RenderingId { get; set; }

        public override void Apply(T ruleContext)
        {
            Assert.ArgumentNotNull(ruleContext, "ruleContext");

            if (!ruleContext.SelectedRenderings.Any())
            {
                BotLog.Log.Info($"DELETE - skipped, no selected renderings");
                Log.Info($"DESIGNBOT:: DELETE - skipped, no selected renderings", this);
                return;
            }

            // Assume remove all selected
            var uids = ruleContext.SelectedRenderings.Select(x => x.UniqueId).ToArray();

            // Case to filter by type
            if(!string.IsNullOrWhiteSpace(RenderingId))
            { 
                uids = ruleContext.SelectedRenderings.Where(x => x.ItemID.Equals(RenderingId, StringComparison.OrdinalIgnoreCase)).Select(x => x.UniqueId).ToArray();
                if (!uids.Any())
                {
                    BotLog.Log.Info($"DELETE - skipped, no target renderingId {RenderingId}");
                    Log.Info($"DESIGNBOT:: DELETE - skipped, no target renderingId {RenderingId}", this);
                    return;
                }
            }
            
            // Build list of indexes from current selection (backwards)
            var indexes = new List<int>();
            for (var i = ruleContext.SelectedDevice.Renderings.Count - 1; i >= 0 ; i--)
            {
                var r = ruleContext.SelectedDevice.Renderings[i] as RenderingDefinition;
                if (uids.Contains(r.UniqueId))
                { 
                    indexes.Add(i);
                }
            }

            // Remove (backwards - so index is descending to keep indexes accurate while deleting)
            foreach (var index in indexes)
            {
                ruleContext.SelectedDevice.Renderings.RemoveAt(index);
                ruleContext.HasLayoutChange = true;
            }

            BotLog.Log.Info($"DELETE - removed all selected occurences of {RenderingId}");
            Log.Info($"DESIGNBOT:: DELETE - removed all selected occurences of {RenderingId}", this);
        }
    }
}