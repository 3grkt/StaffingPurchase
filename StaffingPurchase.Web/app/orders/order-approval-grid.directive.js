(function () {
    'use strict';

    angular
        .module('spApp.orders.approval')
        .controller('orderApprovalGridController', ['$scope', '$filter', 'OrderType', 'Permissions', 'commonService', 'orderBatchService', 'gridService', orderApprovalGridController])
        .directive('orderApprovalGrid', ['gridService', orderApprovalGridDirective]);

    function orderApprovalGridController($scope, $filter, OrderType, Permissions, commonService, orderBatchService, gridService) {
        var self = this;

        self.rejectReason = '';

        // expose methods to view
        self.getGridHeight = gridService.getGridHeight;
        self.bindData = self.gridBindData();
        self.viewOrder = self.gridViewOrder();
        self.editOrder = self.gridEditOrder();
        self.approve = self.gridApprove();
        self.reject = self.gridReject();

        self.hasUpdatePermission = function () {
            return commonService.hasPermission(Permissions.updateOrder);
        };

        // grid
        self.gridOptions = gridService.getDefaultGridOptions(self, self.bindData, self.paginationOptions, false, { orderType: self.orderType });
        self.gridOptions.appScopeProvider = self; // need this to support 'vm' access in template
        self.gridOptions.columnDefs = [
            { field: 'userName', displayName: 'Order.UserName', headerCellFilter: 'translate' },
            { field: 'departmentName', displayName: 'Common.Department', headerCellFilter: 'translate' },
            { field: 'locationName', displayName: 'Common.Location', headerCellFilter: 'translate' },
            {
                field: 'orderDate', displayName: 'Order.OrderDate', headerCellFilter: 'translate',
                cellTemplate: '<div class="ui-grid-cell-contents">{{row.entity[col.field] | customDate }}</div>', width: '12%'
            },
            {
                field: 'value', displayName: 'Order.Value', headerCellFilter: 'translate',
                cellFilter: 'number', headerCellClass: 'cell-number', cellClass: 'cell-number', width: '12%'
            },
            {
                field: 'id', name: '', width: '10%',
                cellTemplate: '<div class="text-center">' +
                    '<a href="javascript:void(0);" ng-click="grid.appScope.viewOrder(grid, row)"><i class="fa fa-eye"></i></a>' + '&nbsp;' +
                    '<a href="javascript:void(0);" ng-if="grid.appScope.hasUpdatePermission()" ng-click="grid.appScope.editOrder(grid, row)"><i class="fa fa-pencil"></i></a>' +
                    '</div>'
            }
        ];

        self.getTotalValue = function (value, filterAndOptions) {
            var filterName = filterAndOptions.splice(0, 1)[0],
                args = [value].concat(filterAndOptions);
            return $filter(filterName).apply(null, args);
        };

        self.init = function init() {
            // watch data changed
            $scope.$watch(
                function () {
                    return self.gridData.orders;
                },
                function () {
                    self.gridOptions.data = self.gridData.orders;
                    self.gridOptions.totalItems = self.gridData.totalItems;

                    self.totalValue = self.getTotalValue(
                        self.gridData.totalValue,
                        self.orderType === OrderType.pv ? ['number', 2] : ['customNumber']);
                });
        };

        self.init();
    }

    function orderApprovalGridDirective(gridService) {
        return {
            scope: {
                gridData: '=',
                gridTitle: '@',
                gridBindData: '&',
                gridViewOrder: '&',
                gridEditOrder: '&',
                gridApprove: '&',
                gridReject: '&',
                canReject: '=',
                canApprove: '=',
                orderType: '=',
                paginationOptions: '=',
                sumLabel: '@'
            },
            controller: 'orderApprovalGridController',
            controllerAs: 'vm',
            bindToController: true,
            templateUrl: 'app/orders/order-approval-grid.html'
        };
    }
})();
