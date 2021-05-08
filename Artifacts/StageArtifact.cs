
using System.Collections.Generic;
using Umbraco.Core;
using Umbraco.Core.Deploy;
using Umbraco.Deploy.Artifacts;

namespace RaceData.CustomData
{
    public class StageArtifact : DeployArtifactBase<GuidUdi>
    {
        public StageArtifact(GuidUdi udi, IEnumerable<ArtifactDependency> dependencies = null)
            : base(udi, dependencies)
        { }

        public string From { get; set; }

        public string To { get; set; }

        public int Distance { get; set; }
    }
}
