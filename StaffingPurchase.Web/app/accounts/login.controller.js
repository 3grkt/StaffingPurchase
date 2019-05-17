(function () {
    'use strict';

    angular
        .module('spApp.accounts.login', [])
        .controller('loginCtrl', ['Page','commonService', 'accountService', loginCtrl]);

    function loginCtrl(Page, commonService, accountService) {
        var self = this;
        Page.setTitle('PageTitle.Login');

        /*
         * Variables
         */
        self.username = '';
        self.password = '';
        self.alert = {};

        /*
         * Methods
         */
        self.login = function () {
            accountService.login(self.username, self.password)
                .then(function (result) {
                    if (result.success) {
                        commonService.setUserData(result.data);
                        location.href = '/';
                    } else {
                        commonService.setAlert(self.alert, 'Account.FailedToLogin', 'danger');
                    }
                });
        };
    }
})();
