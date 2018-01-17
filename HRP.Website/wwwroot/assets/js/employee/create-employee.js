var createEmployee = (function ($) {
    var $form, $personalDetailsStep, $employmentStep;

    function onSubmitSuccess(response) {
        if (response.isSuccessful) {
            $personalDetailsStep.removeClass('active').addClass('finished');
            $employmentStep.addClass('active');
            $('#EmploymentHeaderTab').trigger('click');
        } else {
            $form.validate().showErrors(result.errors);
        }
    }

    function initModule() {
        $form = $('.createPersonalDetailsForm');
        $personalDetailsStep = $('#PersonalDetailsStep');
        $employmentStep = $('#EmploymentStep');

        employeeCommonFuncs.initDatePicker();
        employeeCommonFuncs.initForm($form);
    }

    return {
        InitModule: initModule,
        onSubmitSuccess: onSubmitSuccess
    };
}(jQuery));

$(document).ready(function () {
    createEmployee.InitModule();
});