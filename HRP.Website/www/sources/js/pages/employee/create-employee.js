import 'jquery-ui-dist/jquery-ui';
import 'select2/dist/js/select2';
import 'jquery-validation/dist/jquery.validate';
import 'jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive';
import 'jquery-ajax-unobtrusive/dist/jquery.unobtrusive-ajax';

import './employee-common-funcs';
import './employment-common-funcs';
import './create-employment';

const createEmployee = (function ($) {
    let $form, $personalDetailsStep, $employmentStep;

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