var createEmployee = (function ($) {
    var $form, $personalDetailsStep, $employmentStep;

    function registerEventSubmitForm() {
        $form.submit(function (e) {
            e.preventDefault();

            if ($(this).valid()) {
                commonFunctions.hideValidationSummary($form);
                var formData = $form.serialize();

                $.ajax({
                    type: 'POST',
                    url: commonFunctions.rootURL + 'EmployeeManagement/Create/Employee',
                    dataType: 'json',
                    data: formData,
                    success: function (response) {
                        //if (response.isSuccessful) {
                        //    $personalDetailsStep.removeClass('active').addClass('finished');
                        //    $employmentStep.addClass('active');
                        //    $('#EmploymentHeaderTab').trigger('click');
                        //} else {
                        //    $form.validate().showErrors(response.errors);
                        //    commonFunctions.showValidationSummary($form);
                        //}
                    },
                    error: commonFunctions.handleAjaxError
                });
            } else {
                commonFunctions.showValidationSummary($form);
            }
        });
    }

    function initModule() {
        $form = $('.createPersonalDetailsForm');
        $personalDetailsStep = $('#PersonalDetailsStep');
        $employmentStep = $('#EmploymentStep');

        employeeCommonFuncs.initDatePicker();
        employeeCommonFuncs.initForm($form);
        registerEventSubmitForm();
    }

    return {
        initModule: initModule
    };
}(jQuery));

$(document).ready(function () {
    createEmployee.initModule();
});