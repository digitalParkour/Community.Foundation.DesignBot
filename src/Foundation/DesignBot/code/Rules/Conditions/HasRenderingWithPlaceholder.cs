using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.Layouts;
using Sitecore.Rules.Conditions;
using System;
using System.Linq;

namespace Community.Foundation.DesignBot.Rules.Conditions
{
    public class HasRenderingWithPlaceholder<T> : StringOperatorCondition<T> where T : DesignBotRuleContext
    {
        public string Value { get; set; }
        public string RenderingId { get; set; }

        protected override bool Execute(T ruleContext)
        {
            Assert.ArgumentNotNull((object) ruleContext, "ruleContext");

            var renderings = ruleContext.SelectedDevice.Renderings.Cast<RenderingDefinition>().ToList();
            if (!string.IsNullOrWhiteSpace(RenderingId))
            {
                renderings = renderings.Where(x => x.ItemID.Equals(RenderingId, StringComparison.OrdinalIgnoreCase)).ToList();
            }            

            // Simple case, nothing left to check
            if (!renderings.Any()) {
                return false;
            }
            
            foreach (var r in renderings)
            {
                if (Compare(StringUtil.EnsurePrefix('/', r.Placeholder), Value))
                {
                    return true;
                }
            }
                             
            return false;
        }

    }
}