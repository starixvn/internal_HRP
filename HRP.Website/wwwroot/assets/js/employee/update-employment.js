'use strict';

var updateEmployment = (function ($) {
    var $updateEmploymentForm;

    function loadEmploymentForm() {
        var employeeId = $('#personalDetailForm #EmployeeId').val();
        $.ajax({
            type: 'GET',
            url: commonFunctions.rootURL + 'EmployeeManagement/UpdateEmployment',
            dataType: 'html',
            data: {
                employeeId: employeeId
            },
            success: function (data) {
                $employmentTab.html(data);

                $updateEmploymentForm = $('#updateEmploymentForm');

                if ($updateEmploymentForm.find('#IsHost').prop('checked')) {
                    employmentCommonFuncs.initForm($updateEmploymentForm);
                } else {
                    $updateEmploymentForm.find('input[type="text"]').attr('readonly', 'readonly');
                    $updateEmploymentForm.find('select').attr('disabled', 'disabled');
                    $updateEmploymentForm.submit(false);
                }
            },
            error: commonFunctions.handleAjaxError
        });
    }

    function initModule() {
        loadEmploymentForm();
    }

    return {
        initModule: initModule,
        onUpdateSuccess: function (response) {
            if (response.isSuccessful) {
                $updateEmploymentForm.find('.list-message-error').addClass('hidden');
            } else {
                if (response.errors) {
                    $updateEmploymentForm.validate().showErrors(response.errors);
                } else {
                    $updateEmploymentForm.find('.list-message-error').removeClass('hidden')
                        .append('<span class="message">' + response.message + '</span>');
                }
            }
        }
    };
}(jQuery));

$(document).ready(function () {
    updateEmployment.initModule();
});