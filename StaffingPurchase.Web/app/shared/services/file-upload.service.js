(function () {
    'use strict';

    angular
        .module('spApp.shared.fileUploadService', [])
        .factory('fileUploadService', ['$http', fileUploadService]);

    function fileUploadService($http) {
        var uploadFileToUrl = function (file, uploadUrl) {
            var fd = new FormData();
            fd.append('file', file);

            $http.post(uploadUrl, fd, {
                transformRequest: angular.identity,
                headers: { 'Content-Type': undefined }
            })

            .success(function () {
            })

            .error(function () {
            });
        }

        return {
            uploadFileToUrl: uploadFileToUrl
        };
    }
})();
