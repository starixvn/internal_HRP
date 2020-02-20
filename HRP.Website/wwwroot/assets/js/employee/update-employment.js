'use strict';

var updateEmployment = (function ($) {
    var $updateEmploymentForm, $employmentHeaderTab, $employmentContentTab, $btnSubmit;

    function registerEventSubmitForm() {
        $btnSubmit.on('click', function () {
            employmentCommonFuncs.initSelect2($updateEmploymentForm);

            if ($updateEmploymentForm.valid()) {
                $updateEmploymentForm.submit();
            }

            $('.content-container').scrollTop(0);
        });
    }

    function loadEmploymentForm() {
        var $personalDetailsForm = $('#personalDetailsForm');
        $.ajax({
            type: 'GET',
            url: commonFunctions.rootURL + 'Employee/Update/Employment',
            dataType: 'html',
            data: {
                id: $personalDetailsForm.find('#EmployeeId').val(),
                departmentIds: $personalDetailsForm.find('#Filter_DepartmentIds').val(),
                employmentStatusIds: $personalDetailsForm.find('#Filter_EmploymentStatusIds').val(),
                employeeId: $personalDetailsForm.find('#Filter_EmployeeId').val(),
                employeeName: $personalDetailsForm.find('#Filter_EmployeeName').val()
            },
            success: function (data) {
                $employmentContentTab.html(data);

                $updateEmploymentForm = $('#updateEmploymentForm');
                $btnSubmit = $updateEmploymentForm.find('.js-btnSubmit');
                employmentCommonFuncs.initForm($updateEmploymentForm);

                if ($updateEmploymentForm.find('#IsHost').prop('checked')) {
                    registerEventSubmitForm();
                } else {
                    $updateEmploymentForm.find('.js-select2').select2();
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
                $(this).data('isfirstload', false);
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
                commonFunctions.showSuccessMessage($updateEmploymentForm.find('.msg-success'));
            } else {
                if (response.errors) {
                    $updateEmploymentForm.validate().showErrors(response.errors);
                } else {
                    commonFunctions.showErrorMessage($updateEmploymentForm.find('.msg-error'), response.message);
                }
            }
        }
    };
}(jQuery));

$(document).ready(function () {
    updateEmployment.initModule();
});