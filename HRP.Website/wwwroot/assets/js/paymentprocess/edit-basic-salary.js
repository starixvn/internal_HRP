'use strict';

var editBasicSalary = (function ($) {
    var $btnPreset, $btnRefresh, $btnCancel;
    function initSelect2() {
        $('.js-select2').select2();
    }

    function initModule() {
        $btnPreset = $('.js-btnPreset');
        $btnRefresh = $('.js-btnRefresh');
        $btnCancel = $('.js-btnCancel');

        initSelect2();
    }

    return {
        initModule: initModule
    };
}(jQuery));

$(document).ready(function () {
    editBasicSalary.initModule();
});