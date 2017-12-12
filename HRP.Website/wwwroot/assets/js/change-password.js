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
                var alert = $('#update-password-alert');
                alert.removeClass('hidden');
                $('.icon-close').addClass('hidden');
                $('#old-password-incorrect').addClass('hidden');
                $('input').val('');
                setInterval(function () { alert.addClass('hidden'); }, 3000);
            }
            if (code === -2) {
                $('.icon-close').removeClass('hidden');
                $('#old-password-incorrect').removeClass('hidden');
                $('#old-password-incorrect').text(msg);
            } 
        },
        onChangeFail: function () {
            alert('Server Error');
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