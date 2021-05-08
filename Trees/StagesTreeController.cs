using System.Net.Http.Formatting;
using Umbraco.Core;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;

namespace RaceData.CustomData
{
    [Tree("content", AppConstants.TreeAliases.Stages, TreeTitle = "Stages", SortOrder = 5, TreeUse = TreeUse.Main)]
    [PluginController(AppConstants.PluginName)]
    public class StagesTreeController : TreeController
    {
        private readonly IRaceDataService _raceDataService;

        public StagesTreeController(IRaceDataService raceDataService) => _raceDataService = raceDataService;

        protected override TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings)
        {
            var nodes = new TreeNodeCollection();

            if (id == Constants.System.Root.ToInvariantString())
            {
                foreach (var stage in _raceDataService.GetStages())
                {
                    var node = CreateTreeNode(stage.Id.ToString(), "-1", queryStrings, stage.Name, "icon-activity", false);
                    nodes.Add(node);
                }
            }

            return nodes;
        }

        protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings)
        {
            var menu = new MenuItemCollection();
            if (id == Constants.System.Root.ToInvariantString())
            {
                menu.Items.Add(new RefreshNode(Services.TextService, true));
            }

            return menu;
        }

        protected override TreeNode CreateRootNode(FormDataCollection queryStrings)
        {
            var root = base.CreateRootNode(queryStrings);
            root.Icon = "icon-library";
            root.HasChildren = true;
            return root;
        }
    }
}
