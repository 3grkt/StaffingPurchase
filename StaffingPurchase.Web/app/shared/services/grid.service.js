(function () {
    'use strict';

    angular
        .module('spApp.shared.gridService', [])
        .factory('gridService', ['$location', gridService]);

    function gridService($location) {
        var getPaginationOptions = function () {
            return {
                pageIndex: 1,
                pageSize: 10,
                sortCol: null,
                sortDir: 'ASC'
            };
        };

        var getDefaultGridOptions = function (scope, searchFn, paginationOptions, changeUrlOnPaging, searchData) {
            return {
                enableColumnMenus: false,
                paginationPageSizes: [10, 25, 50, 100],
                paginationPageSize: 10,
                useExternalPagination: true,
                useExternalSorting: true,
                enableHorizontalScrollbar: 0,
                enableVerticalScrollbar: 0,
                columnDefs: [],
                onRegisterApi: function (gridApi) {
                    scope.gridApi = gridApi;
                    scope.gridApi.core.on.sortChanged(null, function (grid, sortColumns) {
                        if (sortColumns.length === 0) {
                            paginationOptions.sortCol = null;
                        } else {
                            paginationOptions.sortCol = sortColumns[0].field;
                            paginationOptions.sortDir = sortColumns[0].sort.direction.toUpperCase();
                        }

                        // Change url - make sure related controller was register as 'reloadOnSearch: false'
                        if (!!changeUrlOnPaging) {
                            var qs = $location.search();
                            qs['sort'] = paginationOptions.sortCol;
                            qs['dir'] = paginationOptions.sortDir;
                            $location.search(qs);
                        }

                        if (searchFn) {
                            searchFn(paginationOptions, searchData);
                        }
                    });
                    gridApi.pagination.on.paginationChanged(null, function (newPage, pageSize) {
                        paginationOptions.pageIndex = newPage;
                        paginationOptions.pageSize = pageSize;

                        // Change url - make sure related controller was register as 'reloadOnSearch: false'
                        if (!!changeUrlOnPaging) {
                            var qs = $location.search();
                            qs['pageindex'] = paginationOptions.pageIndex;
                            qs['pagesize'] = paginationOptions.pageSize;
                            $location.search(qs);
                        }

                        if (searchFn) {
                            searchFn(paginationOptions, searchData);
                        }
                    });
                }
            };
        };

        var getGridOptionsWithoutSearch = function (scope, cfg) {
            var config = cfg || {};
            return {
                enableColumnMenus: false,
                enableSorting: false,
                enableHorizontalScrollbar: 0, // Always disable 
                enableVerticalScrollbar: config.disableScrollbar ? 0 : 1,
                columnDefs: [],
                onRegisterApi: function (gridApi) {
                    scope.gridApi = gridApi;
                }
            };
        };

        var getGridHeight = function (gridOptions, ignorePagination) {
            if (!gridOptions.data) {
                return {};
            }

            var rowHeight = 30;
            var headerHeight = gridOptions.customHeaderHeight || 30;
            var paginationHeight = ignorePagination ? 2 : 32; // leave some pixels when no paging to make sure grid border-bottom displayed
            var lengh = gridOptions.data.length || 1; // leave an empty line if no data
            return {
                height: (lengh * rowHeight + headerHeight + paginationHeight) + 'px',
                width: 'auto'
            };
        };

        var setPaginationByQueryString = function (gridOptions, paginationOptions, qs) {
            paginationOptions.pageIndex = gridOptions.paginationCurrentPage = parseInt(qs['pageindex']) || 1;
            paginationOptions.pageSize = gridOptions.paginationPageSize = parseInt(qs['pagesize']) || 10;

            paginationOptions.sortCol = qs['sort'];
            paginationOptions.sortDir = qs['dir'];
            
            if (paginationOptions.sortCol) {
                var sortedColumn = gridOptions.columnDefs.filter(function (value) {
                    return value.field.toLowerCase() === paginationOptions.sortCol.toLowerCase();
                })[0];

                if (sortedColumn) {
                    sortedColumn.sort = { direction: paginationOptions.sortDir.toLowerCase() };
                }
            }
        };

        return {
            getPaginationOptions: getPaginationOptions,
            getDefaultGridOptions: getDefaultGridOptions,
            getGridOptionsWithoutSearch: getGridOptionsWithoutSearch,
            getGridHeight: getGridHeight,
            setPaginationByQueryString: setPaginationByQueryString
        };
    }
})();
