(function () {
    'use strict';

    angular
       .module('spApp.orders.history')
       .factory('orderHistoryService', ['$httpParamSerializer', 'dataService', orderHistoryService]);

    function orderHistoryService($httpParamSerializer, dataService) {
        var baseApi = 'api/orderhistory';
        var getAll = function (paginationOptions) {
            return dataService.search('/api/orderhistory/gets', paginationOptions);
        };

        var search = function (pagingOptions, filter) {
            var api = baseApi + '?' + $httpParamSerializer(filter);
            return dataService.search(api, pagingOptions);
        };

        var getOrderDetails = function (orderId) {
            return dataService.get('api/orderhistory/get/order/details/' + orderId);
        };

        var getOrderView = function(orderId) {
            return dataService.get('api/orderhistory/get/order/view/' + orderId);
        };
        return {
            getAll: getAll,
            search: search,
            getOrderDetails: getOrderDetails,
            getOrderView: getOrderView
        };
    }
})();
