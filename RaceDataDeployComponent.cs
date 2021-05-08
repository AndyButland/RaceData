using System;
using Umbraco.Core.Composing;
using Umbraco.Core.Deploy;
using Umbraco.Deploy;
using Umbraco.Deploy.Connectors.ServiceConnectors;
using Umbraco.Deploy.Transfer;

namespace RaceData.CustomData
{
    public class RaceDataDeployComponent : IComponent
    {
        private readonly IDiskEntityService _diskEntityService;
        private readonly ITransferEntityService _transferEntityService;
        private readonly IServiceConnectorFactory _serviceConnectorFactory;
        private readonly ISignatureService _signatureService;
        private readonly IRaceDataService _raceDataService;

        public RaceDataDeployComponent(
            IDiskEntityService diskEntityService,
            ITransferEntityService transferEntityService,
            IServiceConnectorFactory serviceConnectorFactory,
            ISignatureService signatureService,
            IRaceDataService raceDataService)
        {
            _diskEntityService = diskEntityService;
            _transferEntityService = transferEntityService;
            _serviceConnectorFactory = serviceConnectorFactory;
            _signatureService = signatureService;
            _raceDataService = raceDataService;
        }

        private readonly bool _transferRaceDataAsContent = true;

        public void Initialize()
        {
            if (_transferRaceDataAsContent)
            {
                _transferEntityService.RegisterTransferEntityType(
                    AppConstants.UdiEntityTypes.Stage,
                    "Stages",
                    new DeployRegisteredEntityTypeDetailOptions
                    {
                        SupportsQueueForTransfer = true,
                        SupportsQueueForTransferOfDescendents = true,
                        SupportsRestore = true,
                        PermittedToRestore = true,
                        SupportsPartialRestore = true,
                    },
                    AppConstants.TreeAliases.Stages,
                    (string routePath) => true,
                    (string nodeId) => true,
                    (string nodeId) => ParseEntityId(nodeId));

                _transferEntityService.RegisterTransferEntityType(
                    AppConstants.UdiEntityTypes.Team,
                    "Teams",
                    new DeployRegisteredEntityTypeDetailOptions
                    {
                        SupportsQueueForTransfer = true,
                        SupportsQueueForTransferOfDescendents = true,
                        SupportsRestore = true,
                        PermittedToRestore = true,
                        SupportsPartialRestore = true,
                    },
                    AppConstants.TreeAliases.Teams,
                    (string routePath) => routePath.Contains($"/teamEdit/") || routePath.Contains($"/teamsOverview"),
                    (string nodeId) => nodeId == "-1" || nodeId.StartsWith(AppConstants.TreeNodeIdPrefixes.Team),
                    (string nodeId) => ParseNodeIdFromPrefixedValue(nodeId, AppConstants.TreeNodeIdPrefixes.Team));

                _transferEntityService.RegisterTransferEntityType(
                    AppConstants.UdiEntityTypes.Rider,
                    "Riders",
                    new DeployRegisteredEntityTypeDetailOptions
                    {
                        SupportsQueueForTransfer = true,
                        SupportsRestore = true,
                        PermittedToRestore = true,
                        SupportsPartialRestore = true,
                    },
                    AppConstants.TreeAliases.Teams,
                    (string routePath) => routePath.Contains($"/riderEdit/"),
                    (string nodeId) => nodeId.StartsWith(AppConstants.TreeNodeIdPrefixes.Rider),
                    (string nodeId) => ParseNodeIdFromPrefixedValue(nodeId, AppConstants.TreeNodeIdPrefixes.Rider));
            }
            else
            {
                _diskEntityService.RegisterDiskEntityType(AppConstants.UdiEntityTypes.Stage);
            }

            _raceDataService.StageSaved += StageServiceOnSaved;

            _signatureService.RegisterHandler<RaceDataService, StageEventArgs>(nameof(IRaceDataService.StageSaved), (refresher, args) => refresher.SetSignature(GetArtifact(args)));
            _signatureService.RegisterHandler<RaceDataService, StageEventArgs>(nameof(IRaceDataService.StageDeleted), (refresher, args) => refresher.ClearSignature(args.Stage.GetUdi()));

        }

        private static Guid ParseEntityId(string nodeId) => Guid.Parse(nodeId);

        private static Guid ParseNodeIdFromPrefixedValue(string nodeId, string prefix) => Guid.Parse(nodeId.Substring(prefix.Length));

        private void StageServiceOnSaved(object sender, StageEventArgs e)
        {
            var artifact = GetArtifact(e);

            if (!_transferRaceDataAsContent)
            {
                
                _diskEntityService.WriteArtifacts(new[] { artifact });
            }

            _signatureService.SetSignature(artifact);
        }

        private IArtifact GetArtifact(StageEventArgs e)
        {
            var udi = e.Stage.GetUdi();
            return _serviceConnectorFactory.GetConnector(udi.EntityType).GetArtifact(e.Stage);
        }

        public void Terminate()
        {
        }
    }
}
