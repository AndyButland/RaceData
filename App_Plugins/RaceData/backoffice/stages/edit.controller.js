(function () {
    "use strict";

    function StageEditController($scope, $routeParams, stageResource, formHelper, notificationsService, editorState, pluginEntityService) {

        var vm = this;

        vm.page = {};
        vm.showBackButton = true;
        vm.stage = {};
        vm.breadcrumbs = [];
        vm.save = save;
        vm.goToPage = goToPage;

        vm.page.defaultButton = {
            alias: "save",
            hotKey: "ctrl+s",
            hotKeyWhenHidden: true,
            labelKey: "buttons_save",
            letter: "S",
            handler: function () { vm.save(); }
        };
        vm.page.subButtons = [];

        function init() {

            vm.loading = true;

            if (!$routeParams.create) {

                stageResource.getById($routeParams.id).then(function (stage) {

                    vm.stage = stage;

                    // Augment with additional details necessary for identifying the node for a deployment.
                    // TODO: can we move this into plugin?
                    vm.stage.metaData = { treeAlias: $routeParams.tree };

                    editorState.set(vm.stage);

                    vm.page.name = vm.stage.name;
                    $scope.$emit("$changeTitle", "Edit Stage: " + vm.stage.name);

                    makeBreadcrumbs();

                    vm.loading = false;
                });
            }

            pluginEntityService.addInstantDeployButton(vm.page.subButtons);
        }

        function save() {

            if (formHelper.submitForm({ scope: $scope })) {
                vm.page.buttonGroupState = "busy";
                saveStage();
            }
        }

        function saveStage() {
            stageResource.save(vm.stage).then(function (stage) {

                formHelper.resetForm({ scope: $scope });
                vm.page.buttonGroupState = "success";
                notificationsService.success("Stage saved");

            }, function (err) {
                vm.page.buttonGroupState = "error";
                formHelper.resetForm({ scope: $scope, hasErrors: true });
                formHelper.handleError(err);

            });
        }

        function goToPage(ancestor) {
            $location.path(ancestor.path);
        }

        function makeBreadcrumbs() {
            vm.breadcrumbs = [
                {
                    "name": "Stages",
                },
                {
                    "name": vm.stage.name
                }
            ];
        }

        init();

    }

    angular.module("umbraco").controller("RaceData.StageEditController", StageEditController);

})();
