
using System;
using System.Collections.Generic;
using Umbraco.Core;
using Umbraco.Core.Deploy;
using Umbraco.Deploy.Artifacts;

namespace RaceData.CustomData
{
    public class RiderArtifact : DeployArtifactBase<GuidUdi>
    {
        public RiderArtifact(GuidUdi udi, IEnumerable<ArtifactDependency> dependencies = null)
            : base(udi, dependencies)
        { }

        public Guid TeamId { get; set; }
    }
}
