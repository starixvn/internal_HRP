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
            $('.js-displayProfilePicture').attr('src', '/css/images/profile.png');
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

    function initSelect2($form) {
        $form.find('.js-select2-hide-searchbox').select2({
            placeholder: '--Please select--',
            minimumResultsForSearch: Infinity
        });
    }

    function reparseform($form) {
        var $validator = $form.validate();
        $validator.settings.highlight = function (element, errorClass, validClass) {
            var $element = $(element);
            $element.addClass(errorClass).removeClass(validClass);

            if ($element.hasClass('js-select2-hide-searchbox')) {
                $element.next().find('.select2-selection')
                    .removeClass(errorClass).removeClass('control').removeClass(validClass)
                    .addClass(errorClass).addClass('control');
            }
        };
        $validator.settings.unhighlight = function (element, errorClass, validClass) {
            var $element = $(element);
            $element.removeClass(errorClass).addClass(validClass);

            if ($element.hasClass('js-select2-hide-searchbox')) {
                $element.next().find('.select2-selection')
                    .removeClass(errorClass).removeClass('control').removeClass(validClass)
                    .addClass(validClass);
            }
        };
        $form.find('.js-select2-hide-searchbox').on('change', function () {
            var $this = $(this);
            var $select2 = $this.next().find('.select2-selection');

            if ($this.val() && $select2.hasClass('input-validation-error')) {
                $select2.removeClass('input-validation-error').removeClass('control');
                $this.valid();
            }
        });
    }

    function initForm($form) {
        registerEventChangePicture($form);
        commonFunctions.showHideValidationSummary($form.find('.list-message-error').get(0));
        initSelect2($form);
        reparseform($form);
    }

    return {
        initForm: initForm,
        initSelect2: initSelect2
    };
}(jQuery));