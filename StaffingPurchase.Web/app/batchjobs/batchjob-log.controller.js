(function () {
    'use strict';

    angular
        .module('spApp.batchJobs.log', [])
        .controller('batchJobLogCtrl', ['$uibModal', 'Page', 'commonService', 'gridService', 'batchJobService', batchJobLogCtrl]);

    function batchJobLogCtrl($uibModal, Page, commonService, gridService, batchJobService) {
        var self = this;

        Page.setTitle('PageTitle.ViewBatchJobLog');

        /*
         * Variables
         */
        self.alert = {};

        self.datePopup = {
            startOpened: false,
            endOpened: false
        };

        self.filter = {
            startDate: null,
            endDate: null
        };

        /*
         * Methods
         */
        self.openDatePopup = function (key) {
            self.datePopup[key] = true;
        };

        self.setDates = function () {
            var d = new Date();
            d.setMonth(d.getMonth() - 1); // last month
            self.filter.startDate = d;
            self.filter.endDate = new Date();
        };

        self.bindData = function () {
            batchJobService.search(self.filter, self.paginationOptions).then(function (response) {
                self.searched = true;

                self.gridOptions.data = response.Data;
                self.gridOptions.totalItems = response.TotalItems;
            });
        };

        self.searchBatchJobLog = function () {
            self.paginationOptions.pageIndex = 1;
            self.gridOptions.paginationCurrentPage = 1;
            self.bindData(self.paginationOptions);
        };

        self.viewLog = function (grid, row) {
            batchJobService.get(row.entity.ErrorId).then(function (data) {
                var modalInstance = $uibModal.open({
                    templateUrl: "app/batchjobs/batchjob-log-detail.html",
                    controller: "batchJobLogDetailCtrl",
                    controllerAs: "vm",
                    backdrop: "static",
                    size: 'lg',
                    resolve: {
                        log: data
                    }
                });
            });
        };

        // grid
        self.paginationOptions = angular.copy(gridService.getPaginationOptions());
        self.gridOptions = gridService.getDefaultGridOptions(self, self.bindData, self.paginationOptions);
        self.gridOptions.columnDefs = [
            { field: 'Message', displayName: 'BatchJob.Message', headerCellFilter: 'translate' },
            { field: 'Time', displayName: 'BatchJob.Time', headerCellFilter: 'translate', cellFilter: 'customDateTime', width: 200 },
            {
                field: 'ErrorId', name: '', width: '5%',
                cellTemplate: '<div class="text-center">' +
                '<a href="javascript:void(0);" ng-click="grid.appScope.vm.viewLog(grid, row)"><i class="fa fa-eye"></i></a>' +
                '</div>'
            }
        ];

        self.init = function () {
            self.setDates();
        };

        self.init();
    }
})();
