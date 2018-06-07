$(document).ready(function () {
    $('form').on('submit', function (e) {
        var password = $('#Password').val();
        var key = $('#RijndaelKey').val();
        var iv = $('#RijndaelIV').val();

        if (password === '' || key === '' || iv === '') {
            return;
        }

        var encrypted = CryptoJS.AES.encrypt(
            CryptoJS.enc.Utf8.parse(password),
            CryptoJS.enc.Utf8.parse(key),
            {
                iv: CryptoJS.enc.Utf8.parse(iv),
                mode: CryptoJS.mode.CBC,
                padding: CryptoJS.pad.Pkcs7
            });

        $('#Password').val(encrypted);
    });

    $('#ShowPassword').on('click', function () {
        if ($(this).prop('checked')) {
            $('#Password').attr('type', 'text');
        } else {
            $('#Password').attr('type', 'password');
        }
    });
});