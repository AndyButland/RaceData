using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Scoping;

namespace RaceData.CustomData
{
    public class RaceDataService : IRaceDataService
    {
        private readonly IScopeProvider _scopeProvider;

        public RaceDataService(IScopeProvider scopeProvider)
        {
            _scopeProvider = scopeProvider;
        }

        public event EventHandler<RiderEventArgs> RiderSaved;

        public event EventHandler<RiderEventArgs> RiderDeleted;

        public event EventHandler<StageEventArgs> StageSaved;

        public event EventHandler<StageEventArgs> StageDeleted;

        public event EventHandler<TeamEventArgs> TeamSaved;

        public event EventHandler<TeamEventArgs> TeamDeleted;

        protected void DispatchStageEvent(IScope scope, EventHandler<StageEventArgs> eventHandler, Stage item, string name) =>
            scope.Events.Dispatch(eventHandler, this, new StageEventArgs(item), $"{typeof(Stage).Name}{name}");

        protected void DispatchTeamEvent(IScope scope, EventHandler<TeamEventArgs> eventHandler, Team item, string name) =>
            scope.Events.Dispatch(eventHandler, this, new TeamEventArgs(item), $"{typeof(Team).Name}{name}");

        protected void DispatchRiderEvent(IScope scope, EventHandler<RiderEventArgs> eventHandler, Rider item, string name) =>
            scope.Events.Dispatch(eventHandler, this, new RiderEventArgs(item), $"{typeof(Rider).Name}{name}");

        public IReadOnlyCollection<Rider> GetRiders()
        {
            using (var scope = _scopeProvider.CreateScope(autoComplete: true))
            {
                var dtos = scope.Database.Fetch<RiderDto>().OrderBy(x => x.Name);
                return dtos.Select(BuildEntity).ToList().AsReadOnly();
            }
        }

        public IReadOnlyCollection<Rider> GetRidersForTeam(Guid teamId)
        {
            using (var scope = _scopeProvider.CreateScope(autoComplete: true))
            {
                var dtos = scope.Database.Fetch<RiderDto>().Where(x => x.TeamId == teamId).OrderBy(x => x.Name);
                return dtos.Select(BuildEntity).ToList().AsReadOnly();
            }
        }

        public Rider GetRiderById(Guid id)
        {
            using (var scope = _scopeProvider.CreateScope(autoComplete: true))
            {
                var dto = scope.Database.Fetch<RiderDto>().SingleOrDefault(x => x.Id == id);
                if (dto == null)
                {
                    return null;
                }

                return BuildEntity(dto);
            }
        }

        public void AddRider(Rider artist)
        {
            var dto = BuildDto(artist);
            using (var scope = _scopeProvider.CreateScope())
            {
                scope.Database.Insert(dto);
                scope.Complete();
            }
        }

        public void UpdateRider(Rider artist)
        {
            var dto = BuildDto(artist);
            using (var scope = _scopeProvider.CreateScope())
            {
                scope.Database.Update(dto);
                scope.Complete();
            }
        }

        private Rider BuildEntity(RiderDto dto) => new Rider
        {
            Id = dto.Id,
            Name = dto.Name,
            Team = GetTeamById(dto.TeamId),
        };

        private RiderDto BuildDto(Rider rider) => new RiderDto
        {
            Id = rider.Id,
            Name = rider.Name,
            TeamId = rider.Team.Id,
        };

        public IReadOnlyCollection<Stage> GetStages()
        {
            using (var scope = _scopeProvider.CreateScope(autoComplete: true))
            {
                var dtos = scope.Database.Fetch<StageDto>().OrderBy(x => x.Name);
                return dtos.Select(BuildEntity).ToList().AsReadOnly();
            }
        }

        public Stage GetStageById(Guid id)
        {
            using (var scope = _scopeProvider.CreateScope(autoComplete: true))
            {
                var dto = scope.Database.Fetch<StageDto>().SingleOrDefault(x => x.Id == id);
                if (dto == null)
                {
                    return null;
                }

                return BuildEntity(dto);
            }
        }

        public void AddStage(Stage stage)
        {
            var dto = BuildDto(stage);
            using (var scope = _scopeProvider.CreateScope())
            {
                scope.Database.Insert(dto);

                DispatchStageEvent(scope, StageSaved, stage, "Saved");

                scope.Complete();
            }
        }

        public void UpdateStage(Stage stage)
        {
            var dto = BuildDto(stage);
            using (var scope = _scopeProvider.CreateScope())
            {
                scope.Database.Update(dto);

                DispatchStageEvent(scope, StageSaved, stage, "Saved");

                scope.Complete();
            }
        }

        private Stage BuildEntity(StageDto dto) => new Stage
        {
            Id = dto.Id,
            Name = dto.Name,
            From = dto.From,
            To = dto.To,
            Distance = dto.Distance
        };

        private StageDto BuildDto(Stage stage) => new StageDto
        {
            Id = stage.Id,
            Name = stage.Name,
            From = stage.From,
            To = stage.To,
            Distance = stage.Distance
        };

        public IReadOnlyCollection<Team> GetTeams()
        {
            using (var scope = _scopeProvider.CreateScope(autoComplete: true))
            {
                var dtos = scope.Database.Fetch<TeamDto>().OrderBy(x => x.Name);
                return dtos.Select(BuildEntity).ToList().AsReadOnly();
            }
        }

        public Team GetTeamById(Guid id)
        {
            using (var scope = _scopeProvider.CreateScope(autoComplete: true))
            {
                var dto = scope.Database.Fetch<TeamDto>().SingleOrDefault(x => x.Id == id);
                if (dto == null)
                {
                    return null;
                }

                return BuildEntity(dto);
            }
        }

        public void AddTeam(Team team)
        {
            var dto = BuildDto(team);
            using (var scope = _scopeProvider.CreateScope())
            {
                scope.Database.Insert(dto);

                DispatchTeamEvent(scope, TeamSaved, team, "Saved");

                scope.Complete();
            }
        }

        public void UpdateTeam(Team team)
        {
            var dto = BuildDto(team);
            using (var scope = _scopeProvider.CreateScope())
            {
                scope.Database.Update(dto);

                DispatchTeamEvent(scope, TeamSaved, team, "Saved");

                scope.Complete();
            }
        }

        private Team BuildEntity(TeamDto dto) => new Team
        {
            Id = dto.Id,
            Name = dto.Name,
        };

        private TeamDto BuildDto(Team label) => new TeamDto
        {
            Id = label.Id,
            Name = label.Name,
        };
    }
}
