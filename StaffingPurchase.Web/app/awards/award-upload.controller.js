(function () {
    'use strict';

    angular
        .module('spApp.awards.upload', ['ngFileUpload'])
        .controller('awardUploadCtrl', ['$timeout', 'Permissions', 'Page', 'commonService', 'gridService', 'progressBarService', 'awardService', 'Upload', awardUploadCtrl]);

    function awardUploadCtrl($timeout, Permissions, Page, commonService, gridService, progressBarService, awardService, Upload) {
        var self = this;

        Page.setTitle('PageTitle.UploadAwardList');

        /**
         * Variables
         */
        self.alert = {
            type: '',
            msg: '',
            show: false
        };
        
        self.uploadedFile = null;
        self.sheetName = '';
        self.awardId = '';
        self.allAwards = [];

        /**
         * Methods
         */

        self.uploadFile = function (file) {
            progressBarService.show();

            file.upload = Upload.upload({
                url: '/api/award/upload?sheetName=' + self.sheetName + '&awardId=' + self.awardId,
                data: { file: file }
            });

            file.upload.then(function (response) {
                $timeout(function () {
                    commonService.setAlert(self.alert, 'Award.Upload.Success', 'success');
                    self.uploadedFile = null;
                    self.awardId = '';
                    progressBarService.hide();
                });
            }, function (response) {
                if (response.status > 0) {
                    commonService.setAlert(self.alert, response.data.message, 'danger');
                }
                progressBarService.hide();
            }, function (evt) {
                // Math.min is to fix IE which reports 200% sometimes
                file.progress = Math.min(100, parseInt(100.0 * evt.loaded / evt.total));
            });
        };

        self.bindAwards = function () {
            awardService.getAllAwards().then(function (response) {
                self.allAwards = response.data;
            });
        };

        self.downloadTemplate = function () {
            awardService.downloadTemplate();
        };

        self.init = function () {
            self.bindAwards();
        };

        self.init();
    }
})();
