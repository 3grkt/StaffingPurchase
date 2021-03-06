(function () {
    'use strict';

    angular
        .module('spApp.shared.dataService', ['spApp.shared.progressBarService'])
        .factory('dataService', ['$http', '$q', '$log', 'progressBarService', dataService]);

    function dataService($http, $q, $log, progressBarService) {
        var onPostSuccess = function (defer, response) {
            defer.resolve({
                success: true,
                data: response.data || response
            });
        };

        var onPostError = function (defer, error) {
            $log.log('error: ' + error);
            defer.resolve({
                success: false,
                errorMessage: error.data.message || error.data.Message || error.data.error
            });
        };

        var buildApiQuery = function (api, query) {
            var querySeparator = api.indexOf('?') >= 0 ? '&' : '?';
            return api + (query ? querySeparator + query.substr(1) : '');
        };

        var search = function (api, paginationOptions) {
            var defer = $q.defer();
            progressBarService.show();

            var query = '';

            if (paginationOptions.pageIndex && paginationOptions.pageIndex > 0) {
                query += '&pageIndex=' + paginationOptions.pageIndex;
            }
            if (paginationOptions.pageSize && paginationOptions.pageSize > 0) {
                query += '&pageSize=' + paginationOptions.pageSize;
            }
            if (paginationOptions.sortCol) {
                query += '&sort=' + paginationOptions.sortCol + '&dir=' + paginationOptions.sortDir;
            }

            $http.get(buildApiQuery(api, query))
                .then(function (response) {
                    defer.resolve(response.data);
                    progressBarService.hide();
                }, function (error) {
                    onPostError(defer, error);
                    progressBarService.hide();
                });

            return defer.promise;
        };

        var get = function (api, config) {
            var defer = $q.defer();
            progressBarService.show();
            $http.get(api, config)
                .then(function (response) {
                    defer.resolve(response.data);
                    progressBarService.hide();
                }, function (error) {
                    onPostError(defer, error);
                    progressBarService.hide();
                });

            return defer.promise;
        };

        var download = function (api, fileName, method, postData) {
            var deferred = $q.defer();
            fileName = fileName || '';
            method = (method || 'get').toLowerCase(); // TODO: keep default as GET for now, but it should be POST
            progressBarService.show();

            var onDownloadSuccess = function (data, status, headers) {
                var defaultFileName = fileName;
                var type = headers('Content-Type');
                var disposition = headers('Content-Disposition');
                if (disposition) {
                    var match = disposition.match(/.*filename=\"?([^;\"]+)\"?.*/);
                    if (match[1])
                        defaultFileName = match[1];
                }
                defaultFileName = defaultFileName.replace(/[<>:"\/\\|?*]+/g, '_');
                var blob = new Blob([data], { type: type });
                saveAs(blob, defaultFileName);
                deferred.resolve({
                    success: true,
                    content: defaultFileName
                });
                progressBarService.hide();
            };

            var onDownloadError = function (data, status) {
                var dataContent = 'Error';

                if (data instanceof ArrayBuffer) {
                    dataContent = decodeUTF8(new Uint8Array(data));
                }
                $log.log('error: ' + dataContent);
                deferred.resolve({
                    success: false,
                    content: dataContent
                });

                progressBarService.hide();
            };

            var requestConfig = { responseType: 'arraybuffer' };
            if (method === 'get') {
                $http.get(api, requestConfig).success(onDownloadSuccess).error(onDownloadError);
            } else {
                $http.post(api, postData, requestConfig).success(onDownloadSuccess).error(onDownloadError);
            }
            return deferred.promise;
        };

        var post = function (api, data, config) {
            var defer = $q.defer();
            progressBarService.showOnHeader();

            $http.post(api, data, config)
                .then(function (response) {
                    onPostSuccess(defer, response);
                    progressBarService.hideOnHeader();
                }, function (error) {
                    onPostError(defer, error);
                    progressBarService.hideOnHeader();
                });

            return defer.promise;
        };

        var put = function (api, data, config) {
            var defer = $q.defer();
            progressBarService.showOnHeader();

            $http.put(api, data, config)
                .then(function (response) {
                    onPostSuccess(defer, response);
                    progressBarService.hideOnHeader();
                }, function (error) {
                    onPostError(defer, error);
                    progressBarService.hideOnHeader();
                });

            return defer.promise;
        };

        var remove = function (api, config) {
            var defer = $q.defer();
            progressBarService.showOnHeader();

            $http.delete(api, config)
                .then(function (response) {
                    onPostSuccess(defer, response);
                    progressBarService.hideOnHeader();
                }, function (error) {
                    onPostError(defer, error);
                    progressBarService.hideOnHeader();
                });

            return defer.promise;
        };

        // Marshals a string to Uint8Array.
        var encodeUTF8 = function(s) {
            var i = 0;
            var bytes = new Uint8Array(s.length * 4);
            for (var ci = 0; ci != s.length; ci++) {
                var c = s.charCodeAt(ci);
                if (c < 128) {
                    bytes[i++] = c;
                    continue;
                }
                if (c < 2048) {
                    bytes[i++] = c >> 6 | 192;
                } else {
                    if (c > 0xd7ff && c < 0xdc00) {
                        if (++ci == s.length) throw 'UTF-8 encode: incomplete surrogate pair';
                        var c2 = s.charCodeAt(ci);
                        if (c2 < 0xdc00 || c2 > 0xdfff) throw 'UTF-8 encode: second char code 0x' + c2.toString(16) + ' at index ' + ci + ' in surrogate pair out of range';
                        c = 0x10000 + ((c & 0x03ff) << 10) + (c2 & 0x03ff);
                        bytes[i++] = c >> 18 | 240;
                        bytes[i++] = c >> 12 & 63 | 128;
                    } else { // c <= 0xffff
                        bytes[i++] = c >> 12 | 224;
                    }
                    bytes[i++] = c >> 6 & 63 | 128;
                }
                bytes[i++] = c & 63 | 128;
            }
            return bytes.subarray(0, i);
        };

        // Unmarshals an Uint8Array to string.
        var decodeUTF8 = function(bytes) {
            var s = '';
            var i = 0;
            while (i < bytes.length) {
                var c = bytes[i++];
                if (c > 127) {
                    if (c > 191 && c < 224) {
                        if (i >= bytes.length) throw 'UTF-8 decode: incomplete 2-byte sequence';
                        c = (c & 31) << 6 | bytes[i] & 63;
                    } else if (c > 223 && c < 240) {
                        if (i + 1 >= bytes.length) throw 'UTF-8 decode: incomplete 3-byte sequence';
                        c = (c & 15) << 12 | (bytes[i] & 63) << 6 | bytes[++i] & 63;
                    } else if (c > 239 && c < 248) {
                        if (i + 2 >= bytes.length) throw 'UTF-8 decode: incomplete 4-byte sequence';
                        c = (c & 7) << 18 | (bytes[i] & 63) << 12 | (bytes[++i] & 63) << 6 | bytes[++i] & 63;
                    } else throw 'UTF-8 decode: unknown multibyte start 0x' + c.toString(16) + ' at index ' + (i - 1);
                    ++i;
                }

                if (c <= 0xffff) s += String.fromCharCode(c);
                else if (c <= 0x10ffff) {
                    c -= 0x10000;
                    s += String.fromCharCode(c >> 10 | 0xd800)
                    s += String.fromCharCode(c & 0x3FF | 0xdc00)
                } else throw 'UTF-8 decode: code point 0x' + c.toString(16) + ' exceeds UTF-16 reach';
            }
            return s;
        };

        return {
            search: search,
            post: post,
            put: put,
            get: get,
            remove: remove,
            download: download
        };
    }
})();
