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
        // Init DOB
        var dateOfBirth = $('#DateOfBirth');
        var defaultDate = moment(dateOfBirth.val(), 'MM/DD/YYYY hh:mm:ss a').format("MMM DD, YYYY");
        dateOfBirth.datetimepicker({
            format: 'll',
            minDate: '01/01/1900',
            maxDate: 'now'
        });
        dateOfBirth.data("DateTimePicker").date(defaultDate);

        // Init DI
        var dateIssue = $('#DateIssue');
        defaultDate = moment(dateIssue.val(), 'MM/DD/YYYY hh:mm:ss a').format("MMM DD, YYYY");
        dateIssue.datetimepicker({
            format: 'll',
            minDate: '01/01/1900',
            maxDate: 'now'
        });
        dateIssue.data("DateTimePicker").date(defaultDate);

        // Init ED
        var expiredDate = $('#ExpiredDate');
        defaultDate = moment(expiredDate.val(), 'MM/DD/YYYY hh:mm:ss a').format("MMM DD, YYYY");
        expiredDate.datetimepicker({
            format: 'll',
            minDate: '01/01/1900'
        });
        expiredDate.data("DateTimePicker").date(defaultDate);
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
                var urlPost = formData.get('EmployeeId') === '0' ? '/EmployeeManagement/CreateEmployee' : '/EmployeeManagement/EditEmployee';
                $.ajax({
                    url: urlPost,
                    type: "POST",
                    dataType: "json",
                    contentType: false,
                    data: formData,
                    processData: false,
                    success: function (result) {
                        if (result.isSuccessful) {
                            alert(result.message);
                        } else {
                            form.validate().showErrors(result.errors);
                            if (result.message) {
                                alert(result.message);
                            }
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