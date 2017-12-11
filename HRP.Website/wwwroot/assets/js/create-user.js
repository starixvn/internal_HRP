var createUser = (function ($) {
    $(function () {
        var settings = $.data($('#create-user-form')[0], 'validator').settings;
        settings.onkeyup = false;
    });

    return {
        onCreateBegin: function () {
            $('#btn-create').button('loading')
        },
        onCreateSuccess: function (response, status) {
            var alert = $('#create-user-alert');
            var delay = 5;
            var textAlert = response.username + ' account has been created successfully. You will be redirected in ';
            alert.removeClass('hidden');
            $('#alert-message', alert).text(textAlert + delay + ' seconds.');

            window.setInterval(function () {
                if (delay === 0) {
                    window.location.href = response.redirectTo;
                } else {
                    delay--;
                    $('#alert-message', alert).text(textAlert + delay + ' seconds.');
                }
            }, 1000); 
        },
        onCreateComplete: function () {
            $('#btn-create').button('reset');
        }
    };
}(jQuery));