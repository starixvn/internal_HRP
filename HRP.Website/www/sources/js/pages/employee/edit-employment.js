window.editEmployment = (function ($) {
    let $editEmploymentForm, $employmentHeaderTab, $employmentContentTab, $btnSubmit;

    function registerEventSubmitForm() {
        $btnSubmit.on('click', function () {
            employmentCommonFuncs.initSelect2($editEmploymentForm);

            if ($editEmploymentForm.valid()) {
                $editEmploymentForm.submit();
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

                $editEmploymentForm = $('#updateEmploymentForm');
                $btnSubmit = $editEmploymentForm.find('.js-btnSubmit');
                employmentCommonFuncs.initForm($editEmploymentForm);

                if ($editEmploymentForm.find('#IsHost').prop('checked')) {
                    registerEventSubmitForm();
                } else {
                    $editEmploymentForm.find('.js-select2').select2();
                    $editEmploymentForm.find('input[type="text"]').attr('readonly', 'readonly');
                    $editEmploymentForm.find('select').attr('disabled', 'disabled');
                    $editEmploymentForm.submit(false);
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
                commonFunctions.showSuccessMessage($editEmploymentForm.find('.msg-success'));
            } else {
                if (response.errors) {
                    $editEmploymentForm.validate().showErrors(response.errors);
                } else {
                    commonFunctions.showErrorMessage($editEmploymentForm.find('.msg-error'), response.message);
                }
            }
        }
    };
}(jQuery));

$(document).ready(function () {
    editEmployment.initModule();
});