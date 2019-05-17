(function () {
    'use strict';

    angular
        .module('spApp.pvLog.controller', [])
        .controller('pvLogCtrl', ['$window', 'Page', 'commonService', 'gridService', 'pvLogService', 'userService', pvLogCtrl]);

    function pvLogCtrl($window, Page, commonService, gridService, pvLogService, userService) {
        var self = this;

        Page.setTitle('PageTitle.ViewPvLog');

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

        self.availableUsers = [];

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
            pvLogService.searchLogSummary(self.filter, self.paginationOptions).then(function (response) {
                self.searched = true;

                self.gridOptions.data = response.Data;
                self.gridOptions.totalItems = response.TotalItems;
            });
        };

        self.searchPvLog = function () {
            self.paginationOptions.pageIndex = 1;
            self.gridOptions.paginationCurrentPage = 1;
            self.bindData(self.paginationOptions);
        };

        self.fetchUsers = function (query) {
            if (query && query.length) {
                userService.searchByUserName(query)
                    .then(function (response) {
                        self.availableUsers = response.Data;
                    });
            }
        };

        self.hasData = function () {
            return self.gridOptions.totalItems > 0;
        };

        self.getDisplayUserName = function (user) {
            return user ? (user.UserName + ' (' + user.FullName + ')') : '';
        };

        // grid
        self.paginationOptions = angular.copy(gridService.getPaginationOptions());
        self.gridOptions = gridService.getDefaultGridOptions(self, self.bindData, self.paginationOptions);
        self.gridOptions.columnDefs = [
            { field: 'UserName', displayName: 'User.UserName', headerCellFilter: 'translate' },
            { field: 'Month', displayName: 'PVLog.Month', headerCellFilter: 'translate' },
            { field: 'PreviousSessionPv', displayName: 'PVLog.PreviousSessionPV', headerCellFilter: 'translate', cellClass: 'cell-number', cellFilter: 'number:2' },
            { field: 'MonthlyRewardedPv', displayName: 'PVLog.MonthlyRewardedPV', headerCellFilter: 'translate', cellClass: 'cell-number', cellFilter: 'number:2' },
            { field: 'AwardedPv', displayName: 'PVLog.AwardedPV', headerCellFilter: 'translate', cellClass: 'cell-number', cellFilter: 'number:2' },
            { field: 'BirthdayRewardedPv', displayName: 'PVLog.BirthdayRewardedPV', headerCellFilter: 'translate', cellClass: 'cell-number', cellFilter: 'number:2' },
            { field: 'OrderingPv', displayName: 'PVLog.OrderingPV', headerCellFilter: 'translate', cellClass: 'cell-number', cellFilter: 'number:2' },
            { field: 'RemainingPv', displayName: 'PVLog.RemainingPV', headerCellFilter: 'translate', cellClass: 'cell-number', cellFilter: 'number:2' }
        ];
        self.gridOptions.enableSorting = false; // disable sort
        // extend common api to allow auto-height in column header
        var commonApi = self.gridOptions.onRegisterApi;
        self.gridOptions.onRegisterApi = function (gridApi) {
            commonApi(gridApi);
            gridApi.core.on.rowsRendered(null, function (api) {
                var columnHeaderHeight = parseInt($window.getComputedStyle(api.grid.element.find('.ui-grid-header-cell-row')[0]).height) || 30;
                api.grid.options.customHeaderHeight = columnHeaderHeight;
            });
        };

        self.init = function () {
            self.setDates();
        };

        self.init();
    }
})();
