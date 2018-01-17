'use strict';

var createEmployment = (function ($) {
    var $personalDetailsStep, $employmentStep,
        $personalDetailHeaderTab, $employmentHeaderTab, $employmentContentTab, $createEmploymentForm,
        $companyCodeElement, $employeeCodeElement, $pdfPasswordElement, $btnBack;

    function initEventChangeCompanyCode() {
        $companyCodeElement.on('change', function () {
            $pdfPasswordElement.val($companyCodeElement.val().trim() + $employeeCodeElement.val().trim()).valid();
        });
    }

    function initEventChangeEmployeeCode() {
        $employeeCodeElement.on('change', function () {
            $pdfPasswordElement.val($companyCodeElement.val().trim() + $employeeCodeElement.val().trim()).valid();
        });
    }

    function initEventBackToPreviousAction() {
        $btnBack.on('click', function () {
            $personalDetailsStep.removeClass('finished').addClass('active');
            $employmentStep.removeClass('active');
            $personalDetailHeaderTab.trigger('click');
        });
    }

    function registerEventSubmitForm() {
        $createEmploymentForm.submit(function (e) {
            e.preventDefault();
            $createEmploymentForm.find('select.search-suggestion').select2();

            if ($(this).valid()) {
                commonFunctions.hideValidationSummary($createEmploymentForm);
                var formData = $createEmploymentForm.serialize() + '&' + $('.createPersonalDetailsForm').serialize();

                $.ajax({
                    type: 'POST',
                    url: commonFunctions.rootURL + 'EmployeeManagement/Create/Employment',
                    dataType: 'json',
                    data: formData,
                    success: function (response) {
                        if (response.isSuccessful) {
                        } else {
                            if (response.errors) {
                                $createEmploymentForm.validate().showErrors(response.errors);
                            } else {
                                $createEmploymentForm.find('.list-message-error').removeClass('hidden')
                                    .append('<span class="message">' + response.message + '</span>');
                            }

                            commonFunctions.showValidationSummary($createEmploymentForm);
                        }
                    },
                    error: commonFunctions.handleAjaxError
                });
            } else {
                commonFunctions.showValidationSummary($createEmploymentForm);
            }
        });
    }

    function loadEmploymentForm() {
        $.ajax({
            type: 'GET',
            url: commonFunctions.rootURL + 'EmployeeManagement/Create/Employment',
            dataType: 'html',
            success: function (data) {
                $employmentContentTab.html(data);

                $createEmploymentForm = $('#createEmploymentForm');
                $companyCodeElement = $createEmploymentForm.find('#CompanyCode');
                $employeeCodeElement = $createEmploymentForm.find('#EmployeeCode');
                $pdfPasswordElement = $createEmploymentForm.find('#PDFPassword');
                $btnBack = $createEmploymentForm.find('.js-btnBack');

                employmentCommonFuncs.initForm($createEmploymentForm);

                initEventChangeCompanyCode();
                initEventChangeEmployeeCode();
                initEventBackToPreviousAction();
                registerEventSubmitForm();
            },
            error: commonFunctions.handleAjaxError
        });
    }

    function initEventClickEmploymentTab() {
        $employmentHeaderTab.on('click', function () {
            var isFirstLoad = $(this).data('isfirstload');

            if (isFirstLoad) {
                $(this).data('isfirstload', false)
                loadEmploymentForm();
            }
        });
    }

    function initModule() {
        $personalDetailsStep = $('#PersonalDetailsStep');
        $employmentStep = $('#EmploymentStep');
        $personalDetailHeaderTab = $('#PersonalDetailHeaderTab');
        $employmentHeaderTab = $('#EmploymentHeaderTab');
        $employmentContentTab = $('#EmploymentContentTab');

        initEventClickEmploymentTab();
    }

    return {
        initModule: initModule
    };
}(jQuery));

$(document).ready(function () {
    createEmployment.initModule();
});