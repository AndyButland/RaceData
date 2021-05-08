using System;
using System.Collections.Generic;

namespace RaceData.CustomData
{
    public interface IRaceDataService
    {
        event EventHandler<RiderEventArgs> RiderSaved;

        event EventHandler<RiderEventArgs> RiderDeleted;

        event EventHandler<StageEventArgs> StageSaved;

        event EventHandler<StageEventArgs> StageDeleted;

        event EventHandler<TeamEventArgs> TeamSaved;

        event EventHandler<TeamEventArgs> TeamDeleted;

        IReadOnlyCollection<Rider> GetRiders();

        IReadOnlyCollection<Rider> GetRidersForTeam(Guid teamId);

        Rider GetRiderById(Guid id);

        void AddRider(Rider rider);

        void UpdateRider(Rider rider);

        IReadOnlyCollection<Stage> GetStages();

        Stage GetStageById(Guid id);

        void AddStage(Stage stage);

        void UpdateStage(Stage stage);

        IReadOnlyCollection<Team> GetTeams();

        Team GetTeamById(Guid id);

        void AddTeam(Team team);

        void UpdateTeam(Team team);
    }
}
