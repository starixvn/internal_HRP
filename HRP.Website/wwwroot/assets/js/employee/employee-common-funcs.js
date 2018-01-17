var employeeCommonFuncs = (function ($) {
    function showPreviewPicture(image) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $('.js-displayProfilePicture').attr('src', e.target.result);
        };
        reader.readAsDataURL(image);
    }

    function showErrorsValidationPicture($form, message) {
        var errorArray = {};
        errorArray["ProfilePicture"] = message;
        $form.validate().showErrors(errorArray);
    }

    function validatePicture($form, image) {
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
            showErrorsValidationPicture($form, '');
        } else {
            $('.js-displayProfilePicture').attr('src', '/assets/styles/images/profile.png');
            showErrorsValidationPicture($form, errorMessage);
        }

        return isValid;
    }

    function registerEventChangePicture($form) {
        $('.js-changePicture').on('click', function () {
            $('#ProfilePicture').trigger('click');
        });

        $('.img-container').on('click', function () {
            $('#ProfilePicture').trigger('click');
        });

        $('#ProfilePicture').on('change', function () {
            var isValid = validatePicture($form, this.files[0]);
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

    function registerEventFormChange($form) {
        $('#personalDetailForm').on('change keyup click', function () {
            var errorTotal = $form.find('.personal-detail-error').children(".field-validation-error").length;
            if (errorTotal === 0) {
                hideListMessageError($form);
            } else {
                showValidationSummary($form);
            }
        });
    }

    function showValidationSummary($form) {
        $form.find('.list-message-error').removeClass("hidden");
    }

    function hideListMessageError($form) {
        $form.find('.list-message-error').addClass("hidden");
    }

    function initForm($form) {
        registerEventOpenDatePicker($form);
        registerEventChangePicture($form);
    }

    return {
        initForm: initForm,
        initDatePicker: initDatePicker,
        hideListMessageError: hideListMessageError,
        showValidationSummary: showValidationSummary
    };
}(jQuery));