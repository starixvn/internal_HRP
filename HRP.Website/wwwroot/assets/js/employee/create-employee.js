var createEmployee = (function ($) {
    var $createPersonalDetalsForm, $personalDetailsStep, $employmentStep;

    function registerEventSubmitForm() {
        $createPersonalDetalsForm.submit(function (e) {
            e.preventDefault();

            if ($(this).valid()) {
                commonFunctions.hideValidationSummary($createPersonalDetalsForm);
                var formData = new FormData(this);

                $.ajax({
                    type: 'POST',
                    url: commonFunctions.rootURL + 'EmployeeManagement/Create/PersonalDetails',
                    dataType: 'json',
                    contentType: false,
                    data: formData,
                    processData: false,
                    success: function (response) {
                        if (response.isSuccessful) {
                            $personalDetailsStep.removeClass('active').addClass('finished');
                            $employmentStep.addClass('active');
                            $('#EmploymentHeaderTab').trigger('click');
                        } else {
                            $createPersonalDetalsForm.validate().showErrors(response.errors);
                            commonFunctions.showValidationSummary($createPersonalDetalsForm);
                        }
                    },
                    error: commonFunctions.handleAjaxError
                });
            } else {
                commonFunctions.showValidationSummary($createPersonalDetalsForm);
            }
        });
    }

    function initModule() {
        $createPersonalDetalsForm = $('.createPersonalDetailsForm');
        $personalDetailsStep = $('#PersonalDetailsStep');
        $employmentStep = $('#EmploymentStep');

        employeeCommonFuncs.initDatePicker();
        employeeCommonFuncs.initForm($createPersonalDetalsForm);
        registerEventSubmitForm();
    }

    return {
        initModule: initModule
    };
}(jQuery));

$(document).ready(function () {
    createEmployee.initModule();
});