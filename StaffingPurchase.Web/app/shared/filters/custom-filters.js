(function () {
    'use strict';

    angular
        .module('spApp.shared.filters')
        .filter('customDate', ['$filter', function ($filter) {
            return function (dateString) {
                if (!dateString) {
                    return '';
                }

                var date = new Date(dateString);
                return $filter('date')(date, 'dd/MM/yyyy');
            };
        }])
        .filter('customDateTime', ['$filter', function ($filter) {
            return function (dateString) {
                if (!dateString) {
                    return '';
                }

                var date = new Date(dateString);
                return $filter('date')(date, 'dd/MM/yyyy HH:mm:ss');
            };
        }])
        .filter('customNumber', ['$filter', function ($filter) {
            return function (value) {
                return $filter('number')(value, 0);
            };
        }])
        .filter('signedNumber', ['$filter', function ($filter) {
            return function (value) {
                var tmp = parseFloat(value) || 0;
                if (tmp > 0) {
                    return '+' + value;
                }
                return value;
            };
        }]);

})();
