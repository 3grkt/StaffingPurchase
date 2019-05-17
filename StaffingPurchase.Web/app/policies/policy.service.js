(function () {
    'use strict';

    angular
        .module('spApp.policies.service', [])
        .factory('policyService', ['dataService', policyService]);

    function policyService(dataService) {
        var getAll = function () {
            return dataService.get('/api/configuration/getall');
        };

        var getHighValueProductPrice = function () {
            return dataService.get('/api/configuration/gethighvalueproductprice');
        };

        var update = function (data) {
            return dataService.put('/api/configuration/put', data);
        };

        var downloadFile = function () {
            return dataService.download('/api/configuration/download-policy-file', '', 'post');
        };

        return {
            getAll: getAll,
            getHighValueProductPrice: getHighValueProductPrice,
            update: update,
            downloadFile: downloadFile
        };
    }
})();
