(function () {
    'use strict';

    angular
        .module('spApp.products.detail', [])
        .controller('productDetailCtrl', ['$routeParams', '$location', '$window', 'Permissions', 'Page', 'commonService', 'gridService', 'productService', productDetailCtrl]);

    function productDetailCtrl($routeParams, $location, $window, Permissions, Page, commonService, gridService, productService) {
        var self = this;

        Page.setTitle('PageTitle.ProductDetail');

        /**
         * Variables
         */
        self.alert = {
            type: '',
            msg: '',
            show: false
        };

        self.allCategories = [];
        self.currentProduct = {};

        /**
         * Methods
         */
        self.close = function () {
            $window.history.back();
        };

        self.save = function () {
            productService.update(self.currentProduct).then(function(res){
                if (res.success){
                    commonService.setAlert(self.alert, 'Product.SuccessToUpdate', 'success');
                } else {
                    commonService.setAlert(self.alert, res.errorMessage, 'danger');
                }
            });
        };

        self.bindData = function () {
            if ($routeParams.id) {
                productService.getById($routeParams.id).then(function (data) {
                    self.currentProduct = data;
                });
            }
        };

        self.init = function () {
            commonService.getAllProductCategories().then(function (data) {
                self.allCategories = data;
                self.bindData();
            });
        };

        self.init();
    }
})();
