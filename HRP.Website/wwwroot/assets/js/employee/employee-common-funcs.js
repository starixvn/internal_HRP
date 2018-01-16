var employeeCommonFuncs = (function ($) {
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
                errorMessage = 'The attachment size exceeds the allowable limits (2MB). Please use a smaller file size!';
            } else {
                var pass = /\.(jpe?g|png)$/i.test(image.name);
                if (!pass) {
                    errorMessage = 'The file type is not supported. Please try again.';
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

        $('.img-container').on('click', function () {
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
            maxDate: 'now',
            useCurrent: false
        });
        dateOfBirth.data("DateTimePicker").date(defaultDate);

        // Init DI
        var dateIssue = $('#DateIssue');
        defaultDate = moment(dateIssue.val(), 'MM/DD/YYYY hh:mm:ss a').format("MMM DD, YYYY");
        dateIssue.datetimepicker({
            format: 'll',
            minDate: '01/01/1900',
            maxDate: 'now',
            useCurrent: false
        });
        dateIssue.data("DateTimePicker").date(defaultDate);

        // Init ED
        var expiredDate = $('#ExpiredDate');
        defaultDate = moment(expiredDate.val(), 'MM/DD/YYYY hh:mm:ss a').format("MMM DD, YYYY");
        expiredDate.datetimepicker({
            format: 'll',
            minDate: '01/01/1900',
            useCurrent: false
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

    function registerEventFormChange() {
        var form = $('#personalDetailForm');
        form.on('change keyup click', function () {
            var errorTotal = $('.personal-detail-error').children(".field-validation-error").length;
            if (errorTotal === 0) {
                hideListMessageError();
            } else {
                showListMessageError();
            }
        });
    }

    function registerEventCancel() {
        $('.js-btnCancel').on('click', function () {
            window.location.href = "/employeemanagement";
        });
    }

    function isCreateNew() {
        return $('#EmployeeId').val() <= '0';
    }

    function showListMessageError() {
        $('.list-message-error').removeClass("hidden");
    }

    function hideListMessageError() {
        $('.list-message-error').addClass("hidden");
    }

    function hideButtonSave() {
        $('.js-btnSave').hide();
    }

    function showButtonSave() {
        $('.js-btnSave').show();
    }

    function showSuccessAndRedirectMessage(message, redirectLink) {
        var messageElement = $('.msg-success');
        var redirectMessage = " You will be redirected in ";
        var delay = 2;
        messageElement.text(message + redirectMessage + delay + ' seconds.').removeClass('hidden');

        window.setInterval(function () {
            if (delay === 0) {
                window.location.href = redirectLink;
            } else {
                delay--;
                messageElement.text(message + redirectMessage + delay + ' seconds.');
            }
        }, 1000);
    }


    function initForm() {
        registerEventOpenDatePicker();
        registerEventChangePicture();
        registerEventFormChange();
        registerEventCancel();
    }

    return {
        initForm: initForm,
        initDatePicker: initDatePicker,
        hideListMessageError: hideListMessageError, 
        showListMessageError: showListMessageError,
        hideButtonSave: hideButtonSave,
        showButtonSave: showButtonSave,
        showSuccessAndRedirectMessage: showSuccessAndRedirectMessage
    };
}(jQuery));