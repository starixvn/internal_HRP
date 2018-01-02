if ($('#update-profile-form').length) {
    var updateProfile = (function ($) {
        $(function () {
            var settings = $.data($('#update-profile-form')[0], 'validator').settings;
            settings.onkeyup = false;
        });

        var displayValidationErrorMessages = function (errors) {
            errors.forEach(function (error) {
                $('[data-valmsg-for="' + error.key + '"]').text(error.errorMessage);
            });
        };

        return {
            onUpdateSuccess: function (response, status) {
                if (response.isSuccessful) {
                    var alert = $('#update-profile-success-alert');
                    alert.removeClass('hidden');
                    setInterval(function () {
                        alert.addClass('hidden');
                    }, 3000);
                } else {

                    if (response.errors) {
                        displayValidationErrorMessages(response.errors);
                    }

                    if (response.message) {
                        var failAlert = $('#update-profile-fail-alert');
                        failAlert.removeClass('hidden');
                        setInterval(function () {
                            failAlert.addClass('hidden');
                        }, 3000);
                    }
                }
            }
        };
    }(jQuery));
}