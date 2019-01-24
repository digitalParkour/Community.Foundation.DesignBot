using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Rules.Conditions;
using System;

namespace Community.Foundation.DesignBot.Rules.Conditions
{
    public class HasLayout<T> : WhenCondition<T> where T : DesignBotRuleContext
    {
        public ID LayoutId { get; set; }
        
        protected override bool Execute(T ruleContext)
        {
            Assert.ArgumentNotNull((object) ruleContext, "ruleContext");
            
           return ruleContext.SelectedDevice.Layout.Equals(LayoutId.ToString(), StringComparison.OrdinalIgnoreCase);
        }

    }
}