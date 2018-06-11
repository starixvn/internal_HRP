'use strict';

$(document).ready(function () {
    var rijndaelKey = $('#RijndaelKey').val();
    var rijndaelIV = $('#RijndaelIV').val();

    $('.js-encrypted').each(function () {
        var value = $(this).html();
        var plainText = rijndaelService.decryptData(value, rijndaelKey, rijndaelIV);

        $(this).html(plainText).removeClass('hidden');
    });
});