(function ($) {
    'use strict';
    angular
        .module('spApp.products.product', [])
    .factory('productService', ['$log', 'dataService', productService]);

    function productService($log, dataService) {
        var getAllWithPaging = function (paginationOptions) {
            return dataService.search('/api/product/gets', paginationOptions);
        };

        var getAll = function () {
            return dataService.get('/api/product/get');
        };

        var getByCategory = function (categoryId) {
            if (typeof categoryId == 'undefined') {
                return;
            }

            return dataService.get('/api/product/get-by-category/' + categoryId);
        };

        var getById = function (productId) {
            return dataService.get('/api/product/getbyid/' + productId);
        };

        var search = function (filter, paginationOptions) {
            return dataService.search('/api/product/search' + '?' + $.param(filter), paginationOptions);
        };

        var remove = function (productId) {
            return dataService.remove('/api/product/delete/' + productId);
        };

        var update = function (product) {
            return dataService.put('/api/product/put', product);
        };

        var downloadTemplate = function () {
            return dataService.download('/api/product/download-template', '', 'post');
        };

        return {
            getAllWithPagin: getAllWithPaging,
            getAll: getAll,
            getByCategory: getByCategory,
            getById: getById,
            search: search,
            remove: remove,
            update: update,
            downloadTemplate: downloadTemplate
        };
    }
})(jQuery);
