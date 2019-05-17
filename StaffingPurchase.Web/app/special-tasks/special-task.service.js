(function () {
    'use strict';
    angular
    .module('spApp.specialTasks')
    .factory('specialTaskService', ['$log', 'dataService', specialTaskService]);

    function specialTaskService($log, dataService) {
        var apiEndpoint = '/api/it';

        var updateOrderAreaBasedOnPackageLog = function () {
            return dataService.put(apiEndpoint + '/update/order-area');
        };

        return {
            updateOrderAreaBasedOnPackageLog: updateOrderAreaBasedOnPackageLog
        };
    }
})();
