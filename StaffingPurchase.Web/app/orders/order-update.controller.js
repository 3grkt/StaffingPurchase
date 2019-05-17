(function () {
    'use strict';

    angular
    .module('spApp.orders.update', [])
    .controller('orderUpdateCtrl', ['$window', '$routeParams', '$uibModal','Notification', 'Page', 'productService', 'gridService', 'orderUpdateService', 'userService', 'commonService', 'policyService', orderUpdateCtrl]);

    function orderUpdateCtrl($window, $routeParams, $uibModal, Notification, Page, productService, gridService, orderUpdateService, userService, commonService, policyService) {
        var self = this;
        Page.setTitle('Menu.OrderUpdate');
        self.enableFilters = true;
        self.batchEndDate = "";
        self.batchStartDate = "";
        self.maxPrice; // default: 5 millions
        // Holding selected options
        self.selectedDepartment = {};
        self.selectedLocation = {};
        self.selectedUser = {};
        self.currentOrder = {};

        // Holding order metadata in "not filtered" mode
        self.metadata = {
            user: '',
            department: '',
            location: '',
            orderSession: ''
        };

        // ui-select datasource
        self.departments = [];
        self.locations = [];
        self.orderUsers = [];
        self.orderUserIds = [];
        self.backupOrderDetails = [];

        // Mode switcher
        self.hasPVOrder = false;
        self.hasPriceOrder = false;
        self.selectedType = 0;

        // Back-up when rollback editing session
        self.priceDetailBackup = {};
        self.pvDetailBackup = {};

        self.isEdiable = false;
        self.trackedOverPriceDetails = [];

        self.openEditDetailModal = function (orderDetail) {
            var editingOrderDetail = orderDetail;
            var modalInstance = $uibModal.open({
                templateUrl: 'app/orders/order-update-edit.html',
                controller: 'orderUpdateEditCtrl',
                controllerAs: 'vm',
                backdrop: 'static',
                resolve: {
                    orderDetail: orderDetail,
                    isPrice: self.selectedType === 0 ? true : false
                }
            });

            modalInstance.result.then(function (orderDetail) {
                if (orderDetail.Price >= self.maxPrice && orderDetail.Volume > editingOrderDetail.Volume) {
                    self.trackedOverPriceDetails.push({ OrderDetailId: editingOrderDetail.OrderDetailId, Volume: editingOrderDetail.Volume });
                }

                editingOrderDetail.Volume = orderDetail.Volume;

                // self.gridApi.core.refresh();
            });
        };

        self.openConfirmModal = function (confirmCallback, confirmMessage) {
            var modalInstance = $uibModal.open({
                templateUrl: 'app/shared/controllers/confirm-modal.html',
                controller: 'confirmModalCtrl',
                controllerAs: 'vm',
                size: 'sm',
                backdrop: 'static',
                resolve: {
                    data: { title: 'Modal.Confirm', message: confirmMessage }
                }
            });

            modalInstance.result.then(function (result) {
                if (result.confirmed && confirmCallback) {
                    confirmCallback();
                }
            }, function () {
                //$log.info('Modal dismissed at: ' + new Date());
            });
        };

        self.getNearestOrderBatchDate = function () {
            commonService.getNearestOrderBatchDate().then(function (data) {
                self.batchEndDate = data.EndDate;
                self.batchStartDate = data.StartDate;
            });
        };

        self.getAllDepartments = function () {
            commonService.getAllDepartments().then(function (data) {
                self.departments = commonService.transferDictionary(data);
                self.departments.unshift({ Key: -1, Value: "Tất cả" });
            });
        };

        self.getAllLocations = function () {
            commonService.getAllLocations().then(function (data) {
                self.locations = commonService.transferDictionary(data);
                self.locations.unshift({ Key: -1, Value: "Tất cả" });
            });
        };

        self.getOrderUsers = function () {
            self.selectedLocation = self.selectedLocation || {};
            self.selectedDepartment = self.selectedDepartment || {};
            userService
                .getUsersByDepartment(self.selectedLocation.Key, self.selectedDepartment.Key)
                .then(function (data) {
                    self.orderUsers = data.Data;
                    if (data.TotalItems > 0) {
                        self.selectedUser = self.orderUsers[0];
                    } else {
                        self.selectedUser = {};
                    }
                });
        };

        self.disableEditForm = function () {
            self.gridOptionsOrderDetail.data = [];
            self.hasPriceOrder = false;
            self.hasPVOrder = false;
            self.isEditable = false;
        };

        self.searchHandler = function (data) {
            if (data.Data.length < 1) {
                Notification.warning('OrderUpdate.OrderNotFound');
            }

            // clear the edit-form before using new searched order-detail data
            self.disableEditForm();

            self.currentOrder = data.Data;

            if (self.currentOrder.length > 0) {
                var firstOrder = self.currentOrder[0];
                self.metadata.user = firstOrder.User;
                self.metadata.department = firstOrder.Department;
                self.metadata.location = firstOrder.Location;
                self.metadata.orderSession = firstOrder.OrderSession;
            }

            for (var i in self.currentOrder) {
                if (self.currentOrder[i].Type === 0) {
                    self.priceDetailBackup = angular.copy(self.currentOrder[i].OrderDetails);
                    self.hasPriceOrder = true;

                    if (self.selectedType === self.currentOrder[i].Type || !self.hasPVOrder) {
                        self.toPriceMode();
                        self.isEditable = true;
                    }
                }

                if (self.currentOrder[i].Type === 1) {
                    self.pvDetailBackup = angular.copy(self.currentOrder[i].OrderDetails);
                    self.hasPVOrder = true;

                    if (self.selectedType === self.currentOrder[i].Type || !self.hasPriceOrder) {
                        self.toPVMode();
                        self.isEditable = true;
                    }
                }
            }

            // self.gridApi.core.refresh();
        };

        self.searchOrderDetails = function () {
            if (!self.enableFilters) {
                orderUpdateService.getOrders(0, $routeParams.id).then(self.searchHandler);
            }
            else if (typeof (self.selectedUser.UserId) != 'undefined') {
                orderUpdateService.getOrders(self.selectedUser.UserId).then(self.searchHandler);
            }
        };

        self.toPriceMode = function () {
            self.toMode(0);
            self.gridOptionsOrderDetail.columnDefs[4].visible = true;
            self.gridOptionsOrderDetail.columnDefs[5].visible = false;
        };

        self.toPVMode = function () {
            self.toMode(1);
            self.gridOptionsOrderDetail.columnDefs[4].visible = false;
            self.gridOptionsOrderDetail.columnDefs[5].visible = true;
        };

        self.toMode = function (typeId) {
            for (var i in self.currentOrder) {
                if (self.currentOrder[i].Type === typeId) {
                    self.gridOptionsOrderDetail.data = self.currentOrder[i].OrderDetails;
                    self.selectedType = typeId;
                }
            }
        };

        self.cancelChanges = function () {
            var currentOrder = self.getCurrentModeOrder();

            if (self.selectedType === 0) {
                currentOrder.OrderDetails = angular.copy(self.priceDetailBackup);
                self.gridOptionsOrderDetail.data = currentOrder.OrderDetails;
            }

            if (self.selectedType === 1) {
                currentOrder.OrderDetails = angular.copy(self.pvDetailBackup);
                self.gridOptionsOrderDetail.data = currentOrder.OrderDetails;
            }
        };

        self.deleteOrderDetail = function (grid, row) {
            var currentOrderDetails = self.getCurrentModeOrder().OrderDetails;
            var removeIndex = -1;
            for (var i = 0; i < currentOrderDetails.length; i++) {
                if (currentOrderDetails[i].OrderDetailId === row.entity.OrderDetailId) {
                    removeIndex = i;
                    break;
                }
            }

            if (removeIndex !== -1) {
                currentOrderDetails.splice(removeIndex, 1);
            }
        };

        self.saveChanges = function () {
            self.openConfirmModal(function () {
                orderUpdateService.
                    saveOrder(self.getCurrentModeOrder())
                    .then(function (data) {
                        if (!data.success) {
                            Notification.warning(data.errorMessage);
                            self.revertOverPriceDetailVolume();
                            self.trackedOverPriceDetails = [];
                        }
                        else {
                            Notification.success('OrderUpdate.SaveSuccess');
                            self.searchOrderDetails();
                            //self.backupCurrentModeOrder();
                        }
                    });
            }, "OrderUpdate.Confirm.SaveChange");
        };

        self.revertOverPriceDetailVolume = function () {
            var currentOrder = self.getCurrentModeOrder();
            for (var i = 0; i < self.trackedOverPriceDetails.length; i++) {
                for (var j = 0; j < currentOrder.OrderDetails.length; j++) {
                    if (currentOrder.OrderDetails[j].OrderDetailId === self.trackedOverPriceDetails[i].OrderDetailId) {
                        currentOrder.OrderDetails[j].Volume = self.trackedOverPriceDetails[i].Volume;
                    }
                }
            }
        };

        self.backupCurrentModeOrder = function () {
            for (var i = 0; i < self.currentOrder.length; i++) {
                if (self.currentOrder[i].Type === self.selectedType) {
                    if (self.selectedType === 0) {
                        self.priceDetailBackup = angular.copy(self.currentOrder[i]);
                    }

                    if (self.selectedType === 1) {
                        self.pvDetailBackup = angular.copy(self.currentOrder[i]);
                    }
                }
            }
        };

        self.getCurrentModeOrder = function () {
            for (var i = 0; i < self.currentOrder.length; i++) {
                if (self.currentOrder[i].Type === self.selectedType) {
                    return self.currentOrder[i];
                }
            }

            return null;
        };

        self.close = function () {
            $window.history.back();
        };

        self.updateOrderDetail = function (grid, row) {
            self.openEditDetailModal(row.entity);
        };

        self.gridOptionsOrder = gridService.getGridOptionsWithoutSearch(self);
        self.gridOptionsOrder.columnDefs = [
        { field: 'OrderNum', displayName: 'STT', headerCellFilter: 'translate', cellTemplate: '<div class="ui-grid-cell-contents">{{grid.renderContainers.body.visibleRowCache.indexOf(row) + 1}}</div>', width: '10%' },
        { field: 'ProductSku', displayName: 'Product.Sku', headerCellFilter: 'translate' },
        { field: 'ProductName', displayName: 'Product.Name', headerCellFilter: 'translate', width: '40%' },
        { field: 'Volume', displayName: 'Product.Quantity', headerCellFilter: 'translate' },
        { field: 'Price', displayName: 'Product.Price', headerCellFilter: 'translate', cellFilter: 'discountFilter:grid.appScope.vm.isEligibleUser | currency:"VND ":0', visible: true },
        { field: 'PV', displayName: 'Product.PV', headerCellFilter: 'translate', visible: false },
            {
                field: 'Change', displayName: '', cellTemplate: '<div class="text-center">' +
                        '<a href="javascript:void(0);" ng-click="grid.appScope.vm.updateOrderDetail(grid, row)">Sửa</a>' +
                        ' / ' +
                        '<a href="javascript:void(0);" ng-click="grid.appScope.vm.deleteOrderDetail(grid, row)">Xóa</i></a>' +
                        '</div>'
            }
        ];

        self.gridOptionsOrderDetail = gridService.getGridOptionsWithoutSearch(self, { disableScrollbar: true });
        self.gridOptionsOrderDetail.columnDefs = [
        { field: 'OrderNum', displayName: 'STT', headerCellFilter: 'translate', cellTemplate: '<div class="ui-grid-cell-contents">{{grid.renderContainers.body.visibleRowCache.indexOf(row) + 1}}</div>', width: '10%' },
        { field: 'ProductSku', displayName: 'Product.Sku', headerCellFilter: 'translate' },
        { field: 'ProductName', displayName: 'Product.Name', headerCellFilter: 'translate', width: '40%' },
        { field: 'Volume', displayName: 'Product.Quantity', headerCellFilter: 'translate' },
        { field: 'Price', displayName: 'Product.Price', headerCellFilter: 'translate', cellFilter: 'discountFilter:grid.appScope.vm.isEligibleUser | currency:"VND ":0', visible: true },
        { field: 'PV', displayName: 'Product.PV', headerCellFilter: 'translate', visible: false },
            {
                field: 'Change', displayName: '', cellTemplate: '<div class="text-center">' +
                                   '<a href="javascript:void(0);" ng-click="grid.appScope.vm.updateOrderDetail(grid, row)">Sửa</a>' +
                                   ' / ' +
                                   '<a href="javascript:void(0);" ng-click="grid.appScope.vm.deleteOrderDetail(grid, row)">Xóa</i></a>' +
                                   '</div>'
            }
        ];

        // Get max price
        policyService.getHighValueProductPrice().then(function (price) {
            self.maxPrice = price;
        });

        // If id is provided in route, search it
        if ($routeParams.id) {
            self.enableFilters = false;
            self.searchOrderDetails();
        } else {
            self.getAllDepartments();
            self.getAllLocations();
            self.getNearestOrderBatchDate();
        }
    }
})();
