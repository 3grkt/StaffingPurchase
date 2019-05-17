(function () {
    'use strict';

    angular
        .module('spApp.orders.warehouse')
        .controller('orderPackageCtrl', ['$uibModalInstance', '$log', 'warehouseService', 'orderPackage', orderPackageCtrl]);

    function orderPackageCtrl($uibModalInstance, $log, warehouseService, orderPackage) {
        var self = this;

        self.package = {
            departmentId: orderPackage.departmentId,
            departmentName: orderPackage.departmentName,
            orderType: orderPackage.orderType,
            isDeficient: false,
            note: ""
        };

        self.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };

        self.packageOrder = function () {
            warehouseService.packageOrder(self.package).then(function (data) {
                $uibModalInstance.close(data);
            });
        };

        self.packageAllOrder = function () {
            warehouseService.packageAllOrder(self.package.orderType).then(function (data) {
                $uibModalInstance.close(data);
            });
        };
    }
})();
