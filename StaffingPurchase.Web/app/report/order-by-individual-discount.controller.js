(function () {
    'use strict';

    angular
        .module('spApp.report')
        .controller('orderByIndividualDiscountCtrl', ['$uibModal','$translate','reportService','productService', 'userService', 'Page', 'Notification', 'commonService', 'gridService', orderByIndividualDiscountCtrl]);

    function orderByIndividualDiscountCtrl($uibModal, $translate, reportService, productService, userService, Page, Notification, commonService, gridService) {
        var self = this;

        Page.setTitle('PageTitle.AdminReport.OrderByIndividualDiscount');
        var uiSelectAll = 'UISelect.All';
        var textSelectAll = $translate.instant(uiSelectAll);


        // UI-Select Sources
        self.departments = [];
        self.locations = [];
        self.orderBatchDates = [];
        self.users = [];
        self.products = [];
        self.productCategories = [];
        self.orderStatuses;

        // UI-Select Selected items
        self.selectedDepartment;
        self.selectedLocation;
        self.selectedBatchDate;
        self.selectedUser;
        self.selectedProduct;
        self.selectedProductCategory;
        self.selectedOrderStatus;

        // Summary field
        self.summaryTotalAmount = 0;
        self.summaryTotalDiscountedPrice = 0;
        self.summaryTotalPrice = 0;
        self.getAllOrderBatchDates = function () {
            commonService.getAllOrderBatchDates().then(function (data) {
                self.orderBatchDates = data;
            });
        };

        self.getAllDepartments = function () {
            commonService.getAllDepartments().then(function (data) {
                self.departments = commonService.transferDictionary(data);
                self.departments.unshift({ Key: null, Value: textSelectAll === uiSelectAll? 'All':textSelectAll });
            });
        };

        self.getAllLocations = function () {
            commonService.getAllLocations().then(function (data) {
                self.locations = commonService.transferDictionary(data);
                self.locations.unshift({ Key: null, Value: textSelectAll === uiSelectAll? 'All':textSelectAll });
            });
        };

        self.getAllOrderStatuses = function () {
            commonService.getAllOrderStatus().then(function (data) {
                self.orderStatuses = commonService.transferDictionary(data);
                if (self.orderStatuses.length > 0) {
                    self.selectedOrderStatus = self.orderStatuses[0];
                }
            });
        };

        self.getOrderUsers = function () {
            self.selectedLocation = self.selectedLocation || {};
            self.selectedDepartment = self.selectedDepartment || {};
            userService
                .getUsersByDepartment(self.selectedLocation.Key, self.selectedDepartment.Key)
                .then(function (data) {
                    self.orderUsers = data.Data;
                    self.selectedUser = null;
                });
        };

        self.getAllProductCategories = function () {
            commonService.getAllProductCategories().then(function (data) {
                self.productCategories = data;
            });
        };

        self.onProductCategoryChanges = function () {
            if (typeof self.selectedProductCategory != 'undefined') {
                self.getAllProducts(self.selectedProductCategory);
            }
        };

        self.getAllProducts = function (categoryId) {
            if (typeof categoryId == 'undefined') {
                self.products = [];
                return;
            }

            productService.getByCategory(categoryId).then(function (data) {
                self.products = data.Data;
                self.selectedProduct = null;
            });
        };


        self.onPagination = function(pagingOptions) {
            if (pagingOptions) {
                self.paginationOptions = pagingOptions;
            }

            self.search();
        };

        self.paginationOptions = gridService.getPaginationOptions();
        self.gridOrderReport = gridService.getDefaultGridOptions(self, self.onPagination, self.paginationOptions, false);
        self.gridOrderReport.columnDefs = [
            { field: 'Id', displayName: 'ID', headerCellFilter: 'translate' },
            { field: 'Name', displayName: 'Name', headerCellFilter: 'translate' },
            { field: 'Department', displayName: 'Department', headerCellFilter: 'translate' },
            { field: 'Location', displayName: 'Location', headerCellFilter: 'translate' },
            { field: 'CostCenter', displayName: 'Cost Center', headerCellFilter: 'translate' },
            { field: 'SKU', displayName: 'SKU', headerCellFilter: 'translate' },
            { field: 'SKUName', displayName: 'Product', headerCellFilter: 'translate' },
            { field: 'Quantity', displayName: 'Quantity', headerCellFilter: 'translate' },
            { field: 'UnitPrice', displayName: 'Distribution Price/ea', headerCellFilter: 'translate', cellFilter: 'currency:"":0' },
            { field: 'TotalPrice', displayName: 'Total price before discount', headerCellFilter: 'translate', cellFilter: 'currency:"":0' },
            { field: 'UnitDiscountedPrice', displayName: 'Discounted amount', headerCellFilter: 'translate', cellFilter: 'currency:"":0' },
            { field: 'TotalDiscountedPrice', displayName: 'Total price after discount', headerCellFilter: 'translate', cellFilter: 'currency:"":0' },
            
            { field: 'Remark', displayName: 'Remark', headerCellFilter: 'translate' }
        ];

        self.search = function () {
            if (self.selectedBatchDate) {
                var filter = {
                    sessionEndDate: encodeURI(self.selectedBatchDate),
                    locationId: self.selectedLocation?self.selectedLocation.Key:null,
                    departmentId: self.selectedDepartment?self.selectedDepartment.Key:null,
                    sku: self.selectedProduct?self.selectedProduct.Sku:null,
                    username: self.selectedUser?self.selectedUser.UserName:null,
                    orderStatus: self.selectedOrderStatus.Key
                };
                reportService.searchOrderByIndividualDiscount(filter, self.paginationOptions)
                    .then(function (data) {
                        if(data.Data.length === 0){
                            Notification.warning('Report.Preview.NoResult');
                        }
                        
                        self.bindSearchResult(data);
                    });
            } else{
                Notification.warning('UISelect.Warning.BatchDateNotSelected');
            }
        };

        self.bindSearchResult = function (data) {
            // Bind grid datasource
            self.gridOrderReport.data = data.Data;
            self.gridOrderReport.totalItems = data.TotalItems;

            // Bind summary fields
            self.summaryTotalAmount = data.SummaryTotalAmount;
            self.summaryTotalDiscountedPrice = data.SummaryTotalDiscountedPrice;
            self.summaryTotalPrice = data.SummaryTotalPrice;
        };

        self.export = function() {
            if(self.selectedBatchDate){
                var filter = {
                    sessionEndDate: encodeURI(self.selectedBatchDate),
                    locationId: self.selectedLocation?self.selectedLocation.Key:null,
                    departmentId: self.selectedDepartment?self.selectedDepartment.Key:null,
                    sku: self.selectedProduct?self.selectedProduct.Sku:null,
                    username: self.selectedUser?self.selectedUser.UserName:null,
                    orderStatus: self.selectedOrderStatus.Key
                };
                reportService.downloadOrderByIndividualDiscount(filter).then(function(data){
                    if(data.success === false){
                        Notification.warning(data.content);
                        self.bindSearchResult({
                            Data: [],
                            TotalItems: 0,
                            SummaryTotalAmount: 0,
                            SummaryTotalDiscountedPrice: 0,
                            SummaryTotalPrice: 0
                        });
                    } else {
                        Notification.success('Report.ExportSuccessful');
                    }
                });
            } else {
                Notification.warning('UISelect.Warning.BatchDateNotSelected');
            }
        };


        self.init = function () {
            self.getAllOrderBatchDates();
            self.getAllDepartments();
            self.getAllLocations();
            self.getAllProductCategories();
            self.getAllOrderStatuses();
        };

        self.init();
    }
})();