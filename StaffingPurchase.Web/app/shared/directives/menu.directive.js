(function () {
    'use strict';

    angular
        .module('spApp.menuDirective', [])
        .directive('appMenu', function() {
            return {
                templateUrl: 'app/shared/directives/menu.directive.html'
            };
        });
})();
