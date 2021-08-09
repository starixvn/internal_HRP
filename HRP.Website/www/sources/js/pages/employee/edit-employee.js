import 'jquery-ui-dist/jquery-ui';
import 'select2/dist/js/select2';
import 'bootstrap/dist/js/bootstrap';
import 'jquery-validation/dist/jquery.validate';
import 'jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive';
import 'jquery-ajax-unobtrusive/dist/jquery.unobtrusive-ajax';

import './employee-common-funcs';
import './employment-common-funcs';
import './edit-employment';

const editEmployee = (function ($) {
    let $form;

    function registerEventSubmitForm() {
        $form.submit(function (e) {
            e.preventDefault();

            if ($(this).valid()) {
                var formData = new FormData(this);

                $.ajax({
                    type: 'POST',
                    url: commonFunctions.rootURL + 'Employee/Update/PersonalDetails',
                    dataType: 'json',
                    contentType: false,
                    data: formData,
                    processData: false,
                    success: function (response) {
                        if (response.isSuccessful) {
                            commonFunctions.showSuccessMessage($form.find('.msg-success'));
                        } else {
                            if (response.errors) {
                                $form.validate().showErrors(response.errors);
                            } else {
                                commonFunctions.showErrorMessage($form.find('.msg-error'), response.message);
                            }
                        }
                    },
                    error: commonFunctions.handleAjaxError
                });
            }
        });
    }

    function initModule() {
        $form = $('#personalDetailsForm');
        commonFunctions.initDatetimepicker($form);
        commonFunctions.disableEnterKeySubmit($form);

        if ($form.find('#IsHost').prop('checked')) {
            employeeCommonFuncs.initForm($form);
            registerEventSubmitForm();
        } else {
            $form.find('input[type="text"]').attr('readonly', 'readonly');
            $form.find('select').attr('disabled', 'disabled');
            $form.submit(false);
            $form.find('.img-container').css('cursor', 'default');
        }
    }

    return {
        initModule: initModule
    };
}(jQuery));

$(function () {
    editEmployee.initModule();
});