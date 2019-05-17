(function () {
    'use strict';

    angular
        .module('spApp.orders.order')
        .factory('orderBatchService', ['$httpParamSerializer', '$filter', 'dataService', orderBatchService]);

    function orderBatchService($httpParamSerializer, $filter, dataService) {
        var baseApi = '/api/orderbatch';

        var internalSearch = function (api, filter, paginationOptions) {
            api = api + '?' + $httpParamSerializer(filter);
            return dataService.search(api, paginationOptions);
        };

        var searchBatches = function (filter, paginationOptions) {
            return internalSearch(baseApi, filter, paginationOptions);
        };

        var searchAllBatches = function (filter, paginationOptions) {
            return internalSearch(baseApi + '/getall', filter, paginationOptions);
        };

        var getDate = function () {
            return dataService.get('api/orderbatch/get-batch-date');
        };

        var approve = function (batchId) {
            var data = { type: 'approve', id: batchId };
            return dataService.put('api/orderbatch', data);
        };

        var approveAll = function (orderType) {
            var data = { type: 'approve', orderType: orderType };
            return dataService.put('api/orderbatch/putall', data);
        };

        var reject = function (batchId, rejectReason) {
            var data = { type: 'reject', id: batchId, reason: rejectReason };
            return dataService.put('api/orderbatch', data);
        };

        var rejectAll = function (orderType, rejectReason) {
            var data = { type: 'reject', orderType: orderType, reason: rejectReason };
            return dataService.put('api/orderbatch/putall', data);
        };

        var formatSessionDate = function (startDate, endDate) {
            return $filter('customDate')(startDate) + ' - ' + $filter('customDate')(endDate);
        };


        return {
            searchBatches: searchBatches,
            searchAllBatches: searchAllBatches,
            approve: approve,
            approveAll: approveAll,
            reject: reject,
            rejectAll: rejectAll,
            getDate: getDate,
            formatSessionDate: formatSessionDate
        };
    }
})();
