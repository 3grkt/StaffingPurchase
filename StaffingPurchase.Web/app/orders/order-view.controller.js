(function () {
    'use strict';

    angular
        .module('spApp.orders.view', [])
        .controller('orderViewCtrl', ['$window','$filter', '$translate', '$routeParams', 'Page', 'gridService','orderBatchService', 'orderUpdateService', 'orderHistoryService', orderViewCtrl]);

    function orderViewCtrl($window, $filter, $translate, $routeParams, Page, gridService,orderBatchService, orderUpdateService, orderHistoryService) {
        var self = this;

        Page.setTitle('PageTitle.ViewOrder');

        /*
         * Variables
         */
        self.viewOrderInfo = {};
        /*
         * Methods
         */
        self.back = function() {
            $window.history.back();
        };

        self.bindData = function (orderId) {
            orderHistoryService.getOrderView(orderId).then(function (data) {
                self.viewingOrderInfo = data.Order;
                self.viewingOrderInfo.FormattedSessionDate = orderBatchService.formatSessionDate(self.viewingOrderInfo.SessionStartDate, self.viewingOrderInfo.SessionEndDate);
                self.viewingOrderInfo.OrderDate = $filter('customDate')(self.viewingOrderInfo.OrderDate);
                if (self.viewingOrderInfo.OrderType === "Cash") {
                    self.orderDetailGridOptions.columnDefs[4].visible = true;
                    self.orderDetailGridOptions.columnDefs[5].visible = false;
                } else {
                    self.orderDetailGridOptions.columnDefs[4].visible = false;
                    self.orderDetailGridOptions.columnDefs[5].visible = true;
                }
                self.orderDetailGridOptions.data = data.OrderDetails;


            });
        };
        // grid
        self.orderDetailGridOptions = gridService.getGridOptionsWithoutSearch(self, { disableScrollbar: true });
        self.orderDetailGridOptions.columnDefs = [
                { field: 'OrderNum', displayName: 'STT', headerCellFilter: 'translate', cellTemplate: '<div class="ui-grid-cell-contents">{{grid.renderContainers.body.visibleRowCache.indexOf(row) + 1}}</div>', width: '10%' }, // TODO: handle sort for this field
                { field: 'ProductSku', displayName: 'Product.Sku', headerCellFilter: 'translate' },
                { field: 'ProductName', displayName: 'Product.Name', headerCellFilter: 'translate', width: '40%' },
                { field: 'Volume', displayName: 'Product.Quantity', headerCellFilter: 'translate' },
                { field: 'Price', displayName: 'Product.Price', headerCellFilter: 'translate', cellFilter: 'discountFilter:grid.appScope.vm.isEligibleUser | currency:"VND ":0', visible: true },
                { field: 'PV', displayName: 'Product.PV', headerCellFilter: 'translate', visible: false }
        ];

        if ($routeParams.id) {
            self.bindData($routeParams.id);
        }
    }
})();
