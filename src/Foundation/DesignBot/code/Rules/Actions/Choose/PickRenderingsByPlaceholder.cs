using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.Layouts;
using Sitecore.Rules.Actions;
using Sitecore.Rules.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Community.Foundation.DesignBot.Rules.Actions.Choose
{
    /// <summary>
    /// Add or update one specific parameter, maintain the rest of the parameters as is
    /// </summary>
    public class PickRenderingsByPlaceholder<T> : RuleAction<T>
    where T : DesignBotRuleContext
    {
        public string RenderingId { get; set; }
        
        public int Skip { get; set; }
        public int Take { get; set; }
        
        public string Value { get; set; }
        public string OperatorId { get; set; }

        public override void Apply(T ruleContext)
        {
            Assert.ArgumentNotNull(ruleContext, "ruleContext");

            var renderings = ruleContext.SelectedDevice.Renderings.Cast<RenderingDefinition>().ToList();
            if (!string.IsNullOrWhiteSpace(RenderingId))
            {
                renderings = renderings.Where(x => x.ItemID.Equals(RenderingId, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            var qIds = new List<string>();
            foreach (var r in renderings)
            {
                if (Compare(StringUtil.EnsurePrefix('/', r.Placeholder),Value))
                {
                    qIds.Add(r.UniqueId);
                }
            }
            renderings = renderings.Where(x => qIds.Contains(x.UniqueId)).ToList();


            if (renderings.Any())
            {
               if (Take > 0)
                {
                    foreach(var r in renderings.Skip(Skip).Take(Take) )
                    {
                        if (!ruleContext.SelectedRenderings.Any(x => x.UniqueId.Equals(r.UniqueId)))
                            ruleContext.SelectedRenderings.Add(r);
                    }
                }
                else
                {
                    foreach (var r in renderings)
                    {
                        if (!ruleContext.SelectedRenderings.Any(x => x.UniqueId.Equals(r.UniqueId)))
                            ruleContext.SelectedRenderings.Add(r);
                    }
                }
            }
            
            BotLog.Log.Info($"PICK - Attempted Add... Current Selection({ruleContext.SelectedRenderings.Count}): {string.Join(",",ruleContext.SelectedRenderings.Select(x=>x.UniqueId))}");
            Log.Info($"DESIGNBOT:: PICK - Attempted Add... Current Selection({ruleContext.SelectedRenderings.Count}): {string.Join(",", ruleContext.SelectedRenderings.Select(x => x.UniqueId))}", this);
        }

        

        /// <summary>
        /// Compares the specified value0.
        /// </summary>
        /// <param name="value0">The value0.</param>
        /// <param name="value1">The value1.</param>
        /// <returns>The boolean.</returns>
        protected bool Compare(string value0, string value1)
        {
            Assert.ArgumentNotNull(value0, "value0");
            Assert.ArgumentNotNull(value1, "value1");
            return ConditionsUtility.CompareStrings(value0, value1, this.OperatorId);
        }

        /// <summary>
        /// Gets the operator.
        /// </summary>
        /// <returns>Returns the condition operator.</returns>
        protected StringConditionOperator GetOperator()
        {
            return ConditionsUtility.GetStringConditionOperatorById(this.OperatorId);
        }
    }
}