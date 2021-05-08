using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Deploy.UI;

namespace RaceData.CustomData
{
    [ComposeBefore(typeof(DeployUiComposer))]
    public class RaceDataComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.RegisterUnique<IRaceDataService, RaceDataService>();

            composition.Components().Append<RaceDataDeployComponent>();
        }
    }
}
