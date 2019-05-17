(function () {
    'use strict';
    angular
    .module('spApp.orders.update')
    .factory('orderUpdateService', ['$log', 'dataService', orderUpdateService]);

    function orderUpdateService($log, dataService) {
        var getOrders = function (userId, orderId) {
            userId = parseInt(userId) || 0;
            orderId = parseInt(orderId) || 0;
            return dataService.get('/api/orderupdate/' + userId + '/' + orderId);
        };
        var saveOrder = function (order) {
            return dataService.put('/api/orderupdate', order);
        };
        return {
            getOrders: getOrders,
            saveOrder: saveOrder
        };
    }
})();
