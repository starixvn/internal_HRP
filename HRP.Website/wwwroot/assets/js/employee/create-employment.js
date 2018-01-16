'use strict';

var createEmployment = (function ($) {
    var $createEmploymentForm, $companyCodeElement, $employeeCodeElement, $pdfPasswordElement;

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

    function initModule() {
        $createEmploymentForm = $('#createEmploymentForm');
        $companyCodeElement = $createEmploymentForm.find('#CompanyCode');
        $employeeCodeElement = $createEmploymentForm.find('#EmployeeCode');
        $pdfPasswordElement = $createEmploymentForm.find('#PDFPassword');

        employmentCommonFuncs.initForm($createEmploymentForm);

        initEventChangeCompanyCode();
        initEventChangeEmployeeCode();
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