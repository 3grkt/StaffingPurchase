(function () {
    "use strict";

    angular
        .module("spApp.users.user")
        .factory("userService", ["$httpParamSerializer", "dataService", userService]);

    function userService($httpParamSerializer, dataService) {
        var baseApi = "api/user";
        var create = function (user) {
            return dataService.post("api/user/post", user);
        };

        var update = function (user) {
            return dataService.put("api/user/put", user);
        };

        var search = function (paginationOptions, filter) {
            if (typeof filter === "undefined") {
                filter = {
                    departmentId: null,
                    locationId: null,
                    userName: null,
                    roleId: null
                };
            }

            var api = baseApi + "?" + $httpParamSerializer(filter);
            return dataService.search(api, paginationOptions);
        };

        var get = function (id) {
            return dataService.get("/api/user/" + id);
        };

        var resetPwd = function (userId) {
            return dataService.post("api/user/reset-pwd", userId);
        };

        var getUsersByDepartment = function (locationId, departmentId) {
            if (typeof (locationId) === "undefined" || locationId < 0) locationId = null;
            if (typeof (departmentId) === "undefined" || departmentId < 0) departmentId = null;
            return dataService.get("api/user/get-by-department/" + locationId + "/" + departmentId);
        };

        var remove = function (id) {
            return dataService.remove("/api/user/" + id);
        };

        var searchByUserName = function (userName, pageSize) {
            pageSize = pageSize || 20;
            var filter = { userName: userName };
            return dataService.search(
                baseApi + '/search-user?' + $httpParamSerializer(filter),
                { pageIndex: 1, pageSize: pageSize });
        };

        return {
            create: create,
            update: update,
            search: search,
            searchByUserName: searchByUserName,
            get: get,
            getUsersByDepartment: getUsersByDepartment,
            remove: remove,
            resetPwd: resetPwd
        };
    }
})();
