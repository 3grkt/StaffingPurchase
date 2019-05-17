(function () {
    'use strict';

    angular
    .module('spApp.orders.update')
    .controller('orderUpdateEditCtrl', ['$uibModalInstance', 'orderDetail','isPrice', orderUpdateEditCtrl]);

    function orderUpdateEditCtrl($uibModalInstance, orderDetail, isPrice) {
        var self = this;

        self.orderDetail = angular.copy(orderDetail);
        self.volumes = [1, 2, 3];
        self.isPrice = isPrice;
        self.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };


        self.save = function () {
            $uibModalInstance.close(self.orderDetail);
        };
    }
})();
