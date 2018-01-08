var createEmployee = (function ($) {
    function showPreviewPicture(image) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $('.js-displayProfilePicture').attr('src', e.target.result);
        };
        reader.readAsDataURL(image);
    }

    function showErrorsValidationPicture(message) {
        var form = $('#personalDetailForm');
        var errorArray = {};
        errorArray["ProfilePicture"] = message;
        form.validate().showErrors(errorArray);
    }

    function validatePicture(image) {
        var errorMessage;
        var isValid = false;

        if (image === undefined) {
            errorMessage = '';
        } else {
            if (image.size / (1024 * 1024) > 5) {
                errorMessage = 'This file is too large. Maximum file size is 5Mb.';
            } else {
                var pass = /\.(gif|jpe?g|png)$/i.test(image.name);
                if (!pass) {
                    errorMessage = 'Invalid file type. Please choose an image file.';
                } else {
                    isValid = true;
                }
            }
        }

        if (isValid) {
            showPreviewPicture(image);
            showErrorsValidationPicture('');
        } else {
            $('.js-displayProfilePicture').attr('src', '/assets/styles/images/profile.png');
            showErrorsValidationPicture(errorMessage);
        }

        return isValid;
    }

    function registerEventChangePicture() {
        $('.js-changePicture').on('click', function () {
            $('#ProfilePicture').trigger('click');
        });

        $('#ProfilePicture').on('change', function () {
            var isValid = validatePicture(this.files[0]);
            if (!isValid) {
                $('#ProfilePicture').val('');
            }
        });
    }

    function initDatePicker() {
        $('#DateOfBirth').datetimepicker({
            format: 'll'
        });

        $('#DateIssue').datetimepicker({
            format: 'll'
        });

        $('#ExpiredDate').datetimepicker({
            format: 'll'
        });
    }

    function registerEventOpenDatePicker() {
        $('.js-iconDateOfBirth').on('click', function () {
            $('#DateOfBirth').data("DateTimePicker").show();
        });

        $('.js-iconDateIssue').on('click', function () {
            $('#DateIssue').data("DateTimePicker").show();
        });

        $('.js-iconExpiredDate').on('click', function () {
            $('#ExpiredDate').data("DateTimePicker").show();
        });
    }

    function registerEventSubmit() {
        var form = $('#personalDetailForm');
        form.submit(function (e) {
            e.preventDefault();

            if ($(this).valid()) {
                var formData = new FormData(this);
                $.ajax({
                    url: '/EmployeeManagement/CreateEmployee',
                    type: "POST",
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    data: formData,
                    contentType: false,
                    processData: false,
                    success: function (result) {
                        if (result.isSuccessful) {
                            alert(result.message);
                        } else {
                            form.validate().showErrors(result.errors);
                        }
                    },
                    error: function (result) {
                    }
                });
            }
        });
    }

    function initModule() {
        initDatePicker();
        registerEventOpenDatePicker();
        registerEventChangePicture();
        registerEventSubmit();
    }

    return {
        InitModule: initModule
    };
}(jQuery));

$(function () {
    createEmployee.InitModule();
});