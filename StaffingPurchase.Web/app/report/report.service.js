(function ($) {
    'use strict';

    angular
        .module('spApp.report.service', [])
        .factory('reportService', ['$log', 'dataService', reportService]);

    function reportService($log, dataService) {
        var searchInternalRequisitionForm = function (filter, paginationOptions) {
            var api = 'api/order/admin-report/search/internal-requisition-form/?' + $.param(filter);
            return dataService.search(api, paginationOptions);
        };

        var searchSummaryDiscountProduct = function (filter, paginationOptions) {
            var api = 'api/order/admin-report/search/summary-discount-product/?' + $.param(filter);
            return dataService.search(api, paginationOptions);
        };

        var searchOrderByIndividualPV = function (filter, paginationOptions) {
            var api = 'api/order/admin-report/search/order-by-individual-pv/?' + $.param(filter);
            return dataService.search(api, paginationOptions);
        };

        var searchOrderByIndividualDiscount = function (filter, paginationOptions) {
            var api = 'api/order/admin-report/search/order-by-individual-discount/?' + $.param(filter);
            return dataService.search(api, paginationOptions);
        };


        var downloadInternalRequisitionForm = function (filter) {
            var api = 'api/order/admin-report/download/internal-requisition-form/?' + $.param(filter);
            return dataService.download(api);
        };

        var downloadSummaryDiscountProduct = function (filter) {
            var api = 'api/order/admin-report/download/summary-discount-product/?' + $.param(filter);
            return dataService.download(api);
        };

        var downloadOrderByIndividualPV = function (filter) {
            var api = 'api/order/admin-report/download/order-by-individual-pv/?' + $.param(filter);
            return dataService.download(api);
        };

        var downloadOrderByIndividualDiscount = function (filter) {
            var api = 'api/order/admin-report/download/order-by-individual-discount/?' + $.param(filter);
            return dataService.download(api);
        };

        var downloadWarehousePackageOrder = function (filter) {
            var api = '/api/order/warehouse-report/download/package-report/?' + $.param(filter);
            return dataService.download(api);
        };

        var downloadWarehousePreviewOrder = function (filter) {
            var api = '/api/order/warehouse-report/download/preview-report/?' + $.param(filter);
            return dataService.download(api);
        };

        return {
            searchInternalRequisitionForm: searchInternalRequisitionForm,
            searchSummaryDiscountProduct: searchSummaryDiscountProduct,
            searchOrderByIndividualPV: searchOrderByIndividualPV,
            searchOrderByIndividualDiscount: searchOrderByIndividualDiscount,
            downloadInternalRequisitionForm: downloadInternalRequisitionForm,
            downloadSummaryDiscountProduct: downloadSummaryDiscountProduct,
            downloadOrderByIndividualPV: downloadOrderByIndividualPV,
            downloadOrderByIndividualDiscount: downloadOrderByIndividualDiscount,
            downloadWarehousePackageOrder: downloadWarehousePackageOrder,
            downloadWarehousePreviewOrder: downloadWarehousePreviewOrder
        };
    }
})(jQuery);
