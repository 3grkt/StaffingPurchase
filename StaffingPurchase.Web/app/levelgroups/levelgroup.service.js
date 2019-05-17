(function () {
    'use strict';

    angular
        .module('spApp.levelgroups.levelgroup')
        .factory('levelGroupService', ['$log', 'dataService', levelGroupService]);

    function levelGroupService($log, dataService) {
        var create = function (levelGroup) {
            return dataService.post('api/levelgroup/post', levelGroup);
        };

        var update = function (levelGroup) {
            return dataService.put('api/levelgroup/put', levelGroup);
        };

        var updateAll = function (levelGroups) {
            return dataService.post('api/levelgroup/saveall', levelGroups);
        };

        var getAll = function () {
            return dataService.get('/api/levelgroup');
        };

        var get = function (id) {
            return dataService.get('/api/levelgroup/' + id);
        };

        var remove = function (id) {
            return dataService.remove('/api/levelgroup/' + id);
        };

        return {
            create: create,
            update: update,
            getAll: getAll,
            updateAll: updateAll,
            get: get,
            remove: remove
        };
    }
})();
