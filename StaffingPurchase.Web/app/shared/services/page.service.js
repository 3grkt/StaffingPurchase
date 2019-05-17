(function () {
    'use strict';

    angular
        .module('spApp.pageService', [])
        .factory('Page', ['$uibModal', function ($uibModal) {
            var title = '';
            return {
                title: function () {
                    return title;
                },

                setTitle: function (newTitle) {
                    title = newTitle;
                },

                openConfirmModal: function (options) {
                    var modalInstance = $uibModal.open({
                        templateUrl: 'app/shared/controllers/confirm-modal.html',
                        controller: 'confirmModalCtrl',
                        controllerAs: 'vm',
                        size: 'sm',
                        backdrop: 'static',
                        resolve: {
                            data: {
                                title: options.title || 'Modal.Confirm',
                                message: options.message
                            }
                        }
                    });

                    modalInstance.result.then(function (result) {
                        if (result.confirmed && options && options.confirmCallback) {
                            options.confirmCallback();
                        }
                    }, function () {
                        //$log.info('Modal dismissed at: ' + new Date());
                    });
                }
            };
        }]);
})();
