(function () {
    'use strict';

    angular
        .module('spApp.specialTasks')
        .controller('orderTaskCtrl', ['Page', 'Notification', 'commonService', 'specialTaskService', orderTaskCtrl]);

    function orderTaskCtrl(Page, Notification, commonService, specialTaskService) {
        Page.setTitle('Order Special Tasks');
        var self = this;

        self.updateOrderArea = function () {
            specialTaskService.updateOrderAreaBasedOnPackageLog()
                .then(function (res) {
                    if (res.success) {
                        Notification.success('Task has been executed successfully');
                    } else {
                        Notification.error('Task has been executed failed');
                    }
                });
        };
    }

})();