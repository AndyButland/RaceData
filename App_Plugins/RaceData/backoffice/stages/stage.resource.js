function stageResource($http, umbRequestHelper) {
    return {
        getById: function (id) {
            return umbRequestHelper.resourcePromise(
                $http.get("/umbraco/backoffice/RaceData/RaceDataEditor/GetStage?id=" + id),
                    "Failed to get stage with id " + id);
        },

        save: function (stage) {
            if (!stage)
                throw "'stage' parameter cannot be null";

            return umbRequestHelper.resourcePromise(
                $http.post("/umbraco/backoffice/RaceData/RaceDataEditor/SaveStage", stage),
                "Failed to save stage with id " + stage.id);
        }
    };
}

angular.module('umbraco.resources').factory('stageResource', stageResource);
