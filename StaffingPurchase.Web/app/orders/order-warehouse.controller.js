(function () {
    'use strict';

    angular
        .module('spApp.orders.warehouse', [])
        .controller('orderWarehouseCtrl', ['$uibModal', '$translate', 'Notification', 'OrderType', 'Page', 'reportService', 'gridService', 'commonService', 'userService', 'orderBatchService', 'warehouseService', orderWarehouseCtrl]);

    function orderWarehouseCtrl($uibModal, $translate, Notification, OrderType, Page, reportService, gridService, commonService, userService, orderBatchService, warehouseService) {
        var self = this;

        Page.setTitle('PageTitle.OrderWarehouse');
        self.textSelectAll = 'All';
        self.package = {
            DepartmentId: 0,
            ProductDetails: [],
            Type: 0,
            Status: 0,
            TotalProducts: 0
        };

        self.currentLocation = {};
        self.batchEndDate = '';
        self.batchStartDate = '';
        // Holding selected options
        self.selectedDepartment = {
            Value: self.textSelectAll
        };
        self.selectedUser = {};
        self.orderdetails = {};

        // ui-select datasource
        self.departments = [];
        self.locations = [];
        self.orderUsers = [{
            FullName: self.textSelectAll
        }];
        self.backupOrderDetails = [];

        // Check whether Orders are packaged
        self.isPVOrderPackaged = false;
        self.isDiscountOrderPackaged = false;
        // Mode switcher
        self.hasPVOrder = false;
        self.hasPriceOrder = false;
        self.selectedType = 0;

        // Summary of PV Orders
        self.defaultPVSummary = {
            summaryTotalAmount: 0,
            summaryTotalPV: 0,
            summaryTotalPrice: 0
        };

        self.pvSummary = angular.copy(self.defaultPVSummary);

        // Summary of Discount Orders
        self.defaultDiscountSummary = {
            summaryTotalAmount: 0,
            summaryTotalDiscountedPrice: 0,
            summaryTotalPrice: 0
        };
        self.discountSummary = angular.copy(self.defaultDiscountSummary);

        // Back-up when rollback editing session
        self.priceDetailBackup = {};
        self.pvDetailBackup = {};

        self.openPackagePVOrderModal = function () {
            var modalInstance = $uibModal.open({
                templateUrl: 'app/orders/order-package.html',
                controller: 'orderPackageCtrl',
                controllerAs: 'vm',
                backdrop: 'static',
                resolve: {
                    orderPackage: {
                        departmentId: self.selectedDepartment.Key,
                        departmentName: self.selectedDepartment.Value,
                        orderType: OrderType.pv
                    }
                }
            });

            modalInstance.result.then(function (data) {
                if (data.success === true) {
                    Notification.success('OrderWarehouse.PackageSuccess');
                    self.gridPVOrders.data = [];
                    self.pvSummary = angular.copy(self.defaultPVSummary);
                    Notification.success('Report.StartDownloading');
                    self.printOrder(OrderType.pv);
                } else {
                    Notification.warning(data.errorMessage);
                }
            });
        };

        self.openPackageDiscountOrderModal = function () {
            var modalInstance = $uibModal.open({
                templateUrl: 'app/orders/order-package.html',
                controller: 'orderPackageCtrl',
                controllerAs: 'vm',
                backdrop: 'static',
                resolve: {
                    orderPackage: {
                        departmentId: self.selectedDepartment.Key,
                        departmentName: self.selectedDepartment.Value,
                        orderType: OrderType.cash
                    }
                }
            });

            modalInstance.result.then(function (data) {
                if (data.success === true) {
                    Notification.success('OrderWarehouse.PackageSuccess');
                    self.gridDiscountOrders.data = [];
                    self.discountSummary = angular.copy(self.defaultDiscountSummary);
                    Notification.success('Report.StartDownloading');
                    self.printOrder(OrderType.cash);
                } else {
                    Notification.warning(data.errorMessage);
                }
            });
        };

        self.getNearestOrderBatchDate = function () {
            commonService.getNearestOrderBatchDate().then(function (data) {
                self.batchEndDate = data.EndDate;
                self.batchStartDate = data.StartDate;
            });
        };

        self.getAllDepartments = function () {
            warehouseService.getDepartmentWithExistingOrders().then(function (data) {
                self.departments = commonService.transferDictionary(data);

                if (self.departments.length === 0) {
                    Notification.warning('OrderWarehouse.Department.NoApprovedOrder');
                }

                self.departments.unshift({
                    Value: self.textSelectAll,
                    Key: null
                });

                self.selectedDepartment = self.departments[0];
            });
        };

        self.getOrderUsers = function () {
            var selectedLocation = self.currentLocation;
            if (typeof (selectedLocation.Id) === 'undefined') {
                return;
            }

            self.selectedDepartment = self.selectedDepartment || {};
            userService
                .getUsersByDepartment(selectedLocation.Id, self.selectedDepartment.Key)
                .then(function (data) {
                    self.orderUsers = data.Data;
                    self.orderUsers.unshift({
                        FullName: self.textSelectAll
                    });
                });
        };

        self.fetchPVOrders = function(filter) {
            warehouseService
                .searchPVOrders(filter, self.paginationOptionsPVGrid)
                .then(function (data) {
                    self.gridPVOrders.data = [];
                    self.pvSummary.summaryTotalAmount = 0;
                    self.pvSummary.summaryTotalPV = 0;
                    self.pvSummary.summaryTotalPrice = 0;

                    if (data.Data.length === 0) {
                        self.isPackaged(OrderType.pv, filter.departmentId);
                    } else {
                        self.gridPVOrders.data = data.Data;
                        self.gridPVOrders.totalItems = data.TotalItems;

                        // Bind summary fields
                        self.pvSummary.summaryTotalAmount = data.SummaryTotalAmount;
                        self.pvSummary.summaryTotalPV = data.SummaryTotalPV;
                        self.pvSummary.summaryTotalPrice = data.SummaryTotalPrice;
                    }
             });
        }

        self.fetchDiscountOrders = function(filter) {
            warehouseService
                .searchDiscountOrders(filter, self.paginationOptionsDiscountGrid)
                .then(function (data) {
                    self.gridDiscountOrders.data = [];
                    self.discountSummary.summaryTotalAmount = 0;
                    self.discountSummary.summaryTotalDiscountedPrice = 0;
                    self.discountSummary.summaryTotalPrice = 0;

                    if (data.Data.length === 0) {
                        self.isPackaged(OrderType.cash, filter.departmentId);
                    } else {
                        self.gridDiscountOrders.data = data.Data;
                        self.gridDiscountOrders.totalItems = data.TotalItems;

                        // Bind summary fields
                        self.discountSummary.summaryTotalAmount = data.SummaryTotalAmount;
                        self.discountSummary.summaryTotalDiscountedPrice = data.SummaryTotalDiscountedPrice;
                        self.discountSummary.summaryTotalPrice = data.SummaryTotalPrice;
                    }
             });
        }

        self.searchOrderDetails = function (option) {
            var selectedLocation = self.currentLocation;
            if (typeof (selectedLocation.Id) === 'undefined') {
                return;
            }

            var filter = {
                departmentId: self.selectedDepartment.Key,
                locationId: self.currentLocation.Id
            };

            switch(option) {
                case 'pv':
                    self.fetchPVOrders(filter);
                    break;
                case 'discount':
                    self.fetchDiscountOrders(filter);
                    break;
                default:
                    self.fetchPVOrders(filter);
                    self.fetchDiscountOrders(filter);
            }
        };

        self.printOrder = function (orderType) {
            var filter = {
                departmentId: self.selectedDepartment.Key,
                orderType: orderType
            };
            reportService.downloadWarehousePackageOrder(filter)
                .then(function (data) {
                    if (data.success === true) {
                        Notification.success('Report.ExportSuccessful');
                    } else {
                        Notification.warning(data.content);
                    }
                });
        };

        self.printOrderPreview = function (orderType) {
            var filter = {
                departmentId: self.selectedDepartment.Key,
                orderType: orderType
            };

            reportService.downloadWarehousePreviewOrder(filter)
                .then(function (data) {
                    if (data.success === true) {
                        Notification.success('Report.ExportSuccessful');
                    } else {
                        Notification.warning(data.content);
                    }
                });
        };

        self.isPackaged = function (orderType, departmentId) {
            warehouseService.isPackaged({
                orderType: orderType,
                departmentId: departmentId ? departmentId : null
            }).then(function (res) {
                switch (orderType) {
                    case OrderType.pv:
                        {
                            self.isPVOrderPackaged = res.IsPackaged;
                            if (res.IsPackaged === true) {
                                Notification.warning('OrderPackage.PVOrder.Packaged');
                            } else {
                                Notification.warning('OrderPackage.PVOrderNotFound');
                            }
                        }
                        break;
                    case OrderType.cash:
                        {
                            self.isDiscountOrderPackaged = res.IsPackaged;
                            if (res.IsPackaged === true) {
                                Notification.warning('OrderPackage.DiscountOrder.Packaged');
                            } else {
                                Notification.warning('OrderPackage.DiscountOrderNotFound');
                            }
                        }
                }
            });
        };

        self.onPaginationForPVOrderGrid = function (pagingOptions) {
            if (pagingOptions) {
                self.paginationOptionsPVGrid = pagingOptions;
            }

            self.searchOrderDetails('pv');
        };


        self.updateOrderDetailGrid = function () {
            self.gridPVOrders.data = self.package.PVDetails;
            self.gridPVOrders.totalItems = self.package.PVDetails.TotalItems;

            self.gridDiscountOrders.data = self.package.DiscountDetails;
            self.gridDiscountOrders.totalItems = self.package.DiscountDetails.TotalItems;
        };

        self.paginationOptionsPVGrid = gridService.getPaginationOptions();
        self.gridPVOrders = gridService.getDefaultGridOptions(self, self.onPaginationForPVOrderGrid, self.paginationOptionsPVGrid, false);
        self.gridPVOrders.columnDefs = [{
            field: 'SKU',
            displayName: 'SKU',
            headerCellFilter: 'translate'
        },
        {
            field: 'SKUName',
            displayName: 'SKU Name',
            headerCellFilter: 'translate'
        },
        {
            field: 'Quantity',
            displayName: 'Quantity',
            headerCellFilter: 'translate'
        },
        {
            field: 'UnitPrice',
            displayName: 'Unit Price (VND)',
            headerCellFilter: 'translate',
            cellFilter: 'currency:"":0'
        },
        {
            field: 'TotalPrice',
            displayName: 'Total Price (VND)',
            headerCellFilter: 'translate',
            cellFilter: 'currency:"":0'
        },
        {
            field: 'PV',
            displayName: 'PV/ea',
            headerCellFilter: 'translate'
        },
        {
            field: 'TotalPV',
            displayName: 'Total PV',
            headerCellFilter: 'translate'
        },
        {
            field: 'Department',
            displayName: 'Department',
            headerCellFilter: 'translate'
        },
        {
            field: 'Location',
            displayName: 'Location',
            headerCellFilter: 'translate'
        },
        {
            field: 'Remark',
            displayName: 'Remark',
            headerCellFilter: 'translate'
        }
        ];

        self.onPaginationForDiscountOrderGrid = function (pagingOptions) {
            if (pagingOptions) {
                self.paginationOptionsDiscountGrid = pagingOptions;
            }

            self.searchOrderDetails('discount');
        };


        self.paginationOptionsDiscountGrid = gridService.getPaginationOptions();
        self.gridDiscountOrders = gridService.getDefaultGridOptions(self, self.onPaginationForDiscountOrderGrid, self.paginationOptionsDiscountGrid, false);
        self.gridDiscountOrders.columnDefs = [{
            field: 'SKU',
            displayName: 'SKU',
            headerCellFilter: 'translate'
        },
        {
            field: 'SKUName',
            displayName: 'SKU Name',
            headerCellFilter: 'translate'
        },
        {
            field: 'Quantity',
            displayName: 'Quantity',
            headerCellFilter: 'translate'
        },
        {
            field: 'UnitPrice',
            displayName: 'Distribution Price/ea',
            headerCellFilter: 'translate',
            cellFilter: 'currency:"":0'
        },
        {
            field: 'TotalPrice',
            displayName: 'Total price before discount',
            headerCellFilter: 'translate',
            cellFilter: 'currency:"":0'
        },
        {
            field: 'UnitDiscountedPrice',
            displayName: 'Discounted amount',
            headerCellFilter: 'translate',
            cellFilter: 'currency:"":0'
        },
        {
            field: 'TotalDiscountedPrice',
            displayName: 'Total price after discount',
            headerCellFilter: 'translate',
            cellFilter: 'currency:"":0'
        },
        {
            field: 'Department',
            displayName: 'Department',
            headerCellFilter: 'translate'
        },
        {
            field: 'Location',
            displayName: 'Location',
            headerCellFilter: 'translate'
        },
        {
            field: 'Remark',
            displayName: 'Remark',
            headerCellFilter: 'translate'
        }
        ];

        self.init = function () {
            warehouseService.getWarehouseLocation()
                .then(function (data) {
                    self.currentLocation = data;
                    self.getOrderUsers();
                    self.getAllDepartments();
                    self.getNearestOrderBatchDate();
                });
        };

        // Load additional text resources then initinalize the form
        var uiSelectAll = 'UISelect.All';
        $translate(uiSelectAll).then(function (text) {
            self.textSelectAll = text;
            self.init();
        });
    }
})();
