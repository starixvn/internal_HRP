'use strict';

$(document).ready(function () {
    var rijndaelKey = CryptoJS.enc.Utf8.parse($('#RijndaelKey').val());
    var rijndaelIV = CryptoJS.enc.Utf8.parse($('#RijndaelIV').val());

    $('.js-encrypted').each(function (element) {
        var value = $(this).html();
        var plainText = rijndaelService.decryptData(value, rijndaelKey, rijndaelIV);

        $(this).html(plainText).removeClass('hidden');
    });
});