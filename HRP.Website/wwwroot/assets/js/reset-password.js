var resetPassword = {
    init: function() {
        resetPassword.toggleShowPassword();
    },

    toggleShowPassword: function() {
        $("#showPassword").on("click", function () {
            var value = $(this).prop("checked");
            if (value) {
                $("input[type=password").attr("type", "text");
            } else {
                $("input[type=text").attr("type", "password");
            }
        });
    }
}

$(document).ready(function () {
    resetPassword.init();
})