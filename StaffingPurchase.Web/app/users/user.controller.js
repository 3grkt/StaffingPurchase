(function() {
    "use strict";

    angular
        .module("spApp.users.user", [])
        .controller("userCtrl", ["$uibModal", "$log", 'Notification', "Page", "gridService", "commonService", "alertService", "userService", userCtrl]);

    function userCtrl($uibModal, $log, Notification, Page, gridService, commonService, alertService, userService) {
        var self = this;
        Page.setTitle("Menu.User");

        /* Alert */
        self.alertUserUpdating = alertService.getDefaultAlertOptions();

        /*
         * Variables
         */
        self.searched = false;
        self.users = [];
        self.allLocations = [];
        self.allDepartments = [];
        self.allRoles = [];
        self.filter = {
            DepartmentId: null,
            LocationId: null,
            RoleId: null,
            UserName: ""
        };

        /*
         * Methods
         */
        self.openModal = function(user) {
            var modalInstance = $uibModal.open({
                templateUrl: "app/users/user-creation.html",
                controller: "userCreationCtrl",
                controllerAs: "vm",
                backdrop: "static",
                resolve: {
                    user: user
                }
            });

            modalInstance.result.then(function(data) {
                //self.alertUserUpdating.activeAlert("User.Update.Success", "success");
                Notification.success('User.Update.Success');
                self.searchUsers();
            }, function() {
                //$log.info('Modal dismissed at: ' + new Date());
            });
        };

        self.openConfirmModal = function(confirmCallback, msg) {
            var modalInstance = $uibModal.open({
                templateUrl: "app/shared/controllers/confirm-modal.html",
                controller: "confirmModalCtrl",
                controllerAs: "vm",
                size: "sm",
                backdrop: "static",
                resolve: {
                    data: { title: "Modal.Confirm", message: msg }
                }
            });

            modalInstance.result.then(function(result) {
                if (result.confirmed && confirmCallback) {
                    confirmCallback();
                }
            }, function() {
                //$log.info('Modal dismissed at: ' + new Date());
            });
        };

        self.searchUsers = function(paginationOptions) {
            userService.search(paginationOptions || self.paginationOptions, self.filter)
                .then(function(data) {
                    self.searched = true;

                    self.gridOptions.data = data.Data;
                    self.gridOptions.totalItems = data.TotalItems;
                });
        };

        self.editUser = function(grid, row) {
            $log.log(row);
            userService.get(row.entity.Id)
                .then(function(data) {
                    self.openModal(data);
                });
        };

        self.deleteUser = function(grid, row) {
            self.openConfirmModal(function() {
                userService.remove(row.entity.Id)
                    .then(function(data) {
                        if (!data.success) {
                            //self.alertUserUpdating.activeAlert(data.errorMessage, "warning");
                            Notification.warning(data.errorMessage);
                        } else {
                            //self.alertUserUpdating.activeAlert("User.Delete.Success", "success");
                            Notification.success('User.Delete.Success');
                            self.searchUsers();
                        }
                    });
            }, "User.ConfirmDelete");
        };

        self.resetUserPwd = function(grid, row) {
            self.openConfirmModal(function() {
                userService.resetPwd(row.entity.Id)
                    .then(function(data) {
                        if (!data.success) {
                            //self.alertUserUpdating.activeAlert(data.errorMessage, "warning");
                            Notification.warning(data.errorMessage);
                        } else {
                            //self.alertUserUpdating.activeAlert("User.ResetPwd.Success", "success");
                            Notification.success('User.ResetPwd.Success');
                        }
                    });
            }, "User.ConfirmResetPwd");
        };

        // grid
        self.paginationOptions = angular.copy(gridService.getPaginationOptions());
        self.gridOptions = gridService.getDefaultGridOptions(self, self.searchUsers, self.paginationOptions);
        self.gridOptions.columnDefs = [
            { field: "UserName", displayName: "User.Name", headerCellFilter: "translate" },
            { field: "FullName", displayName: "User.FullName", headerCellFilter: "translate" },
            { field: "RoleName", displayName: "User.Role", headerCellFilter: "translate" },
            { field: "Location.Name", displayName: "User.Location", headerCellFilter: "translate" },
            { field: "Department.Name", displayName: "User.Department", headerCellFilter: "translate" },
            {
                field: "Id",
                name: "",
                width: "300",
                cellTemplate: '<div class="text-center">' +
                    '<a href="javascript:void(0);" ng-click="grid.appScope.vm.editUser(grid, row)" translate>User.Edit</a>' + " / " +
                    '<a href="javascript:void(0);" ng-click="grid.appScope.vm.deleteUser(grid, row)" translate>User.Delete</a>' + " / " +
                    '<a href="javascript: void(0);" ng-click="grid.appScope.vm.resetUserPwd(grid, row)" translate>User.ResetPwd</a>' +
                    "</div>"
            }
        ];

        self.init = function() {
            commonService.getAllRoles().then(function(data) {
                self.allRoles = data;
            });

            commonService.getAllLocations().then(function(data) {
                self.allLocations = data;
                //self.allLocations.unshift({ null: "Tất cả" });
            });

            commonService.getAllDepartments().then(function(data) {
                self.allDepartments = data;
                //self.allDepartments.unshift({ null: "Tất cả" });
            });
        };

        self.init();
    }
})();
