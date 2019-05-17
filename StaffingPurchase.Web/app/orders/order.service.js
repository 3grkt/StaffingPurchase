(function () {
    'use strict';
    angular
    .module('spApp.orders.order')
    .factory('orderService', ['$log', 'dataService', orderService]);

    function orderService($log, dataService) {
        var addOrder = function (order) {
            return dataService.post('/api/order/post', order);
        }

        var addOrderDetail = function (orderDetail) {
            return dataService.post('/api/order/addorderdetail', orderDetail);
        }

        var getOrderDetails = function (isPriceMode) {
            return dataService.get('/api/order/orderdetail/' + isPriceMode);
        }

        var isEligibleUser = function () {
            return dataService.get('/api/order/get-eligible-discount');
        }

        var deleteOrderDetail = function (orderDetails) {
            return dataService.post('/api/order/delete-orderdetail', orderDetails);
        }

        var deleteOrder = function (isPrice) {
            return dataService.post('/api/order/delete-order/' + isPrice);
        }

        var checkLookedOrder = function () {
            return dataService.get('/api/order/check-locked');
        }

        return {
            addOrder: addOrder,
            addOrderDetail: addOrderDetail,
            getOrderDetails: getOrderDetails,
            isEligibleUser: isEligibleUser,
            deleteOrderDetails: deleteOrderDetail,
            deleteOrder: deleteOrder,
            checkLookedOrder: checkLookedOrder
        }
    }
})();
