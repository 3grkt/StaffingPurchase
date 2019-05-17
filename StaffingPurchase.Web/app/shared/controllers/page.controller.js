(function () {
    'use strict';

    angular
        .module('spApp.page', [])
        .controller('pageCtrl', ['$scope', 'Permissions', 'Roles', 'OrderType', 'Page', 'commonService', 'gridService', 'accountService', pageCtrl]);

    function pageCtrl($scope, Permissions, Roles, OrderType, Page, commonService, gridService, accountService) {
        $scope.Page = Page;

        // expose to views
        $scope.OrderType = OrderType;
        $scope.Permissions = Permissions;
        $scope.Roles = Roles;

        $scope.userPermissions = commonService.getUserPermissions();

        $scope.logout = function () {
            commonService.clearUserData();
            accountService.logout().then(function () {
                location.href = '/login';
            });
        };

        $scope.setLanguage = function (locale) {
            commonService.setUserLanguage(locale).then(function (result) {
                if (result.success) {
                    location.reload(true);
                }
            });
        };

        $scope.hasPermission = function (permissionId) {
            return commonService.hasPermission(permissionId);
        };

        $scope.getGridHeight = function (gridOptions, ignorePagination) {
            return gridService.getGridHeight(gridOptions, ignorePagination);
        };
    }
})();
