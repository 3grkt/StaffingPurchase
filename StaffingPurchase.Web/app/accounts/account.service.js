(function () {
    'use strict';

    angular
        .module('spApp.accounts.service', [])
        .factory('accountService', ['$httpParamSerializer', 'dataService', accountService]);

    function accountService($httpParamSerializer, dataService) {
        var login = function (username, password) {
            var loginData = {
                grant_type: 'password',
                username: username,
                password: password
            };
            var headers = {
                'Content-Type' : 'application/x-www-form-urlencoded'
            };
            return dataService.post('/token', $httpParamSerializer(loginData), { headers: headers });
        };
        
        var logout = function () {
            return dataService.post('/api/account/logout');
        };

        return {
            login: login,
            logout: logout
        };
    }
})();
