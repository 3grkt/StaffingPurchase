(function ($) {
    'use strict';
    angular
        .module('spApp.shared.progressBarService', [])
        .factory('progressBarService', ['$translate', progressBarService]);

    function progressBarService($translate) {
        var numOfPendingCall = 0;
        var numOfPendingPostCall = 0;
        var translatedLoadingText;
        var $dialog = $(
            '<div id="#progressModal" class="modal fade" data-backdrop="false" data-toggle="modal" data-target="#progressModal"  tabindex="-1" role="dialog" aria-hidden="true" style="padding-top:15%; overflow-y:visible;">' +
            '<div class="modal-dialog" style="width:180px">' +
            '<div class="modal-content">' +
            '<div class="modal-body">' +
            '<div style="text-align: center">' +
            '<b id="progressBarLabel">Loading </b>' +
            '<div id="floatBarsG">' +
            '<div id="floatBarsG_1" class="floatBarsG"></div>' +
            '<div id="floatBarsG_2" class="floatBarsG"></div>' +
            '<div id="floatBarsG_3" class="floatBarsG"></div>' +
            '<div id="floatBarsG_5" class="floatBarsG"></div>' +
            '<div id="floatBarsG_6" class="floatBarsG"></div>' +
            '<div id="floatBarsG_7" class="floatBarsG"></div>' +
            '<div id="floatBarsG_4" class="floatBarsG"></div>' +
            '<div id="floatBarsG_8" class="floatBarsG"></div>' +
            '</div>' +
            '</div>' +
            '</div></div></div>');

        var $loadingFloatBar = $('<div id="floatBarsG" style="margin-top: 25px; margin-bottom: 10px">' +
            '<div id="floatBarsG_1" class="floatBarsG"></div>' +
            '<div id="floatBarsG_2" class="floatBarsG"></div>' +
            '<div id="floatBarsG_3" class="floatBarsG"></div>' +
            '<div id="floatBarsG_4" class="floatBarsG"></div>' +
            '<div id="floatBarsG_5" class="floatBarsG"></div>' +
            '<div id="floatBarsG_6" class="floatBarsG"></div>' +
            '<div id="floatBarsG_7" class="floatBarsG"></div>' +
            '<div id="floatBarsG_8" class="floatBarsG"></div>' +
            '</div>');

        var showOnHeader = function () {
            if (++numOfPendingPostCall === 1) {
                $(".wrapper .navbar-header").append($loadingFloatBar);
            }

        };

        var hideOnHeader = function () {
            if (--numOfPendingPostCall === 0 || numOfPendingPostCall < 0) {
                $("#floatBarsG").remove();
            }
        };

        var show = function (message, options) {
            numOfPendingCall++;
            if (numOfPendingCall !== 1 && numOfPendingCall > 0) {
                return;
            }
            // Assigning defaults
            if (typeof options === 'undefined') {
                options = {};
            }
            if (typeof message === 'undefined') {
                message = 'Loading';
            }
            var settings = $.extend({
                dialogSize: 'sm',
                onHide: null // This callback runs after the dialog was hidden
            }, options);

            // Configuring dialog
            $dialog.find('.modal-dialog').attr('class', 'modal-dialog').addClass('modal-' + settings.dialogSize);


            
            // Adding callbacks
            if (typeof settings.onHide === 'function') {
                $dialog.off('hidden.bs.modal').on('hidden.bs.modal', function (e) {
                    settings.onHide.call($dialog);
                });
            }
            // Opening dialog
            if (!translatedLoadingText) {
                translatedLoadingText = $translate('ProgressBar.Loading').then(function(value){
                    translatedLoadingText = value;
                    $dialog.find('#progressBarLabel').text(translatedLoadingText);
                    $dialog.modal();
                });
            } else {
                $dialog.find('#progressBarLabel').text(translatedLoadingText);
                $dialog.modal();
            }
        };

        var hide = function () {
            numOfPendingCall--;
            if (numOfPendingCall === 0 || numOfPendingCall < 0) {
                $dialog.modal('hide');
            }

        };

        return {
            show: show,
            hide: hide,
            showOnHeader: showOnHeader,
            hideOnHeader: hideOnHeader
        };
    }
})(jQuery);
