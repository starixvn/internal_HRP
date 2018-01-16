var editEmployee = (function ($) {
    function isHostFunction() {
        var isHost = $('#IsHost').val() === "True";
        return isHost;
    }

    function registerEventFormSubmit() {
        var form = $('#personalDetailForm');
        form.submit(function (e) {
            e.preventDefault();

            if ($(this).valid()) {
                //hideListMessageError();
                //hideButtonSave();
                var formData = new FormData(this);
                var urlPost = '/EmployeeManagement/EditEmployee';
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
                            //showButtonSave();
                        } else {
                            //employeeCommonFuncs.showButtonSave();
                            var form = $('#personalDetailForm');
                            form.validate().showErrors(result.errors);
                        }
                    },
                    error: function (result) {
                        commonFunctions.showErrorMessage("Internal Error!");
                    }
                });
            } else {
                employeeCommonFuncs.showListMessageError();
            }
        });
    }

    function initModule() {
        employeeCommonFuncs.initDatePicker();
        if (isHostFunction()) {
            employeeCommonFuncs.initForm();
            registerEventFormSubmit();
        } else {
            $('input[type="text"]').attr('readonly', 'readonly');
            $('select').attr('disabled', 'disabled');
            $('form').submit(false);
            $('.img-container').css('cursor', 'default');
        }
    }

    return {
        InitModule: initModule
    };
}(jQuery));

$(function () {
    editEmployee.InitModule();
});