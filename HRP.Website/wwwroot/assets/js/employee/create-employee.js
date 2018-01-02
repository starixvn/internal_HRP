var createEmployee = (function ($) {
     
    function registerEventChangePicture() {
        $('.js-changePicture').on('click', function () {
            $('#profilePicture').trigger('click');
        });

        $('#profilePicture').on('change', function () {
            
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