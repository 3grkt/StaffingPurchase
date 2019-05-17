(function () {
    'use strict';

    angular
        .module('spApp.levelgroups.levelgroup')
        .factory('levelService', ['$log', 'dataService', levelService]);

    function levelService($log, dataService) {
        var create = function (level) {
            return dataService.post('api/level/post', level);
        };

        var update = function (level) {
            return dataService.put('api/level/put', level);
        };

        var getAll = function () {
            return dataService.get('/api/level');
        };

        var updateAll = function (levels) {
            return dataService.post('/api/level/saveall', levels);
        };

        var get = function (id) {
            return dataService.get('/api/level/' + id);
        };

        var remove = function (id) {
            return dataService.remove('/api/level/' + id);
        };

        return {
            create: create,
            update: update,
            getAll: getAll,
            updateAll : updateAll,
            get: get,
            remove: remove
        };
    }
})();
