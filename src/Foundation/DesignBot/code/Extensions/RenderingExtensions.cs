using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Layouts;

namespace Community.Foundation.DesignBot.Extensions
{
    public static class RenderingExtensions
    {

        public static Item GetDatasourceItem(this RenderingDefinition rendering, Item page) {

            var db = page?.Database;

            // Handle SXA style datasources
            if (page != null && rendering.Datasource.StartsWith("local:"))
            {
                // Build absolute path from relative path
                return db?.GetItem($"{page.Paths.FullPath}{rendering.Datasource.Remove(0, "local:".Length)}");
            }            

            ID dataId;
            return ID.TryParse(rendering.Datasource, out dataId)
                ? db?.GetItem(dataId) // by ID
                : db?.GetItem(rendering.Datasource); // by Path
        }
    }
}