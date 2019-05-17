(function () {
    'use strict';

    angular
        .module('spApp.orders.order', [])
        .constant('discount', 0.21)
        .controller('orderCtrl', ['$uibModal', 'Page', 'alertService', 'commonService', 'productService', 'gridService', 'orderService', 'userService', 'discount', 'Notification', '$translate', orderCtrl])
        .filter('propsFilter', function () {
            return function (items, props) {
                var out = [];

                if (angular.isArray(items)) {
                    var keys = Object.keys(props);

                    items.forEach(function (item) {
                        var itemMatches = false;

                        for (var i = 0; i < keys.length; i++) {
                            var prop = keys[i];
                            var text = props[prop].toLowerCase();
                            if (item[prop].toString().toLowerCase().indexOf(text) !== -1) {
                                itemMatches = true;
                                break;
                            }
                        }

                        if (itemMatches) {
                            out.push(item);
                        }
                    });
                } else {
                    // Let the output be the input untouched
                    out = items;
                }

                return out;
            };
        }).filter('discountFilter', ['discount', function (discount) {
            return function (row, isEligible) {
                return isEligible ? row * (1 - discount) : row;
                // return row * (1 - discount);
            };
        }]);

    function orderCtrl($uibModal, Page, alertService, commonService, productService, gridService, orderService, userService, discount, Notification, $translate) {
        Page.setTitle('Menu.PlaceOrder');
        var self = this;
        self.alertAddOrderDetail = alertService.getDefaultAlertOptions();
        self.alertOrderDetailGrid = alertService.getDefaultAlertOptions();
        self.discount = discount;
        self.orderTotal = 0;
        self.orderPv = 0;
        self.currentUserPV = 0;
        self.remainPV = 0;

        self.products = [];
        self.allProductCategories = [];
        self.selectProduct = {};
        self.selectProductCategory = null;
        self.productNumChoices = [1, 2, 3];
        self.selectedNum = self.productNumChoices[0];

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

        self.openAlertModel = function (alertMessage, callback) {
            var modalInstance = $uibModal.open({
                templateUrl: 'app/shared/controllers/alert-modal.html',
                controller: 'alertModalCtrl',
                controllerAs: 'vm',
                size: 'sm',
                backdrop: 'static',
                resolve: {
                    data: { title: 'Modal.Alert', message: alertMessage }
                }
            });

            modalInstance.result.then(function (result) {
                $log.info(result);
            }, function () {
                if(callback){
                    callback();
                }
                //$log.info('Modal dismissed at: ' + new Date());
            });
        };

        self.getCurrentUserPV = function () {
            commonService.getCurrentUserPV().then(function (data) {
                self.currentUserPV = data;
            });
        };

        self.getAllProducts = function (categoryId) {
            if (typeof categoryId == 'undefined') {
                self.products = [];
                return;
            }

            productService.getByCategory(categoryId).then(function (data) {
                self.products = data.Data;
                if (self.products.length > 0) {
                    self.selectProduct = self.products[0];
                }
            });
        };

        self.getAllProductCategories = function () {
            commonService.getAllProductCategories().then(function (data) {
                self.allProductCategories = data;
            });
        };

        self.onProductCategoryChanges = function () {
            if (typeof self.selectedProductCategory != 'undefined') {
                self.getAllProducts(self.selectedProductCategory);
            }
        };

        self.getUserOrderDetails = function () {
            self.orderDetails.length = 0;
            self.orderTotal = 0;
            self.orderPv = 0;
            self.remainPV = 0;
            orderService.getOrderDetails(self.isPriceMode).then(function (data) {
                if (data.Data == null) {
                    self.orderDetails.length = 0;
                } else {
                    self.orderDetails = data.Data;
                }

                for (var orderDetail in data.Data) {
                    self.orderTotal += data.Data[orderDetail].Price * data.Data[orderDetail].Volume;
                    self.orderPv += data.Data[orderDetail].PV * data.Data[orderDetail].Volume;
                }

                if (!self.isPriceMode) {
                    commonService.getCurrentUserPV().then(function (data) {
                        self.currentUserPV = data;
                        self.remainPV = (self.currentUserPV - self.orderPv).toFixed(2);
                    });
                }

                self.orderTotal *= (1 - discount);
                self.orderPv = self.orderPv.toFixed(2);
                self.gridOptionsOrderDetail.data = self.orderDetails;
            });
        };

        self.removeSelectedOrderDetails = function () {
            var selectedOrderDetails = [];
            for (var i = 0; i < self.gridOptionsOrderDetail.data.length; i++) {
                if (self.gridOptionsOrderDetail.data[i].Selected == true) {
                    selectedOrderDetails.push(self.gridOptionsOrderDetail.data[i]);
                }
            }

            if (selectedOrderDetails.length === 0) {
                self.openAlertModel('Order.NotSelectOrderDetailToRemove');
                return;
            }

            self.openConfirmModal(function () {
                orderService.deleteOrderDetails(selectedOrderDetails).then(function (data) {
                    if (!data.success) {
                        Notification.error(data.errorMessage);
                    }
                    else {
                        Notification.success('OrderDetail.DeleteSuccess');
                    }
                    self.getUserOrderDetails();
                });
            }, "Order.Confirm.DeleteOrderDetail");
        };

        self.addOrderDetail = function () {
            if (!self.selectProduct.hasOwnProperty('Id')) {
                self.alertAddOrderDetail.activeAlert('OrderDetail.Validation.UnselectProduct', 'warning');
                return;
            }

            //if ((self.remainPV - (self.selectProduct.PV * self.selectedNum)) < 0 && !self.isPriceMode) {
            //    self.alertAddOrderDetail.activeAlert('OrderDetail.Validation.NotEnoughPV', 'warning');
            //    return;
            //}

            var orderDetail = {
                ProductId: self.selectProduct.Id,
                Volume: self.selectedNum,
                IsPrice: self.isPriceMode
            };

            orderService.addOrderDetail(orderDetail).then(function (data) {
                if (!data.success) {
                    Notification.warning(data.errorMessage);
                }
                else {
                    Notification.success('OrderDetail.AddSuccess');
                }
                self.getUserOrderDetails();
            });
        };

        self.cancelOrder = function () {
            self.openConfirmModal(function () {
                orderService.deleteOrder(self.isPriceMode).then(function (data) {
                    if (!data.success) {
                        Notification.warning(data.errorMessage);
                    }
                    else {
                        Notification.success('Order.CancelSuccess');
                    }

                    self.getUserOrderDetails();
                });
            }, "Order.Confirm.DeleteOrder");
        };

        self.isLookedOrder = false;
        self.getLookedOrderStatus = function (callback) {
            orderService.checkLookedOrder().then(function (data) {
                self.isLookedOrder = data;
                if (self.isLookedOrder == false) {
                    self.getAllProductCategories();
                    self.getUserOrderDetails();
                }
                else {
                    commonService.getCurrentUserPV().then(function (data) {
                        self.currentUserPV = data;
                    });
                }

                if (typeof (callback) === typeof (Function)) {
                    callback();
                }
            });
        };

        self.isEligibleUser = false;
        self.isPriceMode = true;
        self.orderDetails = [];
        self.toPvMode = function () {
            self.isPriceMode = false;
            self.gridOptionsOrderDetail.columnDefs[5].visible = true;
            self.gridOptionsOrderDetail.columnDefs[4].visible = false;
            self.gridApi.core.refresh();
            self.getUserOrderDetails(self.isPriceMode);
            self.resetSelectAllToggle();
        };

        self.toPriceMode = function () {
            self.isPriceMode = true;
            self.gridOptionsOrderDetail.columnDefs[5].visible = false;
            self.gridOptionsOrderDetail.columnDefs[4].visible = true;
            self.gridApi.core.refresh();
            self.getUserOrderDetails(self.isPriceMode);
            self.resetSelectAllToggle();
        };

        self.currentToggle = false;
        self.toggleSelectAll = function () {
            for (var selectIndex in self.orderDetails) {
                self.orderDetails[selectIndex].Selected = self.currentToggle;
            }
        };

        self.resetSelectAllToggle = function () {
            self.currentToggle = false;
        };

        self.gridOptionsOrderDetail = gridService.getGridOptionsWithoutSearch(self);
        self.gridOptionsOrderDetail.columnDefs = [
                { field: 'Selected', displayName: '', headerCellFilter: 'translate', headerCellTemplate: '<input type="checkbox" ng-model="grid.appScope.vm.currentToggle" ng-change="grid.appScope.vm.toggleSelectAll()" >', cellTemplate: '<input type="checkbox" ng-model="row.entity.Selected" >', width: '10%' }, // TODO: handle sort for this field
                { field: 'ProductSku', displayName: 'Product.Sku', headerCellFilter: 'translate' },
                { field: 'ProductName', displayName: 'Product.Name', headerCellFilter: 'translate', width: '35%' },
                { field: 'Volume', displayName: 'Product.Quantity', headerCellFilter: 'translate' },
                { field: 'Price', displayName: 'Product.Price', headerCellFilter: 'translate', cellFilter: 'discountFilter:grid.appScope.vm.isEligibleUser | currency:"VND ":0', visible: true },
                { field: 'PV', displayName: 'Product.PV', headerCellFilter: 'translate', visible: false }
        ];
        self.gridOptionsOrderDetail.data = self.orderDetails;

        self.getLookedOrderStatus();
    }
})();
