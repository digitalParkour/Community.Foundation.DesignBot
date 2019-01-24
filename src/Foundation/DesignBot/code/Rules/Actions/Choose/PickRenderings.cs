using Community.Foundation.DesignBot.Extensions;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Layouts;
using Sitecore.Rules.Actions;
using Sitecore.Rules.Conditions;
using Sitecore.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Community.Foundation.DesignBot.Rules.Actions.Choose
{
    /// <summary>
    /// Add or update one specific parameter, maintain the rest of the parameters as is
    /// </summary>
    public class PickRenderings<T> : RuleAction<T>
    where T : DesignBotRuleContext
    {
        public string PlaceholderPath { get; set; }
        public string RenderingId { get; set; }

        public bool First { get; set; }
        public bool Last { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }

        public string Param { get; set; }
        public ID DsFieldId { get; set; }
        public string Value { get; set; }
        public string OperatorId { get; set; }

        public override void Apply(T ruleContext)
        {
            Assert.ArgumentNotNull(ruleContext, "ruleContext");

            var renderings = ruleContext.SelectedDevice.Renderings.Cast<RenderingDefinition>().ToList();
            if (!string.IsNullOrWhiteSpace(PlaceholderPath))
            {
                renderings = renderings.Where(x => x.Placeholder.Equals(PlaceholderPath, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            if (!string.IsNullOrWhiteSpace(RenderingId))
            {
                renderings = renderings.Where(x => x.ItemID.Equals(RenderingId, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            if (!string.IsNullOrWhiteSpace(Param))
            {
                renderings = renderings.Where(MatchesParamCriteria).ToList();
            }
            if (DsFieldId != (ID)null)
            {
                var qIds = new List<string>();
                foreach(var r in renderings)
                {
                    var dsItem = r.GetDatasourceItem(ruleContext.Item);
                    if (dsItem == null)
                        continue;
                    // Special Empty case (shortcut)
                    if (Value.Equals("EMPTY", StringComparison.OrdinalIgnoreCase))
                    {
                        if (string.IsNullOrWhiteSpace(dsItem[DsFieldId]))
                        {
                            qIds.Add(r.UniqueId);
                        }
                    }
                    else if (dsItem[DsFieldId] != null && Compare(dsItem[DsFieldId], Value))
                    {
                        qIds.Add(r.UniqueId);
                    }
                }
                renderings = renderings.Where(x => qIds.Contains(x.UniqueId)).ToList();
            }

            if (renderings.Any())
            {
                if (First)
                {
                    var r = renderings.First();
                    if (!ruleContext.SelectedRenderings.Any(x=>x.UniqueId.Equals(r.UniqueId)))
                        ruleContext.SelectedRenderings.Add(r);
                }
                else if (Last)
                {
                    var r = renderings.Last();
                    if (!ruleContext.SelectedRenderings.Any(x => x.UniqueId.Equals(r.UniqueId)))
                        ruleContext.SelectedRenderings.Add(r);
                }
                else if (Take > 0)
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


        protected bool MatchesParamCriteria(RenderingDefinition r)
        {
            var urlString = new UrlString(r.Parameters);
            if (string.IsNullOrWhiteSpace(urlString[Param]))
                return false;
            return Compare(HttpUtility.UrlDecode(urlString[Param]), HttpUtility.UrlDecode(Value));
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