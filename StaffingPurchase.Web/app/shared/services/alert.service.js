(function () {
    'use strict';

    angular
        .module('spApp.shared.alertService', [])
        .factory('alertService', ['$timeout', 'Notification', alertService]);

    function alertService($timeout, Notification) {
        var self = {};

        self.getDefaultAlertOptions = function () {
            return {
                mgs: '',
                type: 'default',
                show: false,
                duration: 5000,
                activeAlert: function (mgs, type, duration) {
                    var self = this;
                    self.show = true;
                    self.mgs = mgs;
                    self.type = type;
                    if (duration != null) {
                        self.duration = duration;
                    }
                    $timeout(function () {
                        self.show = false;
                    }, self.duration);
                },
                close: function () {
                    this.show = false;
                }
            }
        };



        return {
            getDefaultAlertOptions: self.getDefaultAlertOptions
        };
    }
})();