if ($('#update-profile-form').length) {
    var updateProfile = (function ($) {
        $(function () {
            var settings = $.data($('#update-profile-form')[0], 'validator').settings;
            settings.onkeyup = false;
        });

        return {
            onUpdateSuccess: function (response, status) {
                var code = response.code;
                if (!code) {
                    var alert = $('#update-profile-success-alert');
                    alert.removeClass('hidden');
                    setInterval(function () {
                        alert.addClass('hidden');
                        location.reload();
                    }, 3000);
                } else {
                    var failAlert = $('#update-profile-fail-alert');
                    failAlert.removeClass('hidden');
                    setInterval(function () {
                        failAlert.addClass('hidden');
                        location.reload();
                    }, 3000);
                }
            }
        };
    }(jQuery));
}