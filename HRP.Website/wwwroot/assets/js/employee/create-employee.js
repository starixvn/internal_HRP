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
        $('#ProfilePicture').valid();
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

    function initModule() {
        registerEventChangePicture();
    }

    return {
        InitModule: initModule
    };
}(jQuery));

$(function () {
    createEmployee.InitModule();
});