using System.Collections.Generic;
using System.Linq;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Layouts;
using Sitecore.Rules;

namespace Community.Foundation.DesignBot.Rules
{
    public class DesignBotRuleContext : RuleContext
    {
        public DesignBotRuleContext(Item item) {
            Item = item;

            var sharedLayoutField = item.Fields[FieldIDs.LayoutField];
            var finalLayoutField = item.Fields[FieldIDs.FinalLayoutField];

            SharedLayout = LayoutDefinition.Parse(LayoutField.GetFieldValue(sharedLayoutField));
            FinalLayout = LayoutDefinition.Parse(LayoutField.GetFieldValue(finalLayoutField));

            // These can be filled with rule actions
            // SelectedDatasources = new List<Item>();

            // This can be overrided with the rule action SetDevice
            SelectedDevice = FinalLayout.GetDevice(Constants.Items.Device.DefaultId);
            SelectedRenderings = new List<RenderingDefinition>(); // SelectedDevice.Renderings.Cast<RenderingDefinition>().ToList();
        }
        //    public List<Item> AllowedRenderingItems { get; set; }
        //    public ID DeviceId { get; set; }
        //    public string PlaceholderKey { get; set; }
        //    public Database ContentDatabase { get; set; }
        //    public string LayoutDefinition { get; set; }

        // Outputs
        public bool HasLayoutChange { get; set; }
        public LayoutDefinition SharedLayout { get; set; }
        public LayoutDefinition FinalLayout { get; set; }

        // Internal
        // public List<Item> SelectedDatasources { get; set; }
        public List<RenderingDefinition> SelectedRenderings { get; set; }
        public DeviceDefinition SelectedDevice { get; set; }

    }
}