using Sitecore.Data;
using Sitecore.Diagnostics;

namespace Community.Foundation.DesignBot.Rules.Actions.Parameters
{
    /// <summary>
    /// Add or update one specific parameter, maintain the rest of the parameters as is
    /// </summary>
    public class SetFriendlyParameterAction<T> : SetParameterAction<T>
    where T : DesignBotRuleContext
    {
        public string ParameterId { get; set; }
               
        public override void Apply(T ruleContext)
        {
            Assert.ArgumentNotNull(ruleContext, "ruleContext");

            // Map Parameter option
            ID paramID;
            if (ID.TryParse(ParameterId, out paramID))
            {
                Name = Sitecore.Context.Database?.GetItem(paramID)?.Name;
            }

            if (string.IsNullOrWhiteSpace(Name))
                return;

            base.Apply(ruleContext);
        }
    }
}