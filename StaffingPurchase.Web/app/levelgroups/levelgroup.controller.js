(function () {
    'use strict';

    angular
        .module('spApp.levelgroups.levelgroup', [])
        .controller('levelGroupCtrl', ['$uibModal', '$log', 'Notification', 'Page', 'gridService', 'alertService', 'levelGroupService', 'levelService', levelGroupCtrl])
    .filter('levelGroupFilter', function () {
        return function (id, levelGroups) {
            for (var i = 0; i < levelGroups.length; i++) {
                if (levelGroups[i].Id == id) {
                    return levelGroups[i].Name;
                }
            }
        };
    });

    function levelGroupCtrl($uibModal, $log, Notification, Page, gridService, alertService, levelGroupService, levelService) {
        var self = this;
        Page.setTitle('Menu.LevelGroup');

        // Add-Level-Group section's alert
        self.alertAddLevelGroup = alertService.getDefaultAlertOptions();
        // LevelGroup-grid section's alert
        self.alertGridLevelGroup = alertService.getDefaultAlertOptions();
        // Level-grid section's alert
        self.alertLevelGroup = alertService.getDefaultAlertOptions();

        self.levelGroup = {
            Name: '',
            PV: 0
        };

        self.levelGroups = [];
        self.levelGroupsBk = [];

        self.levels = [];
        self.levelsBk = [];

        /*
         * Methods
         */

        self.openConfirmModal = function (confirmCallback, confirmMessage) {
            var modalInstance = $uibModal.open({
                templateUrl: 'app/shared/controllers/confirm-modal.html',
                controller: 'confirmModalCtrl',
                controllerAs: 'vm',
                size: 'sm',
                backdrop: 'static',
                resolve: {
                    data: { title: 'Modal.Confirm', message: confirmMessage }
                }
            });

            modalInstance.result.then(function (result) {
                if (result.confirmed && confirmCallback) {
                    confirmCallback();
                }
            }, function () {
            });
        };

        // LevelGroup
        self.getAllLevelGroups = function () {
            levelGroupService.getAll().then(function (data) {
                self.levelGroups = data.Data;
                self.levelGroupsBk = angular.copy(self.levelGroups);
                self.syncLevelGroupGrid();
            });
        };

        self.syncLevelGroupGrid = function () {
            self.gridOptionsLevelGroup.data = self.levelGroups;
            self.gridOptionsLevel.columnDefs[1].editDropdownOptionsArray = self.levelGroups;
        };

        self.saveUpdateLevelGroups = function () {
            self.openConfirmModal(function () {
                levelGroupService.updateAll(self.levelGroups).then(function (data) {
                    Notification.success('Level.UpdateAllSuccess');
                    self.getAllLevelGroups();
                });
            }, 'LevelGroup.ConfirmUpdateAll');
        };

        self.addLevelGroup = function () {
            self.openConfirmModal(function () {
                levelGroupService.create(self.levelGroup).then(function (data) {
                    if (data.success != null && !data.success && data.errorMessage != null) {
                        Notification.warning(data.errorMessage);
                    } else {
                        Notification.success('LevelGroup.AddSuccess');
                        self.getAllLevelGroups();
                    }
                });
            }, 'LevelGroup.ConfirmAdd');
        };

        self.deleteLevelGroup = function (grid, row) {
            self.openConfirmModal(function () {
                levelGroupService.remove(row.entity.Id)
                    .then(function (data) {
                        if (!data.success) {
                            Notification.warning(data.errorMessage);
                        } else {
                            Notification.success('LevelGroup.DeleteSuccess');
                            self.getAllLevelGroups();
                        }
                    });
            }, 'LevelGroup.ConfirmDelete');
        };

        self.cancelUpdateLevelGroups = function () {
            self.openConfirmModal(function () {
                self.levelGroups = angular.copy(self.levelGroupsBk);
                self.syncLevelGroupGrid();
            }, 'LevelGroup.ConfirmCancelUpdateAll');
        };

        // Level
        self.saveUpdateLevels = function () {
            self.openConfirmModal(function () {
                levelService.updateAll(self.levels).then(function (data) {
                    Notification.success('Level.UpdateAllSuccess');
                });
            }, 'LevelGroup.ConfirmUpdateAll');
        };

        self.cancelUpdateLevels = function () {
            self.openConfirmModal(function () {
                self.levels = angular.copy(self.levelsBk);
                self.syncLevelGrid();
            }, 'Level.ConfirmCancelUpdateAll');
        };

        self.syncLevelGrid = function () {
            self.gridOptionsLevel.data = self.levels;
        };

        self.getAllLevels = function () {
            levelService.getAll().then(function (data) {
                self.levels = data.Data;
                self.levelsBk = angular.copy(self.levels);
                self.syncLevelGrid();
            });
        };

        // LevelGroup grid definition
        self.gridOptionsLevelGroup = gridService.getGridOptionsWithoutSearch(self);
        self.gridOptionsLevelGroup.columnDefs = [

            {
                field: 'Name',
                displayName: 'LevelGroup.Name',
                headerCellFilter: 'translate',
                width: "55%"
            },
            {
                field: 'PV',
                displayName: 'LevelGroup.PV',
                headerCellFilter: 'translate',
                cellTemplate: '<input type="number" class="form-control" ng-model="row.entity.PV" />',
                width: "25%"
            },
            {
                field: 'Id',
                displayName: '',
                cellTemplate: '<div class="text-center">' +
                '<a href="javascript:void(0);" ng-click="grid.appScope.vm.deleteLevelGroup(grid, row)"><i class="fa fa-remove"></i></a>' +
                '</div>'
            }
        ];

        // Level grid definition
        self.gridOptionsLevel = gridService.getGridOptionsWithoutSearch(self);
        self.gridOptionsLevel.columnDefs = [

            { field: 'Name', displayName: 'Level.Name', headerCellFilter: 'translate' },
            {
                name: 'GroupId',
                displayName: 'Level.Group',
                headerCellFilter: 'translate',
                editableCellTemplate: 'ui-grid/dropdownEditor',
                editDropdownOptionsArray: self.levelGroups,
                editDropdownValueLabel: 'Name',
                editDropdownIdLabel: 'Id',
                cellFilter: 'levelGroupFilter:grid.appScope.vm.levelGroups',
                width: "50%"
            }
        ];

        self.getAllLevelGroups();
        self.getAllLevels();
    }
})();