(function () {
    'use strict';

    angular
        .module('spApp')
        // Config constants
        // TODO: find better way to sync server's data to client instead of using angular constants
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
            cash: 0,
            pv: 1
        })
        // Config route
        .config(['$routeProvider', function ($routeProvider) {
            $routeProvider
                .when('/home', {
                    templateUrl: 'app/home/home.html',
                    controller: 'homeCtrl',
                    controllerAs: 'vm'
                })
                .when('/order', {
                    templateUrl: 'app/orders/order.html',
                    controller: 'orderCtrl',
                    controllerAs: 'vm'
                })
                .when('/order-approval', {
                    templateUrl: 'app/orders/order-approval.html',
                    controller: 'orderApprovalCtrl',
                    controllerAs: 'vm',
                    reloadOnSearch: false
                })
                .when('/order-history', {
                    templateUrl: 'app/orders/order-history.html',
                    controller: 'orderHistoryCtrl',
                    controllerAs: 'vm',
                    reloadOnSearch: false
                })
                .when('/order-update/:id?', {
                    templateUrl: 'app/orders/order-update.html',
                    controller: 'orderUpdateCtrl',
                    controllerAs: 'vm'
                })
                .when('/order-view/:id?', {
                    templateUrl: '/app/orders/order-view.html',
                    controller: 'orderViewCtrl',
                    controllerAs: 'vm'
                })
                .when('/order-warehouse', {
                    templateUrl: 'app/orders/order-warehouse.html',
                    controller: 'orderWarehouseCtrl',
                    controllerAs: 'vm'
                })
                .when('/order/admin-report/internal-requisition-form',{
                    templateUrl: 'app/report/internal-requisition-form.html',
                    controller: 'internalResitionFormCtrl',
                    controllerAs: 'vm'
                })
                .when('/order/admin-report/summary-discount-product',{
                    templateUrl: 'app/report/summary-discount-product.html',
                    controller: 'summaryDiscountProductCtrl',
                    controllerAs: 'vm'
                })
                .when('/order/admin-report/order-by-individual-pv',{
                    templateUrl: 'app/report/order-by-individual-pv.html',
                    controller: 'orderByIndividualPVCtrl',
                    controllerAs: 'vm'
                })
                .when('/order/admin-report/order-by-individual-discount',{
                    templateUrl: 'app/report/order-by-individual-discount.html',
                    controller: 'orderByIndividualDiscountCtrl',
                    controllerAs: 'vm'
                })
                .when('/order-warehouse-report', {
                    templateUrl: 'app/orders/order-warehouse-report.html',
                    controller: 'orderWarehouseReportCtrl',
                    controllerAs: 'vm'
                })
                .when('/user', {
                    templateUrl: 'app/users/user.html',
                    controller: 'userCtrl',
                    controllerAs: 'vm'
                })
                .when('/user-change-password', {
                    templateUrl: 'app/users/user-change-password.html',
                    controller: 'userChangePwdCtrl',
                    controllerAs: 'vm'
                })
                .when('/levelgroup', {
                    templateUrl: 'app/levelgroups/levelgroup.html',
                    controller: 'levelGroupCtrl',
                    controllerAs: 'vm'
                })
                .when('/product-list', {
                    templateUrl: 'app/products/product-list.html',
                    controller: 'productListCtrl',
                    controllerAs: 'vm',
                    reloadOnSearch: false
                })
                .when('/product-detail/:id?', {
                    templateUrl: 'app/products/product-detail.html',
                    controller: 'productDetailCtrl',
                    controllerAs: 'vm'
                })
                .when('/upload-product', {
                    templateUrl: 'app/products/product-upload.html',
                    controller: 'productUploadCtrl',
                    controllerAs: 'vm'
                })
                .when('/create-award', {
                    templateUrl: 'app/awards/award-list.html',
                    controller: 'awardListCtrl',
                    controllerAs: 'vm'
                })
                .when('/upload-award', {
                    templateUrl: 'app/awards/award-upload.html',
                    controller: 'awardUploadCtrl',
                    controllerAs: 'vm'
                })
                .when('/policy', {
                    templateUrl: 'app/policies/policy.html',
                    controller: 'policyCtrl',
                    controllerAs: 'vm'
                })
                .when('/pv-log', {
                    templateUrl: 'app/pvlog/pvlog.html',
                    controller: 'pvLogCtrl',
                    controllerAs: 'vm'
                })
                .when('/batchjob-log', {
                    templateUrl: 'app/batchjobs/batchjob-log.html',
                    controller: 'batchJobLogCtrl',
                    controllerAs: 'vm'
                })
                .when('/_batchjob-test', {
                    templateUrl: 'app/batchjobs/batchjob.html',
                    controller: 'batchJobCtrl',
                    controllerAs: 'vm'
                })
                .when('/_special-task/order-task', {
                    templateUrl: 'app/special-tasks/order-special-task.html',
                    controller: 'orderTaskCtrl',
                    controllerAs: 'vm'
                })
                .otherwise({
                    redirectTo: '/home'
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
