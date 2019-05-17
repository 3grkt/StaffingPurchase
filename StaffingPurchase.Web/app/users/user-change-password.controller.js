(function() {
    "use strict";

    angular
        .module("spApp.users.user-profile", [])
        .controller("userChangePwdCtrl", ["$uibModal", "$log", "Page", "Notification", "commonService", "alertService", "userProfileService", userChangePwdCtrl]);

    function userChangePwdCtrl($uibModal, $log, Page, Notification, commonService, alertService, userProfileService) {
        var self = this;
        Page.setTitle("Menu.ChangePassword");

        /* Alert */
        self.alertChangePassword = alertService.getDefaultAlertOptions();

        /* Variables */
        self.password = {
            oldPassword: '',
            newPassword: '',
            verifyNewPassword: ''
        };

        self.init = function() {

        };


        self.changePassword = function() {
            userProfileService.changePassword(self.password).then(function (data) {
                if (data.success) {
                    Notification.success("User.ChangePassword.Success");
                } else {
                    Notification.warning(data.errorMessage);
                }
            });
        };
        self.init();
    }

})();