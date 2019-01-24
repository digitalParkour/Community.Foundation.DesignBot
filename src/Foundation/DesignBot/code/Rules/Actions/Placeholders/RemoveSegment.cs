using Sitecore.Diagnostics;
using Sitecore.Rules.Actions;
using Sitecore.Rules.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Community.Foundation.DesignBot.Rules.Actions.Placeholders
{
    /// <summary>
    /// Add or update one specific parameter, maintain the rest of the parameters as is
    /// </summary>
    public class RemoveSegment<T> : RuleAction<T>
    where T : DesignBotRuleContext
    {
        public string Value { get; set; }
        public string OperatorId { get; set; }

        public override void Apply(T ruleContext)
        {
            Assert.ArgumentNotNull(ruleContext, "ruleContext");

            BotLog.Log.Info($"PLACEHOLDER - removing segment compared to '{Value}'");
            Log.Info($"DESIGNBOT:: PLACEHOLDER - removing segment compared to '{Value}'", this);

            // Use selected renderings only
            var renderings = ruleContext.SelectedRenderings;
            
            foreach (var r in renderings)
            {
                var segments = r.Placeholder.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                var keep = new List<string>();
                foreach(var segment in segments)
                {
                    if (!Compare(segment, Value)) {
                        keep.Add(segment);
                    }
                }
                var newPath = keep.Count > 1 ? string.Concat("/", string.Join("/", keep)) : keep.FirstOrDefault();

                if (r.Placeholder != newPath)
                {
                    r.Placeholder = newPath;
                    ruleContext.HasLayoutChange = true;
                }
            }
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