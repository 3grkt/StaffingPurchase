(function () {
    'use strict';

    angular
        .module('spApp.awards.service', [])
        .factory('awardService', ['$httpParamSerializer', 'dataService', awardService]);

    function awardService($httpParamSerializer, dataService) {
        var getAllAwards = function () {
            return dataService.get('/api/award?nopaging=true');
        };

        var search = function (filter, paginationOptions) {
            return dataService.search('/api/award' + '?' + $httpParamSerializer(filter), paginationOptions);
        };

        var get = function (id) {
            return dataService.get('/api/award?id=' + id);
        };

        var create = function (award) {
            return dataService.post('/api/award/post', award);
        };

        var update = function (award) {
            return dataService.put('/api/award/put', award);
        };

        var remove = function (id) {
            return dataService.remove('/api/award/' + id);
        };

        var downloadTemplate = function (id) {
            return dataService.download('/api/award/download-template', '', 'post');
        };

        return {
            getAllAwards: getAllAwards,
            search: search,
            get: get,
            create: create,
            update: update,
            remove: remove,
            downloadTemplate: downloadTemplate
        };
    }
})();
