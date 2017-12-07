(function ($) {
    $(function () {
        var settings = $.data($('#create-user-form')[0], 'validator').settings;
        settings.onkeyup = false;
    });
}(jQuery));