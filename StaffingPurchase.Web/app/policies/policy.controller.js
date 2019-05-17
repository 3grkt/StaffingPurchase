(function () {
    'use strict';

    angular
        .module('spApp.policies.policy', [])
        .controller('policyCtrl', ['$timeout', 'Page', 'commonService', 'policyService', 'progressBarService', 'Upload', policyCtrl]);

    function policyCtrl($timeout, Page, commonService, policyService, progressBarService, Upload) {
        var self = this;

        Page.setTitle('PageTitle.MaintainPolicy');

        /*
         * Variables
         */
        self.alert = {};
        self.allConfigs = [];
        self.policyDocument = {};

        /*
         * Methods
         */
        self.loadConfigs = function () {
            policyService.getAll().then(function (data) {
                self.allConfigs = data.AllConfigurations;
                self.policyDocument = data.PolicyDocument;
            });
        };

        self.updateConfigs = function () {
            policyService.update(self.allConfigs).then(function (res) {
                if (res.success) {
                    commonService.setAlert(self.alert, 'Policy.SuccessToUpdate', 'success');
                } else {
                    commonService.setAlert(self.alert, res.errorMessage, 'danger');
                }
            });
        };

        self.save = function () {
            Page.openConfirmModal({
                message: 'Policy.ConfirmUpdate',
                confirmCallback: self.updateConfigs
            });
        };

        self.reset = function () {
            self.loadConfigs();
        };

        self.downloadPolicyFile = function () {
            policyService.downloadFile();
        };

        self.uploadFile = function (file) {
            progressBarService.show();

            file.upload = Upload.upload({
                url: '/api/configuration/upload-policy-file',
                data: { file: file },
            });

            file.upload.then(function (response) {
                $timeout(function () {
                    commonService.setAlert(self.alert, 'Policy.Upload.Success', 'success');
                    self.uploadedFile = null;
                    self.policyDocument.Value = response.data.newFile;
                    progressBarService.hide();
                });
            }, function (response) {
                if (response.status > 0) {
                    commonService.setAlert(self.alert, response.data.Message, 'danger');
                }
                progressBarService.hide();
            }, function (evt) {
                // Math.min is to fix IE which reports 200% sometimes
                file.progress = Math.min(100, parseInt(100.0 * evt.loaded / evt.total));
            });
        }

        self.init = function () {
            self.loadConfigs();
        };

        self.init();
    }
})();
