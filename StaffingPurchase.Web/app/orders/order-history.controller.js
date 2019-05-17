(function () {
    'use strict';

    angular
       .module('spApp.orders.history', [])
       .controller('orderHistoryCtrl', ['$location', '$filter', '$translate', '$uibModal', 'Notification', 'Page', 'gridService', 'commonService', 'orderHistoryService', 'orderBatchService', orderHistoryCtrl])
       .filter('orderValueFilter', ['$filter', function($filter){
           return function(value, rowEntity){
               if(rowEntity.OrderType === "Cash"){
                   return $filter('currency')(value, 'VND ', 0);
               }
               else if(rowEntity.OrderType === "PV"){
                   return $filter('currency')(value, 'PV ');
               }
               else{
                   return value;
               }
           };
       }]);


    function orderHistoryCtrl($location, $filter, $translate, $uibModal, Notification, Page, gridService, commonService, orderHistoryService, orderBatchService) {
        var self = this;
        var uiSelectAll = 'UISelect.All';
        var textSelectAll = $translate.instant(uiSelectAll);
        Page.setTitle('Menu.OrderHistory');

        /**
         * Variables
         */
        self.allOrderStatus = [];
        self.selectedOrderStatus = {Key: 0};
        self.viewOrderDetails = false;

        self.search = {
            startDate: new Date(),
            endDate: new Date()
        };
        self.search.startDate.setDate(1);

        self.datePopup = {
            startOpened: false,
            endOpened: false
        };

        self.historyRecords = [];
        self.searched = false;

        self.filter = {
            startDate: new Date().toJSON(),
            endDate: new Date().toJSON(),
            status: null
        };

        /**
         * Methods
         */
        self.openStartDate = function () {
            self.datePopup.startOpened = true;
        };

        self.openEndDate = function () {
            self.datePopup.endOpened = true;
        };

        self.back = function () {
            self.searched = true;
            self.viewOrderDetails = false;
            self.viewingOrderInfo = {};
            self.orderDetailGridOptions.data = [];
        };

        self.viewingOrderInfo = {};

        self.getAllOrderStatus = function () {
            commonService.getAllOrderStatus().then(function (data) {
                self.allOrderStatus = commonService.transferDictionary(data);
                self.allOrderStatus.unshift({ Key: null, Value: textSelectAll });
                if (self.allOrderStatus.length > 0) {
                    self.selectedOrderStatus = self.allOrderStatus[0];
                }
            });
        };

        self.bindData = function (filter) {
            if (filter) {
                self.filter = filter;
            }
            
            if (self.paginationOptions.sortCol === 'getOrderSession()') {
                self.paginationOptions.sortCol = 'SessionStartDate';
            }

            orderHistoryService.search(self.paginationOptions, self.filter)
                .then(function (data) {
                    if(data.TotalItems === 0){
                        Notification.warning('OrderHistory.NoData');
                        self.searched = false;
                        return;
                    }
                    self.searched = true;
                    self.gridOptions.data = data.Data;
                    self.gridOptions.totalItems = data.TotalItems;

                    // grid field's method
                    angular.forEach(data.Data, function (row) {
                        row.getOrderSession = function () {
                            return orderBatchService.formatSessionDate(this.SessionStartDate, this.SessionEndDate);
                        };
                    });
                });
        };

        self.searchOrder = function () {
            self.paginationOptions.pageIndex = 1;
            self.gridOptions.paginationCurrentPage = 1;
            self.bindData({
                startDate: self.search.startDate.toJSON(),
                endDate: self.search.endDate.toJSON(),
                status: self.selectedOrderStatus.Key
            });

            var qs = {};
            qs['searched'] = true;
            angular.extend(qs, self.filter);
            $location.search(qs);
        };

        self.onPagination = function(pagingOptions) {
            if (pagingOptions) {
                self.paginationOptions = pagingOptions;
            }

            self.bindData();
        };

        self.bindSearchParamsFromUrl = function (qs) {
            // filter
            for (var key in qs) {
                if (typeof self.filter[key] !== 'undefined') {
                    self.filter[key] = qs[key];
                }
            }

            // pagination
            gridService.setPaginationByQueryString(self.gridOptions, self.paginationOptions, qs);
        };

        // grid
        self.paginationOptions = angular.copy(gridService.getPaginationOptions());
        self.gridOptions = gridService.getDefaultGridOptions(self, self.onPagination, self.paginationOptions, true);
        self.gridOptions.columnDefs = [
            { field: 'getOrderSession()', displayName: 'Order.OrderSession', headerCellFilter: 'translate', width: '20%' }, // TODO: handle sort for this field
            { field: 'OrderDate', displayName: 'Order.OrderDate', headerCellFilter: 'translate', cellFilter: 'customDate' },
            { field: 'OrderTypeDescription', displayName: 'Order.OrderType', headerCellFilter: 'translate' },
            { field: 'Status', displayName: 'Order.Status', headerCellFilter: 'translate' },
            { field: 'Value', displayName: 'Order.ValueDescription', headerCellFilter: 'translate', cellFilter:'orderValueFilter: row.entity' },
            {
                field: 'View', displayName: ' ', headerCellFilter: 'translate', width: '10%',
                cellTemplate: '<div class="text-center">' +
                    '<a href="/#/order-view/{{row.entity.OrderId}}">Xem</a>' +
                    '</div>'
            }
        ];

        self.orderDetailGridOptions = gridService.getGridOptionsWithoutSearch(self);
        self.orderDetailGridOptions.columnDefs = [
        { field: 'OrderNum', displayName: 'STT', headerCellFilter: 'translate', cellTemplate: '<div class="ui-grid-cell-contents">{{grid.renderContainers.body.visibleRowCache.indexOf(row) + 1}}</div>', width: '10%' },
        { field: 'ProductSku', displayName: 'Product.Sku', headerCellFilter: 'translate' },
        { field: 'ProductName', displayName: 'Product.Name', headerCellFilter: 'translate', width: '40%' },
        { field: 'Volume', displayName: 'Product.Quantity', headerCellFilter: 'translate' },
        { field: 'Price', displayName: 'Product.Price', headerCellFilter: 'translate', cellFilter: 'discountFilter:grid.appScope.vm.isEligibleUser | currency:"VND ":0', visible: true },
        { field: 'PV', displayName: 'Product.PV', headerCellFilter: 'translate', visible: false }
        ];


        self.init = function () {
            self.getAllOrderStatus();
            var qs = $location.search();
            if (qs['searched']) {
                self.bindSearchParamsFromUrl(qs);
                self.bindData();
            }
        };


        self.init();

    }
})();
