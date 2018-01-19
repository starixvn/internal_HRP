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
            if (image.size / (1024 * 1024) > 2) {
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

    function registerEventFormChange($form) {
        $form.on('change keyup click', function () {
            var errorTotal = $form.find('.list-message-error .field-validation-error').length;
            if (errorTotal === 0) {
                commonFunctions.hideValidationSummary($form);
            } else {
                commonFunctions.showValidationSummary($form);
            }
        });
    }

    function initForm($form) {
        registerEventChangePicture($form);
        registerEventFormChange($form);
    }

    return {
        initForm: initForm
    };
}(jQuery));