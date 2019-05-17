(function () {
    "use strict";

    angular
        .module("spApp.users.user-profile")
        .factory("userProfileService", ["$log", "dataService", userProfileService]);

    function userProfileService($log, dataService) {
        var baseApi = "api/user-profile";
        
        function changePassword(password) {
            return dataService.post("api/user-profile/change-password", password);
        }

        return {
            changePassword: changePassword
        };
    }
})();