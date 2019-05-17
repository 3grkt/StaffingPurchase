(function () {
    'use strict';

    angular
        .module('spApp.pvLog.service', [])
        .factory('pvLogService', ['$httpParamSerializer', 'dataService', pvLogService]);

    function pvLogService($httpParamSerializer, dataService) {
        var getAll = function () {
            return dataService.get('/api/pvlog/getall');
        };

        var search = function (filter, paginationOptions) {
            return dataService.search('/api/pvlog/search?' + $httpParamSerializer(filter), paginationOptions); 
        };

        var searchLogSummary = function(filter, paginationOptions) {
            return dataService.search('/api/pvlog/logsummary?' + $httpParamSerializer(filter), paginationOptions);
        };

        return {
            getAll: getAll,
            search: search,
            searchLogSummary: searchLogSummary
        };
    }
})();
