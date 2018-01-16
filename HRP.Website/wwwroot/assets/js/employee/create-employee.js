var createEmployee = (function ($) {
    function registerEventFormSubmit() {
        var form = $('#personalDetailForm');
        form.submit(function (e) {
            e.preventDefault();

            if ($(this).valid()) {
                employeeCommonFuncs.hideListMessageError();
                employeeCommonFuncs.hideButtonSave();

                var formData = new FormData(this);
                var urlPost = '/EmployeeManagement/CreateEmployee';
                $.ajax({
                    url: urlPost,
                    type: "POST",
                    dataType: "json",
                    contentType: false,
                    data: formData,
                    processData: false,
                    success: function (result) {
                        if (result.isSuccessful) {
                            employeeCommonFuncs.showSuccessAndRedirectMessage(result.message, result.redirectLink);
                        } else {
                            employeeCommonFuncs.showButtonSave();
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
        employeeCommonFuncs.initForm();
        registerEventFormSubmit();
    }

    return {
        InitModule: initModule
    };
}(jQuery));

$(function () {
    createEmployee.InitModule();
});