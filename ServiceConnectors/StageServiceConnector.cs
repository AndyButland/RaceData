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
    [UdiDefinition(AppConstants.UdiEntityTypes.Stage, UdiType.GuidUdi)]
    public class StageServiceConnector : ServiceConnectorBase<StageArtifact, GuidUdi, ArtifactDeployState<StageArtifact, Stage>>
    {
        private readonly IRaceDataService _raceDataService;

        public StageServiceConnector(IRaceDataService raceDataService) => _raceDataService = raceDataService;

        public override StageArtifact GetArtifact(object o)
        {
            var entity = o as Stage;
            if (entity == null)
            {
                throw new InvalidEntityTypeException($"Unexpected entity type \"{o.GetType().FullName}\".");
            }

            return GetArtifact(entity.GetUdi(), entity);
        }

        public override StageArtifact GetArtifact(GuidUdi udi)
        {
            EnsureType(udi);
            var stage = _raceDataService.GetStageById(udi.Guid);

            return GetArtifact(udi, stage);
        }

        private StageArtifact GetArtifact(GuidUdi udi, Stage stage)
        {
            if (stage == null)
            {
                return null;
            }

            var dependencies = new ArtifactDependencyCollection();
            var artifact = Map(udi, stage, dependencies);
            artifact.Dependencies = dependencies;

            return artifact;
        }

        private StageArtifact Map(GuidUdi udi, Stage stage, ICollection<ArtifactDependency> dependencies)
        {
            var artifact = new StageArtifact(udi)
            {
                Alias = stage.Name,
                Name = stage.Name,
                From = stage.From,
                To = stage.To,
                Distance = stage.Distance,
            };

            return artifact;
        }

        private string[] ValidOpenSelectors => new[]
        {
            DeploySelector.This,
            DeploySelector.ThisAndDescendants,
            DeploySelector.DescendantsOfThis
        };
        private const string OpenUdiName = "All Stages";

        public override void Explode(UdiRange range, List<Udi> udis)
        {
            EnsureType(range.Udi);

            if (range.Udi.IsRoot)
            {
                EnsureSelector(range, ValidOpenSelectors);
                udis.AddRange(_raceDataService.GetStages().Select(e => e.GetUdi()));
            }
            else
            {
                var entity = _raceDataService.GetStageById(((GuidUdi)range.Udi).Guid);
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
                return new NamedUdiRange(Udi.Create(AppConstants.UdiEntityTypes.Stage), OpenUdiName, selector);
            }

            if (!Guid.TryParse(sid, out Guid id))
            {
                throw new ArgumentException("Invalid identifier.", nameof(sid));
            }

            var e = _raceDataService.GetStageById(id);
            if (e == null)
            {
                throw new ArgumentException("Could not find an entity with the specified identifier.", nameof(sid));
            }

            return GetRange(e, selector);
        }

        public override NamedUdiRange GetRange(GuidUdi udi, string selector)
        {
            EnsureType(udi);

            if (udi.IsRoot)
            {
                EnsureSelector(selector, ValidOpenSelectors);
                return new NamedUdiRange(udi, OpenUdiName, selector);
            }

            var stage = _raceDataService.GetStageById(udi.Guid);
            if (stage == null)
            { 
                throw new ArgumentException("Could not find an entity with the specified identifier.", nameof(udi));
            }

            return GetRange(stage, selector);
        }

        private static NamedUdiRange GetRange(Stage e, string selector) => new NamedUdiRange(e.GetUdi(), e.Name, selector);

        public override ArtifactDeployState<StageArtifact, Stage> ProcessInit(StageArtifact art, IDeployContext context)
        {
            EnsureType(art.Udi);

            var entity = _raceDataService.GetStageById(art.Udi.Guid);

            return ArtifactDeployState.Create(art, entity, this, 1);
        }

        public override void Process(ArtifactDeployState<StageArtifact, Stage> state, IDeployContext context, int pass)
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

        private void Pass1(ArtifactDeployState<StageArtifact, Stage> state, IDeployContext context)
        {
            var artifact = state.Artifact;

            artifact.Udi.EnsureType(AppConstants.UdiEntityTypes.Stage);

            var isNew = state.Entity == null;

            var entity = state.Entity ?? new Stage { Id = artifact.Udi.Guid };

            entity.Name = artifact.Name;
            entity.From = artifact.From;
            entity.To = artifact.To;
            entity.Distance = artifact.Distance;

            if (isNew)
            {
                _raceDataService.AddStage(entity);
            }
            else
            {
                _raceDataService.UpdateStage(entity);
            }
        }
    }
}
