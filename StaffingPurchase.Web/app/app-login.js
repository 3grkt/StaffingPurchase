(function () {
    'use strict';

    angular
        .module('spApp-login', [
            'ngRoute',
            'ngCookies',

            'pascalprecht.translate', // support using text resources
            'ui.bootstrap',

            'spApp.page',
            'spApp.pageService',
            'spApp.shared.dataService',
            'spApp.shared.commonService',
            'spApp.shared.alertService',
            'spApp.shared.gridService',
            'spApp.shared.confirmModal',
            'spApp.shared.convertToNumber',
            'spApp.shared.filters',

            'spApp.accounts.login',
            'spApp.accounts.service'
        ])
        .constant('Permissions', {
            viewPolicy: 1,
            submitOrder: 2,
            viewOrderHistory: 3,
            updateOrder: 4,
            approveOrder: 5,
            packOrder: 6,
            createAward: 7,
            uploadAwardList: 8,
            maintainEmployeeList: 9,
            maintainProductList: 10,
            maintainLevelGroup: 11,
            maintainPolicy: 12,
            maintainUser: 13,
            reportEmployeeOrders: 14,
            reportPackagedOrders: 15,
            reportAllOrders: 16,
            resetUserPassword: 17,
            viewBatchjobLog: 18,
            rejectOrder: 19,
            viewPvLog: 20,
            viewUserPvLog: 21
        })
        .constant('Roles', {
            employee: 1,
            warehouse: 2,
            hrAdmin: 3,
            hrManager: 4,
            it: 5
        })
        .constant('OrderType', {
            cast: 0,
            pv: 1
        })
        // Config route
        .config(['$routeProvider', function ($routeProvider) {
            $routeProvider
                .when('/', {
                    templateUrl: 'app/accounts/login.html',
                    controller: 'loginCtrl',
                    controllerAs: 'vm'
                })
                .otherwise({
                    redirectTo: '/'
                });
        }])
        // Config resource
        .config(['$translateProvider', function ($translateProvider) {
            $translateProvider.useUrlLoader('/api/textresource');
            $translateProvider.useLoaderCache(true);
            $translateProvider.preferredLanguage('en'); // just use 'en' to by pass, language is set in backend 
            $translateProvider.useSanitizeValueStrategy('escape'); // Enable escaping of HTML
        }]);
})();
