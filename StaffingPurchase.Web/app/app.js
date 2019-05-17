(function () {
    'use strict';

    angular.module('spApp', [
        'ngRoute',

        'pascalprecht.translate', // support using text resources
        'ui.bootstrap',
        'ui.grid',
        'ui.grid.autoResize',
        'ui.grid.edit',
        'ui.grid.pagination',
        'ui.select',
        'ngSanitize',
        'ui-notification', // Support Notification
        'ngCookies',

        'spApp.page',
        'spApp.pageService',
        'spApp.shared.dataService',
        'spApp.shared.commonService',
        'spApp.shared.alertService',
        'spApp.shared.gridService',
        'spApp.menuDirective',
        'spApp.shared.confirmModal',
        'spApp.shared.alertModal',
        'spApp.shared.convertToNumber',
        'spApp.shared.fileModel',
        'spApp.shared.filters',
        'spApp.shared.progressBarService',
        'spApp.shared.fileUploadService',

        'spApp.home',
        'spApp.orders.order',
        'spApp.orders.history',
        'spApp.orders.approval',
        'spApp.orders.warehouse',
        'spApp.orders.update',
        'spApp.orders.view',
        'spApp.report',
        'spApp.users.user',
        'spApp.users.user-profile',
        'spApp.levelgroups.levelgroup',
        'spApp.products.product',
        'spApp.products.list',
        'spApp.products.detail',
        'spApp.products.upload',
        'spApp.accounts.service',
        'spApp.awards.list',
        'spApp.awards.upload',
        'spApp.awards.service',
        'spApp.policies.policy',
        'spApp.policies.service',
        'spApp.pvLog.controller',
        'spApp.pvLog.service',
        'spApp.batchJobs.log',
        'spApp.batchJobs.test',
        'spApp.batchJobs.service',

        
        'spApp.specialTasks'
    ]);
})();
