if ($('#change-password-form').length) {
    var changePassword = (function ($) {
        $(function () {
            var settings = $.data($('#change-password-form')[0], 'validator').settings;
            settings.onkeyup = false;
        });

        return {
            onChangeSuccess: function (response, status) {
                var code = response.code;
                var msg = response.message;
                if (!code) {
                    var alert = $('#update-password-success-alert');
                    alert.removeClass('hidden');
                    $('.icon-close').addClass('hidden');
                    $('#old-password-incorrect').addClass('hidden');
                    $('input').val('');
                    $('#OldPassword').removeClass('error');
                    setInterval(function () { alert.addClass('hidden'); }, 3000);
                }
                if (code === -2) {
                    $('.icon-close').removeClass('hidden');
                    $('#old-password-incorrect').removeClass('hidden');
                    $('#old-password-incorrect').text(msg);
                    $('#OldPassword').addClass('error');
                } else {
                    var failAlert = $('#update-password-fail-alert');
                    failAlert.removeClass('hidden');
                    setInterval(function () {
                        failAlert.addClass('hidden');
                        location.reload();
                    }, 3000);
                }
            }
        };
    }(jQuery));

    $('#showPassword').on('click', function () {
        var value = $(this).prop("checked");
        if (value) {
            $('input[type=password').attr("type", "text");
        } else {
            $('input[type=text').attr("type", "password");
        }
    });
}