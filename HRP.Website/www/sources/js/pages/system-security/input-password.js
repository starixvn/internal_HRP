import 'jquery-validation/dist/jquery.validate';
import 'jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive';
import rijndaelService from '../../common/rijndael-service';

$(document).ready(function () {
    $('form').on('submit', function (e) {
        var password = $('#Password').val();
        var rijndaelKey = $('#RijndaelKey').val();
        var rijndaelIV = $('#RijndaelIV').val();

        if (password === '' || rijndaelKey === '' || rijndaelIV === '') {
            return;
        }

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