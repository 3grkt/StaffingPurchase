(function () {
    'use strict';

    angular
        .module('spApp.products.list', [])
        .controller('productListCtrl', ['$location', '$uibModal', 'Permissions', 'Page', 'commonService', 'gridService', 'productService', productListCtrl]);

    function productListCtrl($location, $uibModal, Permissions, Page, commonService, gridService, productService) {
        var self = this;

        Page.setTitle('PageTitle.MaintainProductList');

        /**
         * Variables
         */
        self.alert = {
            type: '',
            msg: '',
            show: false
        };

        self.filter = {
            sku: '',
            name: '',
            categoryId: '',
            activeOnly: true
        };

        self.allCategories = [];
        self.searched = false;

        /**
         * Methods
         */
        self.bindData = function () {
            productService.search(self.filter, self.paginationOptions).then(function (response) {
                self.searched = true;

                self.gridOptions.data = response.Data;
                self.gridOptions.totalItems = response.TotalItems;
            });
        };

        self.bindSearchParamsFromUrl = function (qs) {
            // filter
            for (var key in qs) {
                if (typeof self.filter[key] !== 'undefined') {
                    self.filter[key] = qs[key];
                }
            }

            // pagination
            gridService.setPaginationByQueryString(self.gridOptions, self.paginationOptions, qs);
        };

        self.searchProducts = function () {
            self.paginationOptions.pageIndex = 1;
            self.gridOptions.paginationCurrentPage = 1;
            self.bindData(self.paginationOptions);

            // update url
            var qs = {};
            qs['searched'] = true;
            angular.extend(qs, self.filter);
            $location.search(qs);
        };

        self.getProductDetailUrl = function (row) {
            return '/product-detail/' + row.entity.Id;
        };

        self.viewProduct = function (grid, row) {
            $location.path(self.getProductDetailUrl(row)).search({});
        };

        self.deleteProduct = function (grid, row) {
            Page.openConfirmModal({
                message: 'Product.ConfirmDelete',
                confirmCallback: function () {
                    productService.remove(row.entity.Id).then(function (res) {
                        if (res.success) {
                            self.bindData();
                            commonService.setAlert(self.alert, 'Product.SuccessToDelete', 'success');
                        } else {
                            commonService.setAlert(self.alert, res.errorMessage, 'danger');
                        }
                    });
                }
            });
        };

        self.getActiveCss = function (grid, row) {
            return row.entity.IsActive ? 'fa fa-check' : '';
        };

        // grid
        self.paginationOptions = angular.copy(gridService.getPaginationOptions());
        self.gridOptions = gridService.getDefaultGridOptions(self, self.bindData, self.paginationOptions, true);
        self.gridOptions.columnDefs = [
            {
                field: 'Sku', displayName: 'Product.Sku', headerCellFilter: 'translate', width: 100,
                cellTemplate: '<a href="javascript:void({{row.entity.Id}});" ng-click="grid.appScope.vm.viewProduct(grid, row)">{{grid.getCellValue(row,col)}}</a>'
            },
            { field: 'Name', displayName: 'Product.Name', headerCellFilter: 'translate' },
            { field: 'PV', displayName: 'Product.PV', headerCellFilter: 'translate', width: 80 },
            { field: 'Price', displayName: 'Product.Price', headerCellFilter: 'translate', width: 100 },
            { field: 'CategoryName', displayName: 'Product.Category', headerCellFilter: 'translate', width: 150 },
            {
                field: 'IsActive', displayName: 'Product.IsActive', headerCellFilter: 'translate', width: 90,
                cellTemplate: '<div class="text-center"><i class="{{grid.appScope.vm.getActiveCss(grid, row)}}"></i></div>'
            },
            {
                field: 'Id', name: '', width: 60,
                cellTemplate: '<div class="text-center">' +
                '<a href="javascript:void(0);" ng-click="grid.appScope.vm.deleteProduct(grid, row)"><i class="fa fa-remove"></i></a>' +
                '</div>'
            }
        ];

        self.init = function () {
            commonService.getAllProductCategories().then(function (data) {
                self.allCategories = data;
            });

            var qs = $location.search();
            if (qs['searched']) {
                self.bindSearchParamsFromUrl(qs);
                self.bindData();
            }
        };

        self.init();
    }
})();
