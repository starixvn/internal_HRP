'use strict';

var createEmployment = (function ($) {
    var $createEmploymentForm, $companyCodeElement, $employeeCodeElement, $pdfPasswordElement, $btnBack;

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
            $createEmploymentForm.find('#IsBack').prop('checked', true);
            $('#createEmploymentForm')[0].submit();
        });
    }

    function initModule() {
        $createEmploymentForm = $('#createEmploymentForm');
        $companyCodeElement = $createEmploymentForm.find('#CompanyCode');
        $employeeCodeElement = $createEmploymentForm.find('#EmployeeCode');
        $pdfPasswordElement = $createEmploymentForm.find('#PDFPassword');
        $btnBack = $createEmploymentForm.find('.js-btnBack');

        employmentCommonFuncs.initForm($createEmploymentForm);

        initEventChangeCompanyCode();
        initEventChangeEmployeeCode();
        initEventBackToPreviousAction();
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