import 'jquery-validation/dist/jquery.validate';
import '../../common/client-encryption';

const resetPassword = {
    init: function() {
        resetPassword.toggleShowPassword();
    },

    toggleShowPassword: function() {
        $('#showPassword').on('click', function () {
            const value = $(this).prop('checked');
            if (value) {
                $('input[type=password').attr('type', 'text');
            } else {
                $('input[type=text').attr('type', 'password');
            }
        });
    }
}

$(document).ready(function () {
    resetPassword.init();
})