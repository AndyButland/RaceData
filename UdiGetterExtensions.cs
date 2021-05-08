using System;
using Umbraco.Core;

namespace RaceData.CustomData
{
    public static class UdiGetterExtensions
    {
        public static GuidUdi GetUdi(this Rider entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            return new GuidUdi(AppConstants.UdiEntityTypes.Rider, entity.Id).EnsureClosed();
        }

        public static GuidUdi GetUdi(this Stage entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            return new GuidUdi(AppConstants.UdiEntityTypes.Stage, entity.Id).EnsureClosed();
        }

        public static GuidUdi GetUdi(this Team entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            return new GuidUdi(AppConstants.UdiEntityTypes.Team, entity.Id).EnsureClosed();
        }
    }
}
