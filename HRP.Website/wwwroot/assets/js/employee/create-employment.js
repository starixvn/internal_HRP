'use strict';

var createEmployment = (function ($) {
    var $employmentHeaderTab, $employmentTab;

    function registerEventChangeBVICompany() {

    }

    function initSelect2() {
        $employmentTab.find('select.search-suggestion').select2();
    }

    function initDatepicker() {
        $employmentTab.find('input.datetimepicker').datetimepicker({
            format: 'MMM DD, YYYY'
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
                        initSelect2();
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