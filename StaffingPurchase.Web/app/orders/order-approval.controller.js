(function () {
    'use strict';

    angular
        .module('spApp.orders.approval', [])
        .controller('orderApprovalCtrl', ['$location', '$timeout', '$uibModal', 'Permissions', 'OrderType', 'Page', 'commonService', 'orderBatchService', 'gridService', orderApprovalCtrl]);

    function orderApprovalCtrl($location, $timeout, $uibModal, Permissions, OrderType, Page, commonService, orderBatchService, gridService) {
        var self = this;

        Page.setTitle('Menu.OrderApproval');

        /**
         * Variables
         */
        self.searched = false;
        self.canApprove = true;
        self.canReject = commonService.hasPermission(Permissions.rejectOrder);

        self.allLocations = [];
        self.allDepartments = [];

        self.search = {
            locationId: 0,
            departmentId: 0
        };

        // batch prototype
        self.batch = {
            orderSession: '',
            orderStatus: '',
            updatedDate: '',
            orders: [],
            actionComment: '',
            totalItems: 0
        };

        // data dictionary
        self.gridData = {};
        self.gridData[OrderType.pv] = {
            batch: angular.extend({}, self.batch),
            paginationOptions: gridService.getPaginationOptions()
        };
        self.gridData[OrderType.cash] = {
            batch: angular.extend({}, self.batch),
            paginationOptions: gridService.getPaginationOptions()
        };

        self.alert = {
            type: '',
            msg: '',
            show: false
        };

        /**
         * Methods
         */
        self.setAlert = function (msg, type) {
            commonService.setAlert(self.alert, msg, type);
        };

        self.hasData = function () {
            var total = 0;
            for (var key in self.gridData) {
                total += self.gridData[key].batch.totalItems;
            }
            return total > 0;
        };

        self.bindData = function (paginationOptions, bindingData) {
            var orderType = typeof bindingData === 'object' ? bindingData.orderType : bindingData,
                currentBatch = self.gridData[orderType].batch;

            paginationOptions = paginationOptions || self.gridData[orderType].paginationOptions;

            if (self.search.locationId > 0) {
                orderBatchService
                    .searchBatches({
                        locationId: self.search.locationId,
                        departmentId: self.search.departmentId,
                        userId: self.search.userId,
                        userName: self.search.userName,
                        orderType: orderType
                    }, paginationOptions)
                    .then(function (response) {
                        currentBatch = angular.extend(currentBatch, response);
                        currentBatch.totalItems = response.totalOrders;
                        currentBatch.totalValue = response.totalValueWithFilter;
                        self.searched = true;
                    });
            } else {
                orderBatchService
                    .searchAllBatches({
                        departmentId: self.search.departmentId,
                        userId: self.search.userId,
                        userName: self.search.userName,
                        orderType: orderType
                    }, paginationOptions)
                    .then(function (response) {
                        currentBatch.status = '';
                        currentBatch.updatedDate = '';
                        currentBatch.orderSession = response.metadata.orderSession;
                        currentBatch.totalValue = response.metadata.totalValueWithFilter;
                        currentBatch.orders = response.data;
                        currentBatch.totalItems = response.totalItems;
                        self.searched = true;
                    });
            }
        };

        self.bindAllData = function () {
            self.bindData(null, OrderType.pv);
            self.bindData(null, OrderType.cash);
        };

        self.searchOrders = function () {
            // reset page index
            for (var obj in self.gridData) {
                self.gridData[obj].paginationOptions.pageIndex = 1;
            }

            self.bindAllData();
            self.setAlert('');
            $location.search({ locationId: self.search.locationId, searched: true });
        };

        self.approve = function (batchId, orderType) {
            var onApproved = function (data) {
                if (data.success) {
                    self.setAlert('OrderBatch.SuccessToApprove', 'success');
                    self.bindAllData();
                } else {
                    self.setAlert(data.errorMessage, 'danger');
                }
            };
            Page.openConfirmModal({
                message: 'OrderBatch.ConfirmApprove',
                confirmCallback: function () {
                    if (self.search.locationId > 0) {
                        orderBatchService.approve(batchId).then(onApproved);
                    } else {
                        orderBatchService.approveAll(orderType).then(onApproved);
                    }
                }
            });
        };

        self.reject = function (batchId, orderType, rejectReason) {
            var onRejected = function (data) {
                if (data.success) {
                    self.setAlert('OrderBatch.SuccessToReject', 'success');
                    self.bindAllData();
                } else {
                    self.setAlert(data.errorMessage, 'danger');
                }
            };

            Page.openConfirmModal({
                message: 'OrderBatch.ConfirmReject',
                confirmCallback: function () {
                    if (self.search.locationId > 0) {
                        orderBatchService.reject(batchId, rejectReason).then(onRejected);
                    } else {
                        orderBatchService.rejectAll(orderType, rejectReason).then(onRejected);
                    }
                }
            });
        };

        self.editOrder = function (grid, row) {
            $location
                .search({})
                .path('/order-update/' + row.entity.id);
        };

        self.viewOrder = function (grid, row) {
            $location
                .search({})
                .path('/order-view/' + row.entity.id);
        };

        self.init = function () {
            // Bind dropdowns
            commonService.getAllLocations().then(function (data) {
                self.allLocations = data;
            });
            commonService.getAllDepartments().then(function (data) {
                self.allDepartments = data;
            });

            // Retrieve data from queryString
            var qs = $location.search();
            if (qs.locationId) {
                self.search.locationId = qs.locationId;
            }

            // Bind order batch
            if (qs.searched) {
                self.bindAllData();
            }
        };

        self.init();
    }
})();
