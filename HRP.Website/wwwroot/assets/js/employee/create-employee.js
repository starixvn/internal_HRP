var createEmployee = (function ($) {
    function uploadProfilePicture() {
        var message;
        var formData = new FormData();
        var image = $('#profilePicture').prop('files')[0];
        if (image === undefined) {
            return;
        }

        if (image.size / (1024 * 1024) > 5) {
            message = 'This file is larger than 5MB. Sorry, it cannot be uploaded.';
            return;
        }

        var pass = /\.(gif|jpe?g|png)$/i.test(image.name);
        if (!pass) {
            message = 'Please choose an image!';
            return;
        }

        formData.append('image', image);
        $.ajax({
            type: "POST",
            url: '/EmployeeManagement/UploadImage',
            data: formData,
            dataType: 'json',
            contentType: false,
            processData: false,
            success: function (response) {
                message = response.Message;
            },
            error: function (error) {
                message = 'Error';
            }
        });
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