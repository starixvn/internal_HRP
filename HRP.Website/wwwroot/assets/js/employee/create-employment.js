'use strict';

var createEmployment = (function ($) {
    var $personalDetailsStep, $employmentStep,
        $personalDetailHeaderTab, $employmentHeaderTab, $employmentContentTab, $createEmploymentForm,
        $companyCodeElement, $employeeCodeElement, $pdfPasswordElement, $btnBack;

    function initEventChangeCompanyCode() {
        $companyCodeElement.on('change', function () {
            $pdfPasswordElement.val($companyCodeElement.val().trim() + $employeeCodeElement.val().trim());
        });
    }

    function initEventChangeEmployeeCode() {
        $employeeCodeElement.on('change', function () {
            $pdfPasswordElement.val($companyCodeElement.val().trim() + $employeeCodeElement.val().trim());
        });
    }

    function initEventBackToPreviousAction() {
        $btnBack.on('click', function () {
            $personalDetailsStep.removeClass('finished').addClass('active');
            $employmentStep.removeClass('active');
            $personalDetailHeaderTab.trigger('click');
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
        initModule: initModule,
        onUpdateSuccess: function (response) {
            if (response.isSuccessful) {
                $createEmploymentForm.find('.list-message-error').addClass('hidden');
            } else {
                if (response.errors) {
                    $createEmploymentForm.validate().showErrors(response.errors);
                } else {
                    $createEmploymentForm.find('.list-message-error').removeClass('hidden')
                        .append('<span class="message">' + response.message + '</span>');
                }
            }
        }
    };
}(jQuery));

$(document).ready(function () {
    createEmployment.initModule();
});