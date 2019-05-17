(function () {
    'use strict';

    angular
        .module('spApp.batchJobs.log') // same module as parent controller
        .controller('batchJobLogDetailCtrl', ['$uibModalInstance', 'batchJobService', 'log', batchJobLogDetailCtrl]);

    function batchJobLogDetailCtrl($uibModalInstance, batchJobService, log) {
        var self = this;

        /*
         * Variables
         */
        self.log = {};

        /*
         * Methods
         */
        self.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };

        self.init = function () {
            self.log = log;
        };

        self.init();
    }
})();
