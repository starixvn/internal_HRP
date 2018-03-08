var uploadAttendanceAndSalary = (function ($) {
    var domElement = {
        UploadFilesForm: '#uploadFilesForm',
        SelectedTime: '#SelectedTime',
        AttendanceFile: '#AttendanceFile',
        DSTNetSalaryFile: '#DSTNetSalaryFile'
    };

    function showErrorsValidationFile($form, inputName, message) {
        var errorArray = {};
        errorArray[inputName] = message;
        $form.validate().showErrors(errorArray);
    }

    function validateFileExcel($form, inputName, file) {
        var errorMessage;
        var isValid = false;

        if (file === undefined) {
            errorMessage = '';
        } else {
            if (file.size / (1024 * 1024) > 5) {
                errorMessage = 'The upload file size can not exceed 5Mb. Please use a smaller file!';
            } else {
                var pass = /\.(xls|xlsx)$/i.test(file.name);
                if (!pass) {
                    errorMessage = 'The file type is not supported. Please try again.';
                } else {
                    isValid = true;
                }
            }
        }

        if (isValid) {
            showErrorsValidationFile($form, inputName, '');
        } else {
            showErrorsValidationFile($form, inputName, errorMessage);
        }

        return isValid;
    }

    function onUploadSuccess(response) {
        if (response.isSuccessful) {
            alert('success!');
        } else {
            $(domElement.UploadFilesForm).validate().showErrors(response.errors);
        }
    }

    function registerEventSubmitForm($form) {
        $form.submit(function (e) {
            e.preventDefault();

            if ($(this).valid()) {
                var formData = new FormData(this);

                $.ajax({
                    type: 'POST',
                    url: commonFunctions.rootURL + 'PaymentProcess/UploadAttendanceAndSalary',
                    dataType: 'json',
                    contentType: false,
                    data: formData,
                    processData: false,
                    success: onUploadSuccess,
                    error: commonFunctions.handleAjaxError
                });
            }
        });
    }

    function registerEventChangeFileExcel($form) {
        $(domElement.AttendanceFile).on('change', function () {
            var isValid = validateFileExcel($form, "AttendanceFile", this.files[0]);
            if (!isValid) {
                $(this).val('');
            }
        });

        $(domElement.DSTNetSalaryFile).on('change', function () {
            var isValid = validateFileExcel($form, "DSTNetSalaryFile", this.files[0]);
            if (!isValid) {
                $(this).val('');
            }
        });
    }

    function init() {
        var $form = $(domElement.UploadFilesForm);
        registerEventChangeFileExcel($form);
        commonFunctions.showHideValidationSummary($form.find('.list-message-error').get(0));
        registerEventSubmitForm($form);
    }

    return {
        init: init
    };
}(jQuery));

$(document).ready(function () {
    uploadAttendanceAndSalary.init();
});