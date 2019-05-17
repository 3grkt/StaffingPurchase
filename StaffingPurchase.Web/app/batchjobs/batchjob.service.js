(function () {
    'use strict';

    angular
        .module('spApp.batchJobs.service', [])
        .factory('batchJobService', ['dataService', batchJobService]);

    function batchJobService(dataService) {

        var search = function (filter, paginationOptions) {
            return dataService.search('/api/batchjob/getlogs' +
                '?startDate=' + (filter.startDate ? filter.startDate.toJSON() : '') + 
                '&endDate=' + (filter.endDate ? filter.endDate.toJSON() : ''),
                paginationOptions);
        };

        var get = function(id){
            return dataService.get('/api/batchjob/getlog/' + id);
        };

        return {
            search: search,
            get: get
        };
    }
})();
