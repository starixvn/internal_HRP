var editEmployee = (function ($) {
    var $form;

    function isHostFunction() {
        var isHost = $('#IsHost').val() === "True";
        return isHost;
    }

    function registerEventFormSubmit() {
        $form.submit(function (e) {
            e.preventDefault();

            if ($(this).valid()) {
                var formData = new FormData(this);
                var urlPost = commonFunctions.rootURL + 'EmployeeManagement/Update/PersonalDetails';
                $.ajax({
                    url: urlPost,
                    type: "POST",
                    dataType: "json",
                    contentType: false,
                    data: formData,
                    processData: false,
                    success: function (result) {
                        if (result.isSuccessful) {
                            commonFunctions.showSuccessMessage(result.message);
                        } else {
                            $form.validate().showErrors(result.errors);
                        }
                    },
                    error: commonFunctions.handleAjaxError
                });
            } else {
                employeeCommonFuncs.showValidationSummary();
            }
        });
    }

    function initModule() {
        $form = $('#personalDetailsForm');
        commonFunctions.initDatetimepicker($form);

        if (isHostFunction()) {
            employeeCommonFuncs.initForm($form);
            registerEventFormSubmit();
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