namespace Community.Foundation.DesignBot.Rules.Actions.Placeholders
{
    /// <summary>
    /// Move selected renderings to index of first location of provided rendering ID
    /// </summary>
    public class MoveRenderingIndexesBefore<T> : MoveRenderingIndexes<T>
    where T : DesignBotRuleContext
    {
        public MoveRenderingIndexesBefore(){
            IsBefore = true;
        }
    }
}