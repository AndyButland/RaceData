using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Deploy;
using Umbraco.Deploy.Connectors.ServiceConnectors;
using Umbraco.Deploy.Exceptions;
using static Umbraco.Core.Constants;

namespace RaceData.CustomData
{
    [UdiDefinition(AppConstants.UdiEntityTypes.Team, UdiType.GuidUdi)]
    public class TeamServiceConnector : ServiceConnectorBase<TeamArtifact, GuidUdi, ArtifactDeployState<TeamArtifact, Team>>
    {
        private readonly IRaceDataService _raceDataService;

        public TeamServiceConnector(IRaceDataService raceDataService) => _raceDataService = raceDataService;

        public override TeamArtifact GetArtifact(object o)
        {
            var entity = o as Team;
            if (entity == null)
            {
                throw new InvalidEntityTypeException($"Unexpected entity type \"{o.GetType().FullName}\".");
            }

            return GetArtifact(entity.GetUdi(), entity);
        }

        public override TeamArtifact GetArtifact(GuidUdi udi)
        {
            EnsureType(udi);
            var team = _raceDataService.GetTeamById(udi.Guid);

            return GetArtifact(udi, team);
        }

        private TeamArtifact GetArtifact(GuidUdi udi, Team team)
        {
            if (team == null)
            {
                return null;
            }

            var dependencies = new ArtifactDependencyCollection();
            var artifact = Map(udi, team, dependencies);
            artifact.Dependencies = dependencies;

            return artifact;
        }

        private TeamArtifact Map(GuidUdi udi, Team team, ICollection<ArtifactDependency> dependencies)
        {
            var artifact = new TeamArtifact(udi)
            {
                Alias = team.Name,
                Name = team.Name,
            };

            return artifact;
        }

        private string[] ValidOpenSelectors => new[]
        {
            DeploySelector.This,
            DeploySelector.ThisAndDescendants,
            DeploySelector.DescendantsOfThis
        };
        private const string OpenUdiName = "All Teams";

        public override void Explode(UdiRange range, List<Udi> udis)
        {
            EnsureType(range.Udi);
            EnsureSelector(range, ValidOpenSelectors);

            if (range.Udi.IsRoot)
            {
                udis.AddRange(_raceDataService.GetTeams().Select(e => e.GetUdi()));
                udis.AddRange(_raceDataService.GetRiders().Select(e => e.GetUdi()));
            }
            else
            {
                var entity = _raceDataService.GetTeamById(((GuidUdi)range.Udi).Guid);
                if (entity == null) return;
                switch (range.Selector)
                {
                    case DeploySelector.This:
                        udis.Add(entity.GetUdi());
                        break;
                    case DeploySelector.ThisAndDescendants:
                        udis.Add(entity.GetUdi());
                        udis.AddRange(_raceDataService.GetRidersForTeam(entity.Id).Select(x => x.GetUdi()));
                        break;
                    default:
                        throw new InvalidOperationException($"Unexpected selector: '{range.Selector}'.");
                }
            }
        }

        public override NamedUdiRange GetRange(string entityType, string sid, string selector)
        {
            if (sid == "-1")
            {
                EnsureSelector(selector, ValidOpenSelectors);
                return new NamedUdiRange(Udi.Create(AppConstants.UdiEntityTypes.Team), OpenUdiName, selector);
            }

            if (!Guid.TryParse(RemovePrefix(sid), out Guid id))
            {
                throw new ArgumentException("Invalid identifier.", nameof(sid));
            }

            var e = _raceDataService.GetTeamById(id);
            if (e == null)
            {
                throw new ArgumentException("Could not find an entity with the specified identifier.", nameof(sid));
            }

            return GetRange(e, selector);
        }

        private string RemovePrefix(string sid) => sid.Replace(AppConstants.TreeNodeIdPrefixes.Team, string.Empty);

        public override NamedUdiRange GetRange(GuidUdi udi, string selector)
        {
            EnsureType(udi);

            if (udi.IsRoot)
            {
                EnsureSelector(selector, ValidOpenSelectors);
                return new NamedUdiRange(udi, OpenUdiName, selector);
            }

            var team = _raceDataService.GetTeamById(udi.Guid);
            if (team == null)
            { 
                throw new ArgumentException("Could not find an entity with the specified identifier.", nameof(udi));
            }

            return GetRange(team, selector);
        }

        private static NamedUdiRange GetRange(Team e, string selector) => new NamedUdiRange(e.GetUdi(), e.Name, selector);

        public override ArtifactDeployState<TeamArtifact, Team> ProcessInit(TeamArtifact art, IDeployContext context)
        {
            EnsureType(art.Udi);

            var entity = _raceDataService.GetTeamById(art.Udi.Guid);

            return ArtifactDeployState.Create(art, entity, this, 1);
        }

        public override void Process(ArtifactDeployState<TeamArtifact, Team> state, IDeployContext context, int pass)
        {
            switch (pass)
            {
                case 1:
                    Pass1(state, context);
                    state.NextPass = 2;
                    break;
                default:
                    state.NextPass = -1; // exit
                    break;
            }
        }

        private void Pass1(ArtifactDeployState<TeamArtifact, Team> state, IDeployContext context)
        {
            var artifact = state.Artifact;

            artifact.Udi.EnsureType(AppConstants.UdiEntityTypes.Team);

            var isNew = state.Entity == null;

            var entity = state.Entity ?? new Team { Id = artifact.Udi.Guid };

            entity.Name = artifact.Name;

            if (isNew)
            {
                _raceDataService.AddTeam(entity);
            }
            else
            {
                _raceDataService.UpdateTeam(entity);
            }
        }
    }
}
