(function () {
    'use strict';

    angular
        .module('spApp.products.upload', ['ngFileUpload'])
        .controller('productUploadCtrl', ['$timeout', 'Permissions', 'Page', 'commonService', 'gridService', 'fileUploadService', 'progressBarService', 'productService', 'Upload', productUploadCtrl]);

    function productUploadCtrl($timeout, Permissions, Page, commonService, gridService, fileUploadService, progressBarService, productService, Upload) {
        var self = this;

        Page.setTitle('PageTitle.UploadProductList');

        /**
         * Variables
         */
        self.alert = {
            type: '',
            msg: '',
            show: false
        };
        self.uploaded = false;
        self.uploadedFile = null;
        self.sheetName = '';

        // grid
        self.paginationOptions = angular.copy(gridService.getPaginationOptions());
        self.gridOptions = gridService.getDefaultGridOptions(self, self.bindData, self.paginationOptions);
        self.gridOptions.columnDefs = [

        ];

        /**
         * Methods
         */

        self.uploadFile = function (file) {
            progressBarService.show();

            file.upload = Upload.upload({
                url: '/api/product/upload?sheetName=' + self.sheetName,
                data: { file: file },
            });

            file.upload.then(function (response) {
                $timeout(function () {
                    //file.result = response.data;
                    commonService.setAlert(self.alert, 'Product.Upload.Success', 'success');
                    self.uploadedFile = null;
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
        };

        self.downloadTemplate = function () {
            productService.downloadTemplate();
        };

        self.init = function () {

        };

        self.init();
    }
})();
