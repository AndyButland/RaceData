using System;
using System.Net.Http.Formatting;
using Umbraco.Core;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;

namespace RaceData.CustomData
{
    [Tree("content", AppConstants.TreeAliases.Teams, TreeTitle = "Teams", SortOrder = 6, TreeUse = TreeUse.Main)]
    [PluginController(AppConstants.PluginName)]
    public class TeamsTreeController : TreeController
    {
        private readonly IRaceDataService _raceDataService;

        public TeamsTreeController(IRaceDataService raceDataService) => _raceDataService = raceDataService;

        protected override TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings)
        {
            var nodes = new TreeNodeCollection();

            if (id == Constants.System.Root.ToInvariantString())
            {
                foreach (var teams in _raceDataService.GetTeams())
                {
                    var node = CreateTreeNode(AppConstants.TreeNodeIdPrefixes.Team + teams.Id.ToString(), "-1", queryStrings, teams.Name, "icon-flag", true, $"{AppConstants.PluginName}/{AppConstants.TreeAliases.Teams}/teamEdit/{teams.Id}");
                    nodes.Add(node);
                }
            }
            else if (id.StartsWith(AppConstants.TreeNodeIdPrefixes.Team))
            {
                var teamId = Guid.Parse(id.Replace(AppConstants.TreeNodeIdPrefixes.Team, string.Empty));
                foreach (var rider in _raceDataService.GetRidersForTeam(teamId))
                {
                    var node = CreateTreeNode(AppConstants.TreeNodeIdPrefixes.Rider + rider.Id.ToString(), id, queryStrings, rider.Name, "icon-employee", false, $"{AppConstants.PluginName}/{AppConstants.TreeAliases.Teams}/riderEdit/{rider.Id}");
                    nodes.Add(node);
                }
            }

            return nodes;
        }

        protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings)
        {
            var menu = new MenuItemCollection();
            if (id == Constants.System.Root.ToInvariantString() || id.StartsWith(AppConstants.TreeNodeIdPrefixes.Team))
            {
                menu.Items.Add(new RefreshNode(Services.TextService, true));
            }

            return menu;
        }

        protected override TreeNode CreateRootNode(FormDataCollection queryStrings)
        {
            var root = base.CreateRootNode(queryStrings);
            root.Icon = "icon-categories";
            root.HasChildren = true;
            root.RoutePath = $"{AppConstants.PluginName}/{AppConstants.TreeAliases.Teams}/teamsOverview";
            return root;
        }
    }
}
