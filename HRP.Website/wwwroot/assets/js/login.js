var login = (function ($) {
    function validateInput() {
        var allInputs = $('input').length;
        var filledInputs = 0;

        $('input').each(function (index, element) {
            var inputValue = $(element).val();
            if (!inputValue) {
                $(this).parent().effect('shake');
            } else {
                filledInputs++;
            }
        });

        if (allInputs === filledInputs) { return true; }

        return false;
    }

    function doLogin() {
        if (!validateInput()) {
            return false;
        }

        var password = $('#Password').val();
        var key = CryptoJS.enc.Utf8.parse($('#RijndaelKey').val());
        var iv = CryptoJS.enc.Utf8.parse($('#RijndaelIV').val());
        var encrypted = CryptoJS.AES.encrypt(
            CryptoJS.enc.Utf8.parse(password),
            key,
            {
                iv: iv,
                mode: CryptoJS.mode.CBC,
                padding: CryptoJS.pad.Pkcs7
            });

        $('#Password').val(encrypted);
        $('#login').submit();

        return true;
    }

    function initModule() {
        $('#btn-submit').click(doLogin);

        $('#Username, #Password').on('keypress', function (e) {
            if (e.keyCode === 13) {
                $('#btn-submit').trigger('click');
            }
        });
    }

    return {
        initModule: initModule
    };
}(jQuery));

$(document).ready(function () {
    login.initModule();
});