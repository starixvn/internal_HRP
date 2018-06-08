$(document).ready(function () {
    $('form').on('submit', function (e) {
        var password = $('#Password').val();
        var key = $('#RijndaelKey').val();
        var iv = $('#RijndaelIV').val();

        if (password === '' || key === '' || iv === '') {
            return;
        }

        var rijndaelKey = CryptoJS.enc.Utf8.parse(key);
        var rijndaelIV = CryptoJS.enc.Utf8.parse(iv);
        var encrypted = rijndaelService.encryptData(password, rijndaelKey, rijndaelIV);

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