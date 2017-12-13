if ($('#update-profile-form').length) {
    var updateProfile = (function ($) {
        $(function () {
            var settings = $.data($('#update-profile-form')[0], 'validator').settings;
            settings.onkeyup = false;
        });

        return {
            onUpdateSuccess: function (response, status) {
                var code = response.code;
                var msg = response.message;
                if (!code) {
                    var alert = $('#update-profile-alert');
                    alert.removeClass('hidden');
                    setInterval(function () {
                        alert.addClass('hidden');
                        location.reload();
                    }, 3000);
                }
                if (code === -2) {
                }
            },
            OnUpdateFailure: function () {
                alert('Server Error');
            }
        };
    }(jQuery));
}