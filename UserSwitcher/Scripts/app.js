var userSwitcher = angular.module("userSwitcherApp", []);

userSwitcher.directive('dynamic', function ($compile) {
    return {
        restrict: 'A',
        replace: true,
        link: function (scope, ele, attrs) {
            scope.$watch(attrs.dynamic, function (html) {
                ele.html(html);
                $compile(ele.contents())(scope);
            });
        }
    };
});

userSwitcher.ui = {

    init: function () {
        window.alert = function (args) {
            bootbox.alert(args);
        }

        if (!window.console) {
            window.console = {};
            if (!window.console.log) {
                window.console.log = function () { };
            }
        }
    },

    toggleOverlay: function (willDisplay) {
        if (willDisplay && !$('div.overlay').is(':visible')) {
            $('div.overlay').fadeIn(50);
        }
        else {
            $('div.overlay').fadeOut(50);
        }
    },

    _endobj: null,
};

$(document).ready(function () {
    userSwitcher.ui.init();
});