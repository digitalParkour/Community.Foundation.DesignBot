using System;
using System.Linq;
using System.Web;
using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.Layouts;
using Sitecore.Rules.Conditions;
using Sitecore.Text;
using Sitecore.Web;

namespace Community.Foundation.DesignBot.Rules.Conditions
{
    public class HasRenderingWithParamValue<T> : StringOperatorCondition<T> where T : DesignBotRuleContext
    {
        public string Value { get; set; }
        public string Param { get; set; }
        public string PlaceholderPath { get; set; }
        public string RenderingId { get; set; }

        protected override bool Execute(T ruleContext)
        {
            Assert.ArgumentNotNull((object) ruleContext, "ruleContext");

            var renderings = ruleContext.SelectedDevice.Renderings.Cast<RenderingDefinition>().ToList();
            if (!string.IsNullOrWhiteSpace(PlaceholderPath))
            {
                renderings = renderings.Where(x => x.Placeholder.Equals(PlaceholderPath, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            if (!string.IsNullOrWhiteSpace(RenderingId))
            {
                renderings = renderings.Where(x => x.ItemID.Equals(RenderingId, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // Simple case, not checking parameter
            if (string.IsNullOrWhiteSpace(Param)) {
                return renderings.Any();
            }

            // Param value case
            foreach (var rendering in renderings) {

                var urlString = new UrlString(rendering.Parameters);
                var rawValue = urlString.Parameters[Param];
                if (!string.IsNullOrWhiteSpace(rawValue))
                    rawValue = HttpUtility.UrlDecode(rawValue);

                if (Compare(rawValue, Value))
                    return true;
            }

            return false;
        }

    }
}