(function () {
    'use strict';

    angular
        .module('spApp.home', [])
        .controller('homeCtrl', ['Page', 'policyService', homeCtrl]);

    function homeCtrl(Page, policyService) {
        var self = this;

        Page.setTitle('PageTitle.Home');

        /*
         * Variables
         */
        self.allPolicies = [];
        self.policyDocument = {};

        /*
         * Functions
         */
        self.downloadPolicyFile = function () {
            policyService.downloadFile();
        };

        self.init = function () {
            policyService.getAll().then(function (data) {
                self.allPolicies = data.AllConfigurations;
                self.policyDocument = data.PolicyDocument;
            });
        };

        self.init();
    }
})();
