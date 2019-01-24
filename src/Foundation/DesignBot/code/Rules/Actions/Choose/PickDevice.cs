using Sitecore.Diagnostics;
using Sitecore.Rules.Actions;

namespace Community.Foundation.DesignBot.Rules.Actions.Choose
{
    public class PickDevice<T> : RuleAction<T> where T : DesignBotRuleContext
    {
        public string DeviceId { get; set; }

        public override void Apply(T ruleContext)
        {
            Assert.ArgumentNotNull(ruleContext, nameof(ruleContext));

            if (string.IsNullOrWhiteSpace(DeviceId))
                return;

            BotLog.Log.Info($"PICK - picked device");
            Log.Info($"DESIGNBOT:: PICK - picked device", this);
            ruleContext.SelectedDevice = ruleContext.FinalLayout.GetDevice(DeviceId);
        }
    }
}

