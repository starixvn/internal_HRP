'use strict';

var createEmployment = (function ($) {
    var $employmentHeaderTab, $employmentTab;

    function initDatepicker() {
        $employmentTab.find('input.datetimepicker:not(:disabled)').datetimepicker({
            format: 'MMM dd, YYYY'
        });
    }

    function loadEmploymentForm() {
        $employmentHeaderTab.on('click', function () {
            var $this = $(this);
            var isFirstLoad = $this.data('isfirstload');

            if (isFirstLoad) {
                $this.data('isfirstload', false);

                $.ajax({
                    type: 'GET',
                    url: commonFunctions.rootURL + 'EmployeeManagement/CreateEmployment',
                    dataType: 'html',
                    data: {
                        employeeId: 1
                    },
                    success: function (data) {
                        $employmentTab.html(data);

                        initDatepicker();
                    },
                    error: commonFunctions.handleAjaxError
                });
            }
        });
    }

    function initModule() {
        $employmentTab = $('#tab-employment');
        $employmentHeaderTab = $('#tab-header-employment');

        loadEmploymentForm();
    }

    return {
        InitModule: initModule
    };
}(jQuery));

$(document).ready(function () {
    createEmployment.InitModule();
});