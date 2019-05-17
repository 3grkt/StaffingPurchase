// TODO: move to directive
(function () {
    "use strict";

    angular
        .module('spApp.shared.alertModal', [])
        .controller('alertModalCtrl', ['$uibModalInstance', 'data', alertModalCtrl]);

    function alertModalCtrl($uibModalInstance, data) {
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
