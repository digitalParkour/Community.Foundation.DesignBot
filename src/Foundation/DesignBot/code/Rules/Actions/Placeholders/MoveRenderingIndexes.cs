using Sitecore.Diagnostics;
using Sitecore.Layouts;
using Sitecore.Rules.Actions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Community.Foundation.DesignBot.Rules.Actions.Placeholders
{
    /// <summary>
    /// Move selected renderings to index of first location of provided rendering ID
    /// </summary>
    public class MoveRenderingIndexes<T> : RuleAction<T>
    where T : DesignBotRuleContext
    {
        public string RenderingId { get; set; }
        public bool IsBefore { get; set; }

        public override void Apply(T ruleContext)
        {
            Assert.ArgumentNotNull(ruleContext, "ruleContext");
            Assert.ArgumentNotNull(RenderingId, "RenderingId");

            if (!ruleContext.SelectedRenderings.Any())
                return;

            var targetIndex = -1;
            var isMatch = false;
            foreach (RenderingDefinition r in ruleContext.SelectedDevice.Renderings)
            {
                targetIndex++;
                if (r.ItemID.Equals(RenderingId, StringComparison.OrdinalIgnoreCase)) {
                    isMatch = true;
                    break;
                }
            }
            if (!isMatch)
            {
                BotLog.Log.Info($"MOVE - skipped, no target renderingId {RenderingId}");
                Log.Info($"DESIGNBOT:: MOVE - skipped, no target renderingId {RenderingId}", this);
                return;
            }

            // Handle case of placing after target vs before
            if (!IsBefore)
                targetIndex++;
            
            BotLog.Log.Info($"MOVE - renderings to index {targetIndex} the first occurence of {RenderingId}");
            Log.Info($"DESIGNBOT:: MOVE - renderings to index {targetIndex} the first occurence of {RenderingId}", this);

            var indexes = new List<int>();
            var queueFront = new List<RenderingDefinition>();
            var queueBehind = new List<RenderingDefinition>();
            var uids = ruleContext.SelectedRenderings.Select(x => x.UniqueId).ToArray();

            // Build list of indexes from current selection (backwards)
            for (var i = ruleContext.SelectedDevice.Renderings.Count - 1; i >= 0 ; i--)
            {
                var r = ruleContext.SelectedDevice.Renderings[i] as RenderingDefinition;
                if (uids.Contains(r.UniqueId))
                { 
                    indexes.Add(i);
                    // Isolate renderings that when removed will affect the target position index number
                    if (i < targetIndex)
                        queueFront.Add(r);
                    else
                        queueBehind.Add(r);
                }
            }

            // Remove (backwards - so index is descending to keep indexes accurate while deleting)
            foreach (var index in indexes)
            {
                ruleContext.SelectedDevice.Renderings.RemoveAt(index);
                ruleContext.HasLayoutChange = true;
            }

            targetIndex = Math.Max(0,targetIndex - queueFront.Count());
            // Add to target (backwards so items maintain same order)
            foreach (var p in queueBehind)
            {
                ruleContext.SelectedDevice.Insert(targetIndex, p);
            }
            foreach (var p in queueFront)
            {
                ruleContext.SelectedDevice.Insert(targetIndex, p);
            }
        }
    }
}