using log4net;

namespace Community.Foundation.DesignBot
{
    public static class BotLog
    {
        private static ILog log;
        public static ILog Log
        {
            get
            {
                return log ?? (log = Sitecore.Diagnostics.LoggerFactory.GetLogger(typeof(BotLog)));
            }
        }
    }
}