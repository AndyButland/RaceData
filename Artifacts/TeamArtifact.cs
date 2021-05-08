
using System.Collections.Generic;
using Umbraco.Core;
using Umbraco.Core.Deploy;
using Umbraco.Deploy.Artifacts;

namespace RaceData.CustomData
{
    public class TeamArtifact : DeployArtifactBase<GuidUdi>
    {
        public TeamArtifact(GuidUdi udi, IEnumerable<ArtifactDependency> dependencies = null)
            : base(udi, dependencies)
        { }
    }
}
