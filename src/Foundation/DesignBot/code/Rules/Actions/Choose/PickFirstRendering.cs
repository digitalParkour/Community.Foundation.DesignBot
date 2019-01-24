namespace Community.Foundation.DesignBot.Rules.Actions.Choose
{
    /// <summary>
    /// Add or update one specific parameter, maintain the rest of the parameters as is
    /// </summary>
    public class PickFirstRendering<T> : PickRenderings<T>
    where T : DesignBotRuleContext
    {
        public PickFirstRendering()
        {
            First = true;
        }
    }
}