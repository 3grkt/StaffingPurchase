(function () {
    'use strict';

    angular
        .module('spApp.orders.warehouse')
        .factory('warehouseService', ['$httpParamSerializer', 'dataService', warehouseService]);

    function warehouseService($httpParamSerializer, dataService) {
        var baseApi = 'api/orderwarehouse';
        var searchPVOrders = function (filter, pagingOptions) {
            var api = baseApi + '/order/pv?' + $httpParamSerializer(filter);
            return dataService.search(api, pagingOptions);
        };

        var searchDiscountOrders = function(filter, pagingOptions){
            var api = baseApi + '/order/discount?' + $httpParamSerializer(filter);
            return dataService.search(api, pagingOptions);
        };

        var getWarehouseLocation = function () {
            return dataService.get(baseApi + '/warehouse-location');
        };

        var getDepartmentWithExistingOrders = function () {
            return dataService.get(baseApi + '/existing-order/department');
        };

        var getDepartmentWithPackagedOrders = function () {
            return dataService.get(baseApi + '/packaged-order/department');
        };

        var packageOrder = function (requestPackage) {
            return dataService.post(baseApi + '/package/single', requestPackage);
        };

        var packageAllOrder = function (orderType) {
            return dataService.post(baseApi + '/package/all', orderType);
        };

        var isPackaged = function(filter){
            return dataService.get(baseApi + '/ispackaged/?' +$httpParamSerializer(filter));
        };

        return {
            searchPVOrders: searchPVOrders,
            searchDiscountOrders: searchDiscountOrders,
            getWarehouseLocation: getWarehouseLocation,
            packageOrder: packageOrder,
            packageAllOrder: packageAllOrder,
            getDepartmentWithExistingOrders: getDepartmentWithExistingOrders,
            getDepartmentWithPackagedOrders: getDepartmentWithPackagedOrders,
            isPackaged: isPackaged
        };
    }
})();
