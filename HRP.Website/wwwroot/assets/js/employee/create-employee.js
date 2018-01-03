var createEmployee = (function ($) {
    function uploadProfilePicture() {
        var formData = new FormData();
        var guidToken = $('#GuidToken');
        var image = $('#profilePicture').prop('files')[0];
        var isValid = validatePicture(image);

        if (!isValid) {
            return;
        }

        formData.append('image', image);
        formData.append('token', guidToken.val());
        $.ajax({
            type: "POST",
            url: '/EmployeeManagement/UploadImage',
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.code === 1) {
                    displayMessageValidattionPicture(response.message);
                } else {
                    var imagePath = response.message;
                    $('.js-displayProfilePicture').attr('src', imagePath);
                    $('#ProfilePic').val(imagePath);
                }
            },
            error: function (error) {
                displayMessageValidattionPicture('Upload failed.');
            }
        });
    }

    function validatePicture(image) {
        var errorMessage;
        var isValid = false;

        clearMessageValidattionPicture();
        if (image !== undefined) {
            if (image.size / (1024 * 1024) > 5) {
                errorMessage = 'This file is larger than 5MB. Sorry, it cannot be uploaded.';
            } else {
                var pass = /\.(gif|jpe?g|png)$/i.test(image.name);
                if (!pass) {
                    errorMessage = 'Please choose an image.';
                } else {
                    isValid = true;
                }
            }

            if (!isValid) {
                displayMessageValidattionPicture(errorMessage);
            }
        }

        return isValid;
    }

    function displayMessageValidattionPicture(message) {
        var labelValidation = $('.js-validationPicture');
        labelValidation.text(message);
        labelValidation.show();
    }

    function clearMessageValidattionPicture() {
        var labelValidation = $('.js-validationPicture');
        labelValidation.empty();
        labelValidation.hide();
    }

    function registerEventChangePicture() {
        $('.js-changePicture').on('click', function () {
            $('#profilePicture').trigger('click');
        });

        $('#profilePicture').on('change', function () {
            uploadProfilePicture();
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