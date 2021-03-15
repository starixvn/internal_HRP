import rijndaelService from '../../common/rijndael-service';

$(document).ready(function () {
    const rijndaelKey = $('#RijndaelKey').val(),
        rijndaelIV = $('#RijndaelIV').val();

    $('.js-encrypted').each(function () {
        const value = $(this).html(),
            plainText = rijndaelService.decryptData(value, rijndaelKey, rijndaelIV);

        $(this).html(plainText).removeClass('hidden');
    });
});