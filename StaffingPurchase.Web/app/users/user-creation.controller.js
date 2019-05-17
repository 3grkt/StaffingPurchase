(function () {
    'use strict';

    angular
        .module('spApp.users.user') // this share same module as "user.controller.js"
        .controller('userCreationCtrl', ['$log', '$uibModalInstance', 'userService', 'commonService', 'user', userCreationCtrl]);

    function userCreationCtrl($log, $uibModalInstance, userService, commonService, user) {
        var self = this;

        /*
         * Variables
         */

        self.user = {
            UserName: '',
            RoleId: '0',
            FullName: '',
            LocationId: '0'
        };

        self.allRoles = [];
        self.allLocations = [];
        self.allDepartments = [];

        /*
         * Methods
         */
        self.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };

        self.onSaveCompleted = function (result) {
            if (result.success) {
                self.errorMessage = '';
                $uibModalInstance.close(result); // Pass status to its parent
            } else {
                self.errorMessage = result.errorMessage;
            }
        };

        self.save = function () {
            // Create if no user passed to this; otherwise, update user
            if (user) {
                userService.update(self.user)
                    .then(function (result) {
                        self.onSaveCompleted(result);
                    });

            } else {

                userService.create(self.user)
                    .then(function (result) {
                        self.onSaveCompleted(result);
                    });
            }
        };

        self.init = function () {
            if (user) {
                self.user = user;
            }

            commonService.getAllRoles().then(function (data) {
                self.allRoles = data;
            });

            commonService.getAllLocations().then(function (data) {
                self.allLocations = data;
            });

            commonService.getAllDepartments().then(function (data) {
                self.allDepartments = data;
            });
        };

        // Init
        self.init();
    }
})();
