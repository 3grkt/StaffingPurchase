(function () {
    'use strict';

    angular
        .module('spApp.shared.commonService', [])
        .factory('commonService', ['$timeout', '$cookies', '$q', 'dataService', commonService]);

    function commonService($timeout, $cookies, $q, dataService) {
        var self = {};

        self.getAllLocations = function () {
            return dataService.get('/api/common/getAllLocations');
        };

        self.getAllDepartments = function () {
            return dataService.get('/api/common/getAllDepartments');
        };

        self.getAllProductCategories = function () {
            return dataService.get('/api/common/getAllProductCategories');
        };

        self.getAllRoles = function () {
            return dataService.get('/api/common/getAllRoles');
        };

        self.getOrderBatchDate = function () {
            return dataService.get('/api/common/getOrderBatchDate');
        };

        self.getNearestOrderBatchDate = function () {
            return dataService.get('/api/common/getNearestOrderBatchDate');
        };

        self.getAllOrderBatchDates = function () {
            return dataService.get('/api/common/getAllOrderBatchDates');
        };

        self.getAllOrderStatus = function () {
            return dataService.get('/api/common/getAllOrderStatus');
        };

        self.transferDictionary = function (data) {
            var objectArray = [];
            for (var i in data) {
                objectArray.push({ Key: i, Value: data[i] });
            }

            return objectArray;
        };

        self.getUserPermissions = function () {
            try {
                return JSON.parse(sessionStorage.getItem('_userPermissions'));
            } catch (ex) {
                return [];
            }
        };

        self.setUserLanguageCookie = function (locale) {
            $cookies.put('userlanguage', locale);
        };

        self.setUserData = function (data) {
            sessionStorage.setItem('_userPermissions', data.permissions);
            self.setUserLanguageCookie(data.userlanguage);
        };

        self.clearUserData = function (data) { // don't clear language $cookies
            sessionStorage.removeItem('_userPermissions');
        };

        self.alertTimeout = null;
        self.setAlert = function (alertObj, msg, type, duration) {
            duration = duration || 5000; // default 5 seconds

            alertObj.msg = msg;
            alertObj.type = type;
            alertObj.show = !!msg;

            // Hide alert after 5 seconds
            if (!self.alertTimeout) {
                $timeout.cancel(self.alertTimeout);
            }

            self.alertTimeout = $timeout(function () {
                alertObj.show = false;
            }, duration);
        };

        self.hasPermission = function (permissionId) {
            var userPermisions = self.getUserPermissions();
            return userPermisions && userPermisions.indexOf(permissionId) >= 0;
        };

        self.getCurrentUserPV = function () {
            return dataService.get('/api/common/getCurrentUserPV');
        };

        self.setUserLanguage = function (locale) {
            var deferred = $q.defer();
            dataService.post('/api/user/update-language?locale=' + locale)
                .then(function () {
                    self.setUserLanguageCookie(locale);
                    deferred.resolve({ success: true });
                }, function () {
                    deferred.resolve({ success: false });
                });
            return deferred.promise;
        };

        return {
            getAllLocations: self.getAllLocations,
            getAllDepartments: self.getAllDepartments,
            getAllProductCategories: self.getAllProductCategories,
            getAllRoles: self.getAllRoles,
            transferDictionary: self.transferDictionary,
            getUserPermissions: self.getUserPermissions,
            setUserData: self.setUserData,
            clearUserData: self.clearUserData,
            setAlert: self.setAlert,
            hasPermission: self.hasPermission,
            getOrderBatchDate: self.getOrderBatchDate,
            getNearestOrderBatchDate: self.getNearestOrderBatchDate,
            getAllOrderBatchDates: self.getAllOrderBatchDates,
            getCurrentUserPV: self.getCurrentUserPV,
            getAllOrderStatus: self.getAllOrderStatus,
            setUserLanguage: self.setUserLanguage
        };
    }
})();
