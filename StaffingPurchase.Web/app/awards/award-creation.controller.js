(function () {
    'use strict';

    angular
        .module('spApp.awards.list') // share module with award-list.controller.js
        .controller('awardCreationCtrl', ['$uibModalInstance', 'awardService', 'award', awardListCtrl]);

    function awardListCtrl($uibModalInstance, awardService, award) {
        var self = this;

        /**
         * Variables
         */
        self.alert = {
            type: '',
            msg: '',
            show: false
        };

        self.award = {
            name: '',
            pv: 0
        };

        self.isEdit = false;

        /**
         * Methods
         */
        self.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };

        self.onSaveCompleted = function (res) {
            if (res.success) {
                $uibModalInstance.close(self.award);
            } else {
                self.errorMessage = res.errorMessage;
            }
        };

        self.save = function () {
            if (self.isEdit) {
                awardService.update(self.award).then(self.onSaveCompleted);
            } else {
                awardService.create(self.award).then(self.onSaveCompleted);
            }
        };

        self.init = function () {
            self.award = award;
            self.isEdit = award && award.id > 0;
        };

        self.init();
    }
})();
