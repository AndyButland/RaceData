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
    [UdiDefinition(AppConstants.UdiEntityTypes.Rider, UdiType.GuidUdi)]
    public class RiderServiceConnector : ServiceConnectorBase<RiderArtifact, GuidUdi, ArtifactDeployState<RiderArtifact, Rider>>
    {
        private readonly IRaceDataService _raceDataService;

        public RiderServiceConnector(IRaceDataService raceDataService) => _raceDataService = raceDataService;

        public override RiderArtifact GetArtifact(object o)
        {
            var entity = o as Rider;
            if (entity == null)
            {
                throw new InvalidEntityTypeException($"Unexpected entity type \"{o.GetType().FullName}\".");
            }

            return GetArtifact(entity.GetUdi(), entity);
        }

        public override RiderArtifact GetArtifact(GuidUdi udi)
        {
            EnsureType(udi);
            var rider = _raceDataService.GetRiderById(udi.Guid);

            return GetArtifact(udi, rider);
        }

        private RiderArtifact GetArtifact(GuidUdi udi, Rider rider)
        {
            if (rider == null)
            {
                return null;
            }

            var dependencies = new ArtifactDependencyCollection();
            var artifact = Map(udi, rider, dependencies);
            artifact.Dependencies = dependencies;

            return artifact;
        }

        private RiderArtifact Map(GuidUdi udi, Rider rider, ICollection<ArtifactDependency> dependencies)
        {
            var artifact = new RiderArtifact(udi)
            {
                Alias = rider.Name,
                Name = rider.Name,
                TeamId = rider.Team.Id,
            };

            // Team node must exist to deploy the rider.
            dependencies.Add(new ArtifactDependency(rider.Team.GetUdi(), true, ArtifactDependencyMode.Exist));

            return artifact;
        }

        private string[] ValidOpenSelectors => new[]
        {
            DeploySelector.This,
            DeploySelector.ThisAndDescendants,
            DeploySelector.DescendantsOfThis
        };
        private const string OpenUdiName = "All Riders";

        public override void Explode(UdiRange range, List<Udi> udis)
        {
            EnsureType(range.Udi);

            if (range.Udi.IsRoot)
            {
                EnsureSelector(range, ValidOpenSelectors);
                udis.AddRange(_raceDataService.GetRiders().Select(e => e.GetUdi()));
            }
            else
            {
                var entity = _raceDataService.GetRiderById(((GuidUdi)range.Udi).Guid);
                if (entity == null)
                {
                    return;
                }

                EnsureSelector(range.Selector, DeploySelector.This);
                udis.Add(entity.GetUdi());
            }
        }

        public override NamedUdiRange GetRange(string entityType, string sid, string selector)
        {
            if (sid == "-1")
            {
                EnsureSelector(selector, ValidOpenSelectors);
                return new NamedUdiRange(Udi.Create(AppConstants.UdiEntityTypes.Rider), OpenUdiName, selector);
            }

            if (!Guid.TryParse(RemovePrefix(sid), out Guid id))
            {
                throw new ArgumentException("Invalid identifier.", nameof(sid));
            }

            var e = _raceDataService.GetRiderById(id);
            if (e == null)
            {
                throw new ArgumentException("Could not find an entity with the specified identifier.", nameof(sid));
            }

            return GetRange(e, selector);
        }

        private string RemovePrefix(string sid) => sid.Replace(AppConstants.TreeNodeIdPrefixes.Rider, string.Empty);

        public override NamedUdiRange GetRange(GuidUdi udi, string selector)
        {
            EnsureType(udi);

            if (udi.IsRoot)
            {
                EnsureSelector(selector, ValidOpenSelectors);
                return new NamedUdiRange(udi, OpenUdiName, selector);
            }

            var rider = _raceDataService.GetRiderById(udi.Guid);
            if (rider == null)
            { 
                throw new ArgumentException("Could not find an entity with the specified identifier.", nameof(udi));
            }

            return GetRange(rider, selector);
        }

        private static NamedUdiRange GetRange(Rider e, string selector) => new NamedUdiRange(e.GetUdi(), e.Name, selector);

        public override ArtifactDeployState<RiderArtifact, Rider> ProcessInit(RiderArtifact art, IDeployContext context)
        {
            EnsureType(art.Udi);

            var entity = _raceDataService.GetRiderById(art.Udi.Guid);

            return ArtifactDeployState.Create(art, entity, this, 1);
        }

        public override void Process(ArtifactDeployState<RiderArtifact, Rider> state, IDeployContext context, int pass)
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

        private void Pass1(ArtifactDeployState<RiderArtifact, Rider> state, IDeployContext context)
        {
            var artifact = state.Artifact;

            artifact.Udi.EnsureType(AppConstants.UdiEntityTypes.Rider);

            var isNew = state.Entity == null;

            var entity = state.Entity ?? new Rider { Id = artifact.Udi.Guid };

            entity.Name = artifact.Name;
            entity.Team = _raceDataService.GetTeamById(artifact.TeamId);

            if (isNew)
            {
                _raceDataService.AddRider(entity);
            }
            else
            {
                _raceDataService.UpdateRider(entity);
            }
        }
    }
}
