'use strict';

var updateEmployment = (function ($) {
    var $updateEmploymentForm, $employmentHeaderTab, $employmentContentTab, $btnSubmit;

    function registerEventSubmitForm() {
        $btnSubmit.on('click', function () {
            employmentCommonFuncs.initSelect2($updateEmploymentForm);

            if ($updateEmploymentForm.valid()) {
                $updateEmploymentForm.submit();
            } else {
                commonFunctions.showValidationSummary($updateEmploymentForm);
            }

            $('.content-container').scrollTop(0);
        });
    }

    function loadEmploymentForm() {
        var employeeId = $('#personalDetailsForm #EmployeeId').val();
        $.ajax({
            type: 'GET',
            url: commonFunctions.rootURL + 'EmployeeManagement/Update/Employment',
            dataType: 'html',
            data: {
                employeeId: employeeId
            },
            success: function (data) {
                $employmentContentTab.html(data);

                $updateEmploymentForm = $('#updateEmploymentForm');
                $btnSubmit = $updateEmploymentForm.find('.js-btnSubmit');

                if ($updateEmploymentForm.find('#IsHost').prop('checked')) {
                    employmentCommonFuncs.initForm($updateEmploymentForm);
                    registerEventSubmitForm();
                } else {
                    $updateEmploymentForm.find('input[type="text"]').attr('readonly', 'readonly');
                    $updateEmploymentForm.find('select').attr('disabled', 'disabled');
                    $updateEmploymentForm.submit(false);
                }
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
        $employmentHeaderTab = $('#EmploymentHeaderTab');
        $employmentContentTab = $('#EmploymentContentTab');

        initEventClickEmploymentTab();
    }

    return {
        initModule: initModule,
        onUpdateSuccess: function (response) {
            if (response.isSuccessful) {
                commonFunctions.showSuccessMessage('Updated sucessfully!');
            } else {
                if (response.errors) {
                    $updateEmploymentForm.validate().showErrors(response.errors);
                    commonFunctions.showValidationSummary($updateEmploymentForm);
                } else {
                    commonFunctions.showErrorMessage(response.message);
                }
            }
        }
    };
}(jQuery));

$(document).ready(function () {
    updateEmployment.initModule();
});