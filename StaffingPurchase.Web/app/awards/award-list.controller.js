(function () {
    'use strict';

    angular
        .module('spApp.awards.list', [])
        .controller('awardListCtrl', ['$timeout', '$uibModal', 'Permissions', 'Page', 'commonService', 'gridService', 'awardService', awardListCtrl]);

    function awardListCtrl($timeout, $uibModal, Permissions, Page, commonService, gridService, awardService) {
        var self = this;

        Page.setTitle('PageTitle.CreateAward');

        /**
         * Variables
         */
        self.alert = {
            type: '',
            msg: '',
            show: false
        };

        self.filter = {
            name: '',
            pv: ''
        };

        /**
         * Methods
         */
        self.searchAwards = function () {
            awardService.search(self.filter, self.paginationOptions).then(function (response) {
                self.searched = true;

                self.gridOptions.data = response.data;
                self.gridOptions.totalItems = response.totalItems;
            });
        };

        self.openCreationModal = function (award) {
            var modalInstance = $uibModal.open({
                templateUrl: 'app/awards/award-creation.html',
                controller: 'awardCreationCtrl',
                controllerAs: 'vm',
                backdrop: 'static',
                resolve: {
                    award: award
                }
            });

            modalInstance.result.then(function (award) {
                self.searchAwards();
                commonService.setAlert(self.alert, 'Award.SuccessToSave', 'success');
            });
        };

        self.editAward = function (grid, row) {
            awardService.get(row.entity.id)
                .then(function (data) {
                    self.openCreationModal(data);
                });
        };

        self.deleteAward = function (grid, row) {
            Page.openConfirmModal({
                message: 'Award.ConfirmDelete',
                confirmCallback: function () {
                    awardService.remove(row.entity.id).then(function (data) {
                        self.searchAwards();
                        commonService.setAlert(self.alert, 'Award.SuccessToDelete', 'success');
                    });
                }
            });
        };

        // grid
        self.paginationOptions = angular.copy(gridService.getPaginationOptions());
        self.gridOptions = gridService.getDefaultGridOptions(self, self.searchAwards, self.paginationOptions);
        self.gridOptions.columnDefs = [
            { field: 'name', displayName: 'Award.Name', headerCellFilter: 'translate' },
            { field: 'pv', displayName: 'Award.PV', headerCellFilter: 'translate' },
            {
                field: 'id', name: '', width: 60,
                cellTemplate: '<div class="text-center">' +
                                '<a href="javascript:void(0);" ng-click="grid.appScope.vm.editAward(grid, row)"><i class="fa fa-edit"></i></a>' + '&nbsp;' +
                                '<a href="javascript:void(0);" ng-click="grid.appScope.vm.deleteAward(grid, row)"><i class="fa fa-remove"></i></a>' +
                                '</div>'
            }
        ];

        self.init = function () {

        };

        self.init();
    }
})();
