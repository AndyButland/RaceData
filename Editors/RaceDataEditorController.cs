using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;

namespace RaceData.CustomData
{
    [PluginController(AppConstants.PluginName)]
    public class RaceDataEditorController : UmbracoAuthorizedJsonController
    {
        private readonly IRaceDataService _raceDataService;

        public RaceDataEditorController(IRaceDataService raceDataService)
        {
            _raceDataService = raceDataService;
        }

        [HttpGet]
        public Rider GetRider(Guid id)
        {
            var rider = _raceDataService.GetRiderById(id);
            if (rider == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return rider;
        }

        [HttpPost]
        public HttpResponseMessage SaveRider(Rider rider)
        {
            if (rider.Id == Guid.Empty)
            {
                rider.Id = Guid.NewGuid();
                _raceDataService.AddRider(rider);
            }
            else
            {
                _raceDataService.UpdateRider(rider);
            }
            
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet]
        public Stage GetStage(Guid id)
        {
            var stage = _raceDataService.GetStageById(id);
            if (stage == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return stage;
        }

        [HttpPost]
        public HttpResponseMessage SaveStage(Stage stage)
        {
            if (stage.Id == Guid.Empty)
            {
                stage.Id = Guid.NewGuid();
                _raceDataService.AddStage(stage);
            }
            else
            {
                _raceDataService.UpdateStage(stage);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet]
        public Team GetTeam(Guid id)
        {
            var team = _raceDataService.GetTeamById(id);
            if (team == null)
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.NotFound));
            }

            return team;
        }

        [HttpPost]
        public HttpResponseMessage SaveTeam(Team team)
        {
            if (team.Id == Guid.Empty)
            {
                team.Id = Guid.NewGuid();
                _raceDataService.AddTeam(team);
            }
            else
            {
                _raceDataService.UpdateTeam(team);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
