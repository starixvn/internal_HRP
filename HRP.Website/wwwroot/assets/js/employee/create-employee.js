var createEmployee = (function ($) {
    var $form, $personalDetailsStep, $employmentStep;

    function registerEventSubmitForm() {
        $form.submit(function (e) {
            e.preventDefault();

            employeeCommonFuncs.initSelect2($form);

            if ($(this).valid()) {
                var formData = new FormData(this);

                $.ajax({
                    type: 'POST',
                    url: commonFunctions.rootURL + 'Employee/Create/PersonalDetails',
                    dataType: 'json',
                    contentType: false,
                    data: formData,
                    processData: false,
                    success: onUpdateSuccess,
                    error: commonFunctions.handleAjaxError
                });
            }
        });
    }

    function onUpdateSuccess(response) {
        if (response.isSuccessful) {
            $personalDetailsStep.removeClass('active').addClass('finished');
            $employmentStep.addClass('active');
            $('#EmploymentHeaderTab').trigger('click');
        } else {
            $form.validate().showErrors(response.errors);
        }
    }

    function initModule() {
        $form = $('#personalDetailsForm');
        $personalDetailsStep = $('#PersonalDetailsStep');
        $employmentStep = $('#EmploymentStep');

        commonFunctions.initDatetimepicker($form);
        employeeCommonFuncs.initForm($form);
        registerEventSubmitForm();
        commonFunctions.disableEnterKeySubmit($form);
    }

    return {
        initModule: initModule
    };
}(jQuery));

$(document).ready(function () {
    createEmployee.initModule();
});