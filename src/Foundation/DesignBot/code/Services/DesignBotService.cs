using Community.Foundation.DesignBot.Extensions;
using Community.Foundation.DesignBot.Rules;
using Sitecore;
using Sitecore.ContentTesting.Configuration;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Rules;
using Sitecore.SecurityModel;
using System;

namespace Community.Foundation.DesignBot.Services
{
    public interface IDesignBotService {
        string ApplyDesign(Item ruleProgram, Item page);
    }


    public class DesignBotService
    {
        private readonly ID SxaDataTemplateId = new ID("{1C82E550-EBCD-4E5D-8ABD-D50D0809541E}");

        public string ApplyDesign(Item ruleProgram, Item page, string crawlDepth = null) {
            BotLog.Log.Info($"::START:: program {ruleProgram?.DisplayName} on page {page.DisplayName}, {page.ID}, crawl depth {crawlDepth}");
            Log.Info($"DESIGNBOT::START:: program {ruleProgram?.DisplayName} on page {page.DisplayName}, {page.ID}, crawl depth {crawlDepth}", this);

            // Get design rules
            RuleList<DesignBotRuleContext> rules = RuleFactory.GetRules<DesignBotRuleContext>(ruleProgram, "Rule");
            if (rules != null)
            {
                // Apply design program
                int depth;
                if (!int.TryParse(crawlDepth ?? "0", out depth))
                    depth = 0;
                depth = Math.Max(0, depth);

                using (new SecurityDisabler())
                {
                    using (new ScreenshotGenerationDisabler())
                    {
                        RunProgram_r(rules, page, depth);
                    }
                }
            }

            BotLog.Log.Info($"::END:: program {ruleProgram?.DisplayName} on page {page.DisplayName}, {page.ID}, crawl depth {crawlDepth}");
            Log.Info($"DESIGNBOT::END:: program {ruleProgram?.DisplayName} on page {page.DisplayName}, {page.ID}, crawl depth {crawlDepth}", this);
            return "Success";
                    // item.Editing.BeginEdit();
                    // item.Editing.EndEdit(updateStatistics: true, silent: false);
                    // item.Editing.CancelEdit();
        }

        public void RunProgram_r(RuleList<DesignBotRuleContext> rules, Item page, int depth)
        {
            if (depth < 0 || page == null)
                return;

            // skip datasource items
            if (page.TemplateID.Equals(SxaDataTemplateId))
                return;

            BotLog.Log.Info($"Processing item {page.DisplayName}, {page.ID}...");
            Log.Info($"DESIGNBOT:: Processing item {page.DisplayName}, {page.ID}...", this);

            try
            {
                var ruleContext = new DesignBotRuleContext(page);

                // Run conditions and actions
                rules.Run(ruleContext);

                if (ruleContext.HasLayoutChange)
                {
                    // Create new version for easy undo (delete version)
                    page = page.AsNewVersion(matchWorkflowState: true);
                    ItemUtil.SetLayoutDetails(page, ruleContext.SharedLayout.ToXml(), ruleContext.FinalLayout.ToXml());
                }
            }
            catch (Exception ex)
            {
                BotLog.Log.Error($"Error applying program to item {page?.ID}", ex);
                Log.Error($"DESIGNBOT:: Error applying program to item {page?.ID}", ex, this);
            }

            // Recursion
            depth--;
            if (depth < 0 || !page.HasChildren)
                return;
            var children = page.GetChildren();
            foreach (Item child in children) {
                RunProgram_r(rules, child, depth);
            }
            
        }
    }
}