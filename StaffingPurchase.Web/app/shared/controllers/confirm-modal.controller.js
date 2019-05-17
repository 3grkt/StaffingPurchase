// TODO: move to directive
(function () {
    'use strict';

    angular
        .module('spApp.shared.confirmModal', [])
        .controller('confirmModalCtrl', ['$uibModalInstance', 'data', confirmModalCtrl]);

    function confirmModalCtrl($uibModalInstance, data) {
        var self = this;

        /*
         * Variables
         */
        self.title = '';
        self.message = '';

        /*
         * Methods
         */
        self.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };

        self.accept = function () {
            $uibModalInstance.close({ confirmed: true }); // pass new user to parent
        };

        self.init = function () {
            if (data) {
                self.title = data.title;
                self.message = data.message;
            }
        };

        // Init
        self.init();
    }
})();
