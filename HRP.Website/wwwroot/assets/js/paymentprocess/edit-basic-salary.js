'use strict';

var editBasicSalary = (function ($) {
    var $btnPreset, $btnRefresh, $btnCancel, $form,
        salaryPerDay;

    function calculateTotalAddition() {
        $('.js-addition-netSalary').each(function () {
            var $this = $(this),
                value = $this.val().trim(),
                totalAddition = 0;

            if (value.length > 0) {
                totalAddition += new BigNumber(value);
            }
        });
    }

    function initSelect2() {
        $('.js-select2-disabled').select2();
    }

    function registerRefreshPageEvent() {
        $btnRefresh.on('click', function () {
            location.reload();
        });
    }

    function initModule() {
        $btnPreset = $('.js-btnPreset');
        $btnRefresh = $('.js-btnRefresh');
        $btnCancel = $('.js-btnCancel');
        $form = $('#edit-basic-salary-form');
        salaryPerDay = $('#SalaryPerDay');

        registerRefreshPageEvent();

        initSelect2();
    }

    return {
        initModule: initModule
    };
}(jQuery));

$(document).ready(function () {
    editBasicSalary.initModule();
});