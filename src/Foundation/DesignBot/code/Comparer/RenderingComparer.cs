using Sitecore;
using Sitecore.Layouts;
using System.Collections;

namespace Community.Foundation.DesignBot.Comparer
{
    public class RenderingComparer : IComparer
    {
        public int Compare(RenderingDefinition x, RenderingDefinition y)
        {
            if (x == null || y == null)
                return 0;

            return string.Compare(
                StringUtil.EnsurePrefix('/', x.Placeholder),
                StringUtil.EnsurePrefix('/', y.Placeholder)
            );
        }

        public int Compare(object x, object y)
        {
            return Compare(x as RenderingDefinition, y as RenderingDefinition);
        }
    }
}